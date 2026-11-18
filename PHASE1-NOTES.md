# Phase 1 validation

The application builds with .NET SDK 10.0.301: `dotnet build src/MeetingCopilot.Desktop/MeetingCopilot.Desktop.csproj` succeeded with no warnings or errors on 18 November 2026. Restore completed for all three application projects.

`dotnet test tests/MeetingCopilot.Tests/MeetingCopilot.Tests.csproj` restored and built the test project, but Windows Application Control blocked the test assembly from loading (0x800711C7). No tests ran. The policy must be resolved by the computer's administrator; do not disable or bypass it as a workaround.

## Manual checks still required

1. Set up `appsettings.json` as described in README.md. Put the Deepgram API key only in this local file.
2. Start the application and confirm that connected playback devices appear. Choose the device used by the authorized audio source.
3. Play authorized speech and select Start listening. Confirm that the connection succeeds and interim and final text appear. Stop playback briefly and confirm listening resumes when speech returns.
4. Select Stop while listening and while connecting. Confirm the application remains responsive and can start a fresh session. Close the window while connecting and while listening.
5. With permission to test the device, disconnect the selected output device while listening. Confirm the application reports an error and capture stops; reconnect it and restart the app to refresh the device list.
6. Temporarily use an invalid key and then restore the correct local value. Confirm an actionable authentication error appears. Disconnect and restore the network during a session and confirm the application stops and offers a manual retry.

These checks have not been executed. No live Deepgram key or authorized meeting audio was available during development. Do not report Phase 1 as validated until they and the automated tests pass.

## Known limitations

Only 16-bit PCM and 32-bit floating-point playback formats are accepted. Audio conversion, automatic connection recovery, device list refresh, dependency injection, and full MVVM separation are not implemented. The bounded buffer drops oldest frames under overload. LLM settings are placeholders and are not used in Phase 1.
