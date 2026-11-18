using System.IO;
using System.Text.Json;
using System.Windows;
using NAudio.CoreAudioApi;
using MeetingCopilot.Infrastructure;
using MeetingCopilot.Core;

namespace MeetingCopilot.Desktop;

public partial class MainWindow : Window
{
    private IStreamingSession? session;
    private CancellationTokenSource? startup;
    private bool closing;
    private bool stopping;

    public MainWindow()
    {
        InitializeComponent();
        try
        {
            using var enumerator = new MMDeviceEnumerator();
            Devices.ItemsSource = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                .Select(d => new DeviceOption(d.ID, d.FriendlyName)).ToList();
            Devices.SelectedIndex = 0;
            if (Devices.SelectedIndex < 0) Status.Text = "No active playback device was found. Connect a device and restart the application.";
        }
        catch (Exception ex) { Status.Text = $"Device enumeration failed: {ex.Message}"; }
    }

    private async void Start_Click(object sender, RoutedEventArgs e)
    {
        if (Devices.SelectedItem is not DeviceOption device) { Status.Text = "Select a playback device."; return; }
        StartButton.IsEnabled = false;
        StopButton.IsEnabled = true;
        Devices.IsEnabled = false;
        var tokenSource = new CancellationTokenSource();
        startup = tokenSource;
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) throw new InvalidOperationException("Create appsettings.json beside the application using appsettings.example.json and add your Deepgram key.");
            using var json = JsonDocument.Parse(await File.ReadAllTextAsync(path, tokenSource.Token));
            var key = json.RootElement.GetProperty("Deepgram").GetProperty("ApiKey").GetString();
            if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Set Deepgram:ApiKey in appsettings.json.");
            var current = new StreamingSession();
            session = current;
            current.Transcript += (text, final) => Dispatcher.BeginInvoke(() =>
            {
                if (session != current || stopping) return;
                if (final) { Transcript.AppendText(text + Environment.NewLine); Transcript.ScrollToEnd(); Status.Text = "Listening."; }
                else Status.Text = "Listening: " + text;
            });
            current.Failed += message => Dispatcher.BeginInvoke(async () =>
            {
                if (session != current || stopping) return;
                await StopAsync();
                Status.Text = message + " Select Start listening to retry.";
            });
            Status.Text = "Connecting to Deepgram...";
            await current.StartAsync(device.Id, key, tokenSource.Token);
            Status.Text = "Listening. Audio from the selected playback device is sent to Deepgram.";
        }
        catch (OperationCanceledException) { Status.Text = "Starting was cancelled."; await StopAsync(); }
        catch (Exception ex) { await StopAsync(); Status.Text = $"Could not start: {ex.Message}"; }
        finally { if (ReferenceEquals(startup, tokenSource)) startup = null; tokenSource.Dispose(); }
    }

    private async void Stop_Click(object sender, RoutedEventArgs e) => await StopAsync();
    private async Task StopAsync()
    {
        if (stopping) return;
        stopping = true;
        StopButton.IsEnabled = false;
        startup?.Cancel();
        var current = session; session = null;
        try { if (current is not null) await current.DisposeAsync(); }
        catch (Exception ex) { Status.Text = $"Error while stopping: {ex.Message}"; }
        finally { StartButton.IsEnabled = true; Devices.IsEnabled = true; stopping = false; }
    }
    private async void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (closing) return;
        e.Cancel = true; closing = true;
        await StopAsync(); Close();
    }
    private sealed record DeviceOption(string Id, string Name);
}
