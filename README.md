# Invisible Messages

Invisible Messages is an early Windows desktop meeting-copilot prototype. Its first phase captures authorized audio from a selected Windows playback device and streams it to Deepgram for live transcription. Future phases are intended to detect questions and provide AI-suggested answers.

## Current status

The prototype includes a WPF window, playback-device selection, WASAPI loopback capture, bounded audio buffering, a Deepgram streaming client, and partial and final transcript display. The application builds, but live audio capture and Deepgram transcription have not yet been validated. AI answers, AgentRouter integration, floating overlay, automatic reconnection, and audio format conversion are not implemented.

Only 16-bit PCM and 32-bit floating-point playback formats are currently supported. See [Phase 1 notes](PHASE1-NOTES.md) for the latest validation results and limitations.

## Requirements

- Windows 10 or 11 and the .NET 10 SDK with Windows desktop support.
- An audio output device playing the meeting audio.
- A Deepgram API key and an internet connection.

Capture and transcribe meeting audio only when authorized and in accordance with applicable consent requirements. Loopback captures audio played through the selected device, which may include sounds outside the meeting.

## Build and run

From the repository root in PowerShell:

```powershell
dotnet restore src/MeetingCopilot.Desktop/MeetingCopilot.Desktop.csproj
dotnet build src/MeetingCopilot.Desktop/MeetingCopilot.Desktop.csproj
```

Create your local configuration from the example in the build output:

```powershell
Copy-Item src/MeetingCopilot.Desktop/bin/Debug/net10.0-windows/appsettings.example.json src/MeetingCopilot.Desktop/bin/Debug/net10.0-windows/appsettings.json
```

Open the new `appsettings.json` and set `Deepgram.ApiKey` locally. The file contains a secret in plaintext: keep it private. It is excluded by `.gitignore`. The LLM settings are placeholders for a later phase.

Start the application:

```powershell
dotnet run --project src/MeetingCopilot.Desktop/MeetingCopilot.Desktop.csproj
```

Select the playback device, select **Start listening**, and play authorized speech through that device. Select **Stop** to end capture.

## Tests

```powershell
dotnet test tests/MeetingCopilot.Tests/MeetingCopilot.Tests.csproj
```

On the development machine, Windows Application Control blocked the test assembly from loading, so the tests have not yet been executed successfully. See [Phase 1 notes](PHASE1-NOTES.md) for details.

## Project layout

- `src/MeetingCopilot.Core` — core session interface.
- `src/MeetingCopilot.Infrastructure` — audio capture and Deepgram streaming prototype.
- `src/MeetingCopilot.Desktop` — WPF application and example configuration.
- `tests/MeetingCopilot.Tests` — transcript parsing tests.
- `docs/` — architecture, plan, and project guidance.

See [Agent.md](Agent.md) for the product brief and planned phases. The initial implementation targets Windows; macOS support would require alternatives to WPF and WASAPI.
