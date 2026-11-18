using System.Text.Json;
namespace MeetingCopilot.Infrastructure;

public static class TranscriptParser
{
    public static (string Text, bool IsFinal)? Parse(ReadOnlySpan<byte> message)
    {
        using var json = JsonDocument.Parse(message.ToArray()); var root = json.RootElement;
        if (!root.TryGetProperty("channel", out var channel) || !channel.TryGetProperty("alternatives", out var alternatives) || alternatives.GetArrayLength() == 0) return null;
        var text = alternatives[0].GetProperty("transcript").GetString();
        if (string.IsNullOrWhiteSpace(text)) return null;
        return (text, root.TryGetProperty("is_final", out var final) && final.GetBoolean());
    }
}
