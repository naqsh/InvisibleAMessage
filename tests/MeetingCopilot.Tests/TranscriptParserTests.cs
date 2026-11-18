using System.Text;
using MeetingCopilot.Infrastructure;

namespace MeetingCopilot.Tests;

public class TranscriptParserTests
{
    [Fact]
    public void InterimTranscriptIsIdentified()
    {
        var result = TranscriptParser.Parse(Encoding.UTF8.GetBytes("{\"is_final\":false,\"channel\":{\"alternatives\":[{\"transcript\":\"Hello\"}]}}"));
        Assert.NotNull(result); Assert.Equal("Hello", result.Value.Text); Assert.False(result.Value.IsFinal);
    }
    [Fact]
    public void FinalTranscriptIsIdentified()
    {
        var result = TranscriptParser.Parse(Encoding.UTF8.GetBytes("{\"is_final\":true,\"channel\":{\"alternatives\":[{\"transcript\":\"Hello world\"}]}}"));
        Assert.NotNull(result); Assert.Equal("Hello world", result.Value.Text); Assert.True(result.Value.IsFinal);
    }
    [Theory]
    [InlineData("{\"type\":\"Metadata\"}")]
    [InlineData("{\"channel\":{\"alternatives\":[]}}")]
    [InlineData("{\"channel\":{\"alternatives\":[{\"transcript\":\"\"}]}}")]
    public void IgnoresMessagesWithoutTranscript(string message) => Assert.Null(TranscriptParser.Parse(Encoding.UTF8.GetBytes(message)));
}
