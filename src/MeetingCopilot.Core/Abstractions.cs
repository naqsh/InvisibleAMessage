namespace MeetingCopilot.Core;

public interface IStreamingSession : IAsyncDisposable
{
    event Action<string, bool>? Transcript;
    event Action<string>? Failed;
    Task StartAsync(string deviceId, string apiKey);
}
