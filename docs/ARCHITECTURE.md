# Architecture

The authoritative product brief and staged workflow are in [Agent.md](../Agent.md). Stage 1 architecture and compatibility decisions are pending.

The initial target is Windows 10/11 with C#, .NET 10 and WPF. Keep platform-specific capture and UI separate from portable domain and provider logic where practical; macOS testing is a later goal and WPF/WASAPI require replacement there.

The intended flow is authorized audio capture → streaming transcription → question detection and context selection → streaming LLM → desktop answer display.
