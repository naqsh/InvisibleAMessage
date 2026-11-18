---
trigger: always_on
---

# Meeting Copilot AI

Follow the authoritative product brief, C#/.NET 10/WPF technology stack, and mandatory staged workflow in `Agent.md`. The initial target is Windows; macOS is a later goal. Preserve a separation between platform-specific components and portable core logic where practical.

The user requests editable API credentials and endpoint settings in local `appsettings.json`, with AgentRouter as the initial OpenAI-compatible endpoint and a configurable alternate. Keep the local file out of source control. Verify provider compatibility in Stage 1.

Keep the implementation simple and focused, measure end-to-end performance, protect meeting data, and report actual validation. Do not claim untested functionality works.
