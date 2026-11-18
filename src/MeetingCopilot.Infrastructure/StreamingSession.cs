using NAudio.CoreAudioApi;
using NAudio.Wave;
using MeetingCopilot.Core;

namespace MeetingCopilot.Infrastructure;

public sealed class StreamingSession : IStreamingSession
{
    private readonly DeepgramClient provider = new();
    private WasapiLoopbackCapture? capture;
    public event Action<string, bool>? Transcript;
    public event Action<string>? Failed;
    public async Task StartAsync(string deviceId, string apiKey)
    {
        provider.Transcript += (text, final) => Transcript?.Invoke(text, final);
        provider.Failed += error => Failed?.Invoke(error);
        using var enumerator = new MMDeviceEnumerator();
        var device = enumerator.GetDevice(deviceId);
        capture = new WasapiLoopbackCapture(device);
        var format = capture.WaveFormat;
        var floatFormat = format.Encoding == WaveFormatEncoding.IeeeFloat || (format is WaveFormatExtensible ext && ext.SubFormat == new Guid("00000003-0000-0010-8000-00aa00389b71"));
        var pcmFormat = format.Encoding == WaveFormatEncoding.Pcm || (format is WaveFormatExtensible ext2 && ext2.SubFormat == new Guid("00000001-0000-0010-8000-00aa00389b71"));
        if (!(floatFormat && format.BitsPerSample == 32) && !(pcmFormat && format.BitsPerSample == 16)) throw new InvalidOperationException($"Unsupported playback format: {format}. Use 16-bit PCM or 32-bit float in Windows sound settings.");
        await provider.ConnectAsync(apiKey, floatFormat ? "linear32" : "linear16", format.SampleRate, format.Channels);
        capture.DataAvailable += (_, e) => provider.Submit(e.Buffer.AsSpan(0, e.BytesRecorded));
        capture.RecordingStopped += (_, e) => { if (e.Exception is not null) Failed?.Invoke("Audio capture stopped: " + e.Exception.Message); };
        capture.StartRecording();
    }
    public async ValueTask DisposeAsync()
    {
        capture?.StopRecording(); capture?.Dispose();
        await provider.DisposeAsync();
    }
}
