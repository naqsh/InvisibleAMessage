# Phase 1 development snapshot

The prototype provides a WPF shell, playback device selection, WASAPI loopback capture, bounded audio buffering, a Deepgram WebSocket client, and partial/final transcript display. The audio session and provider are separate infrastructure classes; additional dependency injection, recovery and resilience work is still needed. This is not yet a fully validated Phase 1.

## Run

1. Run `dotnet build src/MeetingCopilot.Desktop/MeetingCopilot.Desktop.csproj`.
2. In `src/MeetingCopilot.Desktop/bin/Debug/net10.0-windows/`, copy `appsettings.example.json` to `appsettings.json` and enter your Deepgram key locally. Never share the file.
3. Start the built application, select the playback device used for authorized meeting audio, and select Start listening. Select Stop when finished.

## Validation

- Application build: passed with zero warnings and errors.
- `dotnet test tests/MeetingCopilot.Tests/MeetingCopilot.Tests.csproj`: test project compiled, but Windows Application Control blocked loading the test assembly (0x800711C7); tests did not execute.
- Device enumeration, live capture, Deepgram authentication, transcript updates, device disconnect and shutdown: not yet manually validated.
- Only 16-bit PCM and 32-bit float playback formats are supported; format conversion and reconnection are not implemented.
- Primary and alternate LLM settings are placeholders; no LLM integration exists in Phase 1.
