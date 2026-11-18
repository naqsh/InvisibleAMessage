# MASTER PROMPT — Real-Time AI Meeting Copilot for Windows

## Role

Act as a team of senior software engineers specializing in:

1. C# / .NET 10 desktop development
2. Windows WASAPI audio capture
3. Real-time audio streaming and speech recognition
4. Low-latency LLM inference
5. WPF desktop UI and MVVM architecture
6. Windows-operating-system integration
7. Application security and privacy
8. Performance profiling and optimization

Your task is to architect and implement a production-ready Windows desktop AI meeting assistant.

Primary objective: Generate useful AI-suggested answers to spoken questions with the lowest practical end-to-end latency.

Do not stop at theoretical architecture. Produce working, testable source code incrementally, using the staged response process defined below.

## Mandatory Staged Response Process

Follow this sequence exactly. Do not skip ahead, combine stages, or provide later-phase implementation before the preceding stage has been completed and validated.

### Stage 1 — Architecture and Compatibility Decisions

First provide only the architecture and compatibility decisions required to begin implementation. Include:

1. Final architecture recommendation.
2. Technology compatibility assessment for Windows 10/11, .NET 10, WPF, NAudio, Deepgram, SQLite, DPAPI or Credential Manager, dependency injection, logging, and xUnit.
3. Exact package and version recommendations, including known compatibility risks.
4. Visual Studio solution and project structure.
5. Audio pipeline design.
6. Streaming transcription design.
7. Question-detection and context-management design.
8. LLM streaming design.
9. UI and MVVM design.
10. Security, privacy, consent, and data-retention decisions.
11. Latency optimization and measurement strategy.
12. Phase-by-phase implementation plan.
13. Explicit assumptions, unresolved decisions, and risks.

At the end of Stage 1, stop and wait for approval or requested changes. Do not provide implementation code yet.

### Stage 2 — Minimal Compilable Phase 1 Repository

After Stage 1 is approved, produce a minimal but complete Phase 1 repository that compiles successfully.

Phase 1 must contain only the smallest reliable implementation needed for:

- A WPF application shell.
- Audio-device enumeration and selection.
- WASAPI loopback capture.
- Bounded asynchronous audio buffering.
- A Deepgram streaming connection or a clearly isolated provider implementation with real API calls.
- Partial and finalized transcript display.
- Cancellation and graceful shutdown.
- Configuration through safe placeholders or local settings, without embedding credentials.
- Dependency injection and logging where required for the implementation.
- Basic error reporting.

Provide:

1. Complete repository tree.
2. Every required source file in full.
3. Project files and package references.
4. Configuration files and safe example configuration.
5. Build and run instructions.
6. Manual test instructions.
7. Any required API setup steps.
8. Explicit statements identifying code that has not been executed or externally validated.

Do not implement Phase 2, Phase 3, Phase 4, or Phase 5 features unless they are strictly required for the Phase 1 repository to compile and run.

The Phase 1 repository must be internally consistent: namespaces, project references, package versions, APIs, configuration keys, and startup behavior must match. Do not present pseudocode, placeholders disguised as working code, invented APIs, or incomplete files as a complete repository.

### Stage 3 — Phase 1 Validation Results

After providing the Phase 1 repository, perform and report validation before proposing or implementing subsequent phases.

Validation must include, as applicable:

1. `dotnet restore` result.
2. `dotnet build` result.
3. Test-project build and test results.
4. Static consistency checks across project references, namespaces, package versions, and configuration.
5. Manual validation procedure for audio-device enumeration.
6. Manual validation procedure for WASAPI capture.
7. Manual validation procedure for Deepgram connectivity and transcript updates.
8. Cancellation, device-disconnection, and shutdown checks.
9. Known failures, unverified assumptions, and environment-specific limitations.

Do not claim that code compiles, runs, or works unless it has actually been validated. If execution is unavailable, clearly label results as “not executed” and provide exact commands the user can run.

Stop after reporting Phase 1 validation results. Wait for approval before proceeding.

### Stage 4 — Subsequent Phases

Only after Phase 1 validation has been reported and approved, proceed one phase at a time.

For each subsequent phase:

1. State the phase objective and scope.
2. Identify architecture or compatibility changes.
3. Provide only the files added or changed, unless complete files are necessary for clarity.
4. Provide build and test instructions.
5. Validate the phase using the same standards as Stage 3.
6. Report results, limitations, and regressions.
7. Stop and wait for approval before beginning the next phase.

Use this order:

- Phase 2 — AI Suggested Answers
- Phase 3 — Floating UI
- Phase 4 — Optimization
- Phase 5 — Production Hardening

Do not begin a later phase while an earlier phase has unresolved build failures, unreported validation results, or unapproved architectural changes.

## 1. Product Overview

Product Name: Meeting Copilot AI

Operating System: Windows 10/11

Technology Stack:

| Layer                    | Technology                               |
| ------------------------ | ---------------------------------------- |
| Language                 | C#                                       |
| Runtime                  | .NET 10                                  |
| Desktop UI               | WPF                                      |
| UI architecture          | MVVM                                     |
| Audio capture            | WASAPI loopback                          |
| Audio library            | NAudio                                   |
| Voice activity detection | WebRTC VAD or Silero VAD                 |
| Speech-to-text           | Deepgram streaming API                   |
| Alternative STT          | Azure AI Speech                          |
| LLM                      | Configurable low-latency cloud LLM       |
| LLM providers            | OpenAI, Gemini, Claude                   |
| Networking               | Async HTTP streaming / WebSocket         |
| Local configuration      | SQLite                                   |
| Secrets                  | Windows DPAPI or Credential Manager      |
| Dependency injection     | Microsoft.Extensions.DependencyInjection |
| Logging                  | Microsoft.Extensions.Logging             |
| Testing                  | xUnit                                    |

Verify library compatibility with .NET 10 before choosing exact versions.

Avoid Electron, embedded browsers, unnecessary microservices, and separate backend servers for the MVP.

## 2. Core User Workflow

The user launches Meeting Copilot AI before joining an authorized meeting.

The application must:

1. Detect available Windows audio devices.
2. Allow selection of the meeting playback device.
3. Capture meeting audio with WASAPI loopback.
4. Stream audio continuously to Deepgram.
5. Display partial and finalized transcription.
6. Detect spoken questions automatically.
7. Maintain recent conversation context.
8. Send relevant questions to the selected LLM.
9. Display suggested answers as tokens arrive.
10. Allow users to copy, regenerate, or expand answers.

Support Microsoft Teams, Google Meet, Zoom, and other applications that output audio through compatible Windows devices.

Do not depend on browser extensions or meeting-platform APIs for basic audio capture.

Audio capture must be explicitly enabled, and recording or transcription must comply with applicable consent requirements.

## 3. Audio Capture Engine

Implement an independent audio capture service.

### Required Features

- WASAPI loopback capture using NAudio.
- Configurable playback device.
- Optional microphone capture.
- Real-time PCM processing.
- Automatic audio format conversion.
- Audio resampling for STT requirements.
- Bounded audio buffers.
- Device-disconnection handling.
- Automatic reconnection where safe.
- CancellationToken support.
- Graceful start, pause, resume, and stop.

Use Channel\<T> or an equivalent bounded producer-consumer pipeline.

Never block the audio callback with network requests or LLM inference.

### Audio Pipeline

Audio Device
→ WASAPI Capture
→ Audio Format Conversion
→ Voice Activity Detection
→ Streaming STT Provider

Evaluate whether VAD reduces costs without harming recognition of short questions or quiet speech.

## 4. Deepgram Streaming Integration

Implement a dedicated Deepgram transcription provider.

Requirements:

- Persistent WebSocket connection.
- Streaming audio frames.
- Partial transcription events.
- Final transcription events.
- Timestamp support.
- Endpointing configuration.
- Keepalive handling.
- Connection recovery.
- Error handling.
- Transcript deduplication.

Do not wait for a complete sentence before updating the UI.

Separate interim transcript state from confirmed transcript history.

Use provider configuration that balances accuracy with latency.

Provide an Azure AI Speech adapter as a future alternative.

## 5. Intelligent Question Detection

Create a lightweight question detection service.

It should identify:

- Direct questions
- Technical interview-style questions
- Requests for explanations
- Requests for examples
- Follow-up questions
- Requests for recommendations

Use a two-stage approach:

Stage A: Fast detection

Use lightweight rules and transcript structure to identify potential questions.

Stage B: Context-aware confirmation

Use a lightweight classifier or LLM only when the question is ambiguous.

Do not send every transcript update to an LLM.

Avoid generating duplicate answers for repeated partial transcripts.

Allow manual generation through a configurable keyboard shortcut.

## 6. LLM Integration

Build an abstraction:

ILLMProvider

Implement provider adapters for:

- OpenAI
- Google Gemini
- Anthropic Claude
- OpenAI-compatible endpoints

Support:

- Model selection
- API key configuration
- Streaming responses
- Cancellation
- Request timeouts
- Token usage monitoring
- Error recovery
- Context window management

### Response Modes

Quick Mode

Generate a concise answer suitable for immediate reference.

Target approximately 2–4 sentences.

Technical Mode

Generate structured technical explanations, code snippets when relevant, and practical examples.

Detailed Mode

Generate more complete explanations with important context and tradeoffs.

### System Prompt for the LLM

You are an AI meeting assistant.

Analyze the detected question and relevant conversation context.

Generate an accurate, concise, natural-language suggested answer.

Rules:

1. Answer the actual question.
2. Prioritize correctness and clarity.
3. Avoid unnecessary introductions.
4. Keep Quick Mode concise.
5. Use technical terminology when appropriate.
6. Do not invent facts.
7. State uncertainty when needed.
8. Avoid repeating the question.
9. Use relevant prior conversation context.
10. Produce the answer immediately without unnecessary formatting.

## 7. Ultra-Low-Latency Architecture

Use the following pipeline:

WASAPI Audio Capture
→ Streaming Deepgram STT
→ Incremental Transcript Processing
→ Question Detection
→ Context Selection
→ Streaming LLM Inference
→ WPF Floating Answer Panel

### Optimization Requirements

- Use asynchronous processing throughout.
- Maintain persistent STT connections.
- Reuse HTTP clients.
- Stream LLM tokens immediately.
- Avoid unnecessary serialization.
- Minimize intermediate allocations.
- Use bounded channels.
- Cache stable system prompts where supported.
- Debounce unstable partial transcripts.
- Cancel obsolete inference requests.
- Avoid blocking the UI thread.
- Batch UI updates when appropriate.

### Latency Measurement

Instrument:

1. Audio capture latency
2. STT partial-result latency
3. Question detection latency
4. LLM request startup
5. Time to first token
6. Time to first useful answer
7. Total response completion time

Use monotonic timestamps where possible.

Report p50, p95, and p99 latency from realistic tests.

Treat a 1–2 second first useful answer as an aspirational target, not a guaranteed result.

## 8. Floating WPF Assistant

Create a compact, lightweight floating window.

### UI Features

- Always-on-top option
- Resizable window
- Adjustable opacity
- Collapsible answer panel
- Live suggested-answer streaming
- Question preview
- Copy answer
- Regenerate answer
- Pause listening
- Model selector
- Connection status

Provide a full application window for settings and transcript review.

### Screen-Sharing Privacy Mode

Investigate Windows SetWindowDisplayAffinity with WDA_EXCLUDEFROMCAPTURE.

Implement:

- User-controlled capture exclusion where supported
- Compatibility detection
- Clear warnings when exclusion cannot be guaranteed
- A screen-sharing test workflow
- A recommended application-window-sharing alternative

Do not claim the assistant can remain invisible to every screen recorder, monitoring tool, or meeting application.

Do not bypass organizational security controls.

## 9. Keyboard Shortcuts

Provide configurable global shortcuts:

| Action                 | Default        |
| ---------------------- | -------------- |
| Start / Stop Listening | Ctrl + Alt + L |
| Generate Answer        | Ctrl + Alt + A |
| Toggle Floating Panel  | Ctrl + Alt + M |
| Switch Answer Mode     | Ctrl + Alt + Q |
| Copy Last Answer       | Ctrl + Alt + C |

Check for conflicts and allow customization.

## 10. Application Architecture

Create a Visual Studio solution with the following projects:

MeetingCopilot.Desktop

MeetingCopilot.Core

MeetingCopilot.Audio

MeetingCopilot.Transcription

MeetingCopilot.QuestionDetection

MeetingCopilot.LLM

MeetingCopilot.Overlay

MeetingCopilot.Infrastructure

MeetingCopilot.Tests

Use dependency injection and clearly defined interfaces.

Suggested interfaces:

- IAudioCaptureService
- IAudioProcessingService
- ITranscriptionProvider
- IQuestionDetectionService
- ILLMProvider
- IConversationContextService
- IOverlayService
- ISettingsService

Use MVVM for UI separation.

Avoid unnecessary abstraction layers that increase complexity without improving testability.

## 11. Conversation Context Management

Maintain a rolling transcript buffer.

Requirements:

- Preserve recent relevant dialogue.
- Identify repeated questions.
- Handle follow-up questions.
- Remove stale context.
- Control token usage.
- Avoid transmitting unrelated conversation history.
- Support clearing session data.

Do not retain audio or transcripts permanently by default.

## 12. Settings and Configuration

Allow users to configure:

- Audio playback device
- Microphone
- STT provider
- STT language
- LLM provider
- LLM model
- API keys
- Answer mode
- Auto-detection on/off
- Floating panel opacity
- Always-on-top behavior
- Capture-exclusion preference
- Keyboard shortcuts
- Transcript retention

Use secure storage for credentials.

Never log raw API keys.

## 13. Reliability and Error Handling

Handle:

- Audio device changes
- No available audio device
- STT WebSocket disconnection
- Network interruption
- API authentication failure
- LLM rate limits
- Empty transcripts
- Duplicate transcription events
- Overlapping LLM requests
- Application shutdown during streaming

Provide actionable user-facing errors.

Avoid silent failures.

## 14. Implementation Phases

### Phase 1 — Working Audio and STT

Build:

- WPF application shell
- Audio device selector
- WASAPI capture
- Deepgram streaming connection
- Live transcript display

Deliver a minimal compilable repository, then report validation results before any subsequent phase.

### Phase 2 — AI Suggested Answers

Implement:

- Question detection
- Conversation context
- LLM provider abstraction
- One working cloud LLM integration
- Streaming answer display
- Manual and automatic modes

Implement only after Phase 1 has been validated and approved.

### Phase 3 — Floating UI

Implement:

- Compact floating window
- Always-on-top behavior
- Keyboard shortcuts
- Answer streaming
- Capture-exclusion compatibility testing

Implement only after Phase 2 has been validated and approved.

### Phase 4 — Optimization

Implement:

- Latency instrumentation
- Question deduplication
- Connection recovery
- Provider benchmarks
- Memory profiling
- CPU profiling
- Long-session stability testing

Implement only after Phase 3 has been validated and approved.

### Phase 5 — Production Hardening

Implement:

- Secure settings
- Installer packaging
- Automated tests
- User documentation
- Crash recovery
- Privacy controls

Implement only after Phase 4 has been validated and approved.

## 15. Engineering Quality Requirements

All implementation must:

- Compile successfully.
- Use real APIs, not invented methods.
- Include necessary NuGet dependencies.
- Handle cancellation correctly.
- Avoid blocking UI and audio threads.
- Support dependency injection.
- Follow modern C# conventions.
- Include meaningful error handling.
- Be testable without a live meeting.
- Keep API credentials out of source code.

Do not present pseudocode as a complete implementation.

Do not claim a feature works without testing it.

When execution or external service access is unavailable, explicitly distinguish between code inspection, static reasoning, and actual build or runtime validation.

## 16. Required Output

Begin with Stage 1 only:

1. Final architecture recommendation.
2. Technology compatibility assessment.
3. Visual Studio solution structure.
4. NuGet dependency list.
5. Audio pipeline design.
6. Streaming transcription design.
7. LLM streaming design.
8. Latency optimization strategy.
9. MVP implementation milestones.
10. Explicit compatibility risks and unresolved decisions.

Stop after Stage 1 and wait for approval.

After approval, provide the minimal compilable Phase 1 repository. Then stop and report validation results. Only after Phase 1 validation and approval may you proceed to Phase 2, and continue in the same staged manner for each later phase.

For every implementation phase:

- Provide complete source files for the requested scope.
- Explain where files belong.
- Provide build instructions.
- Provide test instructions.
- Report actual validation results.
- Identify remaining limitations.
- Stop for approval before beginning the next phase.

Final Priority Order:

1. Fast first useful answer
2. Accurate transcription
3. Reliable audio capture
4. Responsive desktop interface
5. Stable long-running operation
6. Privacy and security
7. Low operating costs
8. Extensibility

Build the simplest reliable MVP first. Do not overengineer.
