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
    public MainWindow()
    {
        InitializeComponent();
        try
        {
            using var enumerator = new MMDeviceEnumerator();
            Devices.ItemsSource = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                .Select(d => new DeviceOption(d.ID, d.FriendlyName)).ToList();
            Devices.SelectedIndex = 0;
        }
        catch (Exception ex) { Status.Text = $"Device enumeration failed: {ex.Message}"; }
    }
    private async void Start_Click(object sender, RoutedEventArgs e)
    {
        if (Devices.SelectedItem is not DeviceOption device) { Status.Text = "Select a playback device."; return; }
        StartButton.IsEnabled = false;
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) throw new InvalidOperationException("Create appsettings.json beside the application using appsettings.example.json and add your Deepgram key.");
            using var json = JsonDocument.Parse(await File.ReadAllTextAsync(path));
            var key = json.RootElement.GetProperty("Deepgram").GetProperty("ApiKey").GetString();
            if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Set Deepgram:ApiKey in appsettings.json.");
            session = new StreamingSession();
            session.Transcript += (text, final) => Dispatcher.BeginInvoke(() => { if (final) { Transcript.AppendText(text + Environment.NewLine); Transcript.ScrollToEnd(); } else Status.Text = "Listening: " + text; });
            session.Failed += message => Dispatcher.BeginInvoke(() => { Status.Text = message; StopButton.IsEnabled = true; });
            await session.StartAsync(device.Id, key);
            Status.Text = "Listening. Audio from the selected playback device is sent to Deepgram.";
            StopButton.IsEnabled = true;
        }
        catch (Exception ex) { Status.Text = $"Could not start: {ex.Message}"; if (session is not null) await session.DisposeAsync(); session = null; StartButton.IsEnabled = true; }
    }
    private async void Stop_Click(object sender, RoutedEventArgs e) => await StopAsync();
    private async Task StopAsync()
    {
        StopButton.IsEnabled = false;
        if (session is not null) await session.DisposeAsync();
        session = null; StartButton.IsEnabled = true; Status.Text = "Stopped.";
    }
    private async void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (session is null) return;
        e.Cancel = true; await StopAsync(); Close();
    }
    private sealed record DeviceOption(string Id, string Name);
}


