using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace MeetingCopilot.Infrastructure;

internal sealed class DeepgramClient : IAsyncDisposable
{
    private readonly ClientWebSocket socket = new();
    private readonly CancellationTokenSource stop = new();
    private readonly Channel<byte[]> audio = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(100) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true });
    private Task? sender, receiver;
    public event Action<string, bool>? Transcript;
    public event Action<string>? Failed;
    public async Task ConnectAsync(string key, string encoding, int rate, int channels)
    {
        socket.Options.SetRequestHeader("Authorization", "Token " + key);
        await socket.ConnectAsync(new Uri($"wss://api.deepgram.com/v1/listen?model=nova-3&encoding={encoding}&sample_rate={rate}&channels={channels}&interim_results=true&endpointing=300"), stop.Token);
        sender = SendAsync(); receiver = ReceiveAsync();
    }
    public void Submit(ReadOnlySpan<byte> bytes) { if (!bytes.IsEmpty) audio.Writer.TryWrite(bytes.ToArray()); }
    private async Task SendAsync()
    {
        try
        {
            while (!stop.IsCancellationRequested)
            {
                using var wait = CancellationTokenSource.CreateLinkedTokenSource(stop.Token);
                wait.CancelAfter(TimeSpan.FromSeconds(5));
                try { var bytes = await audio.Reader.ReadAsync(wait.Token); await socket.SendAsync(bytes, WebSocketMessageType.Binary, true, stop.Token); }
                catch (OperationCanceledException) when (!stop.IsCancellationRequested) { await socket.SendAsync(Encoding.UTF8.GetBytes("{\"type\":\"KeepAlive\"}"), WebSocketMessageType.Text, true, stop.Token); }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { if (!stop.IsCancellationRequested) Failed?.Invoke("Transcription connection failed: " + ex.Message); }
    }
    private async Task ReceiveAsync()
    {
        var buffer = new byte[8192];
        try
        {
            while (!stop.IsCancellationRequested)
            {
                using var message = new MemoryStream(); WebSocketReceiveResult result;
                do { result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), stop.Token); if (result.MessageType == WebSocketMessageType.Close) { if (!stop.IsCancellationRequested) Failed?.Invoke("Transcription connection closed."); return; } message.Write(buffer, 0, result.Count); } while (!result.EndOfMessage);
                var transcript = TranscriptParser.Parse(message.ToArray());
                if (transcript is not null) Transcript?.Invoke(transcript.Value.Text, transcript.Value.IsFinal);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { if (!stop.IsCancellationRequested) Failed?.Invoke("Transcription receive failed: " + ex.Message); }
    }
    public async ValueTask DisposeAsync()
    {
        stop.Cancel(); audio.Writer.TryComplete();
        if (sender is not null) await sender;
        if (receiver is not null) await receiver;
        socket.Dispose(); stop.Dispose();
    }
}
