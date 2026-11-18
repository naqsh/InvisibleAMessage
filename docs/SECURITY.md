# Configuration and security

The user requests editable credentials and API settings in a local `appsettings.json`, including an AgentRouter primary endpoint using an OpenAI-compatible API and a configurable alternate endpoint. Exact endpoint, model, streaming compatibility and fallback behavior remain to be verified during Stage 1.

Keep real credentials only in the local configuration file and exclude it from source control. Supply an example with empty placeholders when implementation begins. Restrict file access where practical, never log keys, and recognize that plaintext configuration is readable by processes and users with access to the file. No credentials have been created here.

Capture and transcription must be explicitly enabled and authorized. Avoid retaining audio or transcripts permanently by default.
