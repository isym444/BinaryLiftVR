# Project instructions for contributors

Applies to this entire repository.

- Read README.md, Docs/Handoff.md, Docs/TeamWorkflow.md and the latest DevelopmentLog
  entry before changing code. Inspect git status and preserve existing work.
- Write all game text, identifiers, comments, logs, filenames, documentation, commits
  and PR descriptions in English. Conversation with the user may use their language.
- The user permits a small amount of AI-generated media as of 2026-10-18.
  Label generated candidates and obtain approval of their preview before import.
  This replaces the earlier blanket prohibition; it is not approval of any specific asset.
  Approved Unity primitive blockouts remain allowed.
- External assets must be free and approved by the user after source, preview, licence,
  and proposed usage are presented. Maintain Docs/AssetRegister.md. The user's statue
  photo is reference-only; do not add it to the game.
- Keep the University of Glasgow setting, first-person player, horse companion,
  cone collection, and binary-lifting progression unless the user changes direction.
- Record meaningful work steps, decisions, changed files and validation in
  Docs/DevelopmentLog.md. Update Docs/Handoff.md and README.md as relevant before finishing.
- Keep algorithm/progression rules in Assets/Scripts/Logic and independent of Unity.
- Retain source .meta files and do not commit generated caches, installers, or account data.
- Run meaningful logic tests and Unity integration checks after gameplay changes.
  Scripts/Test-Logic.ps1 supports Unity's Mono compiler when a .NET SDK is unavailable.
- Never equate desktop Play Mode with a headset test. Record untested hardware and
  use Docs/VRValidation.md before claiming VR readiness or on-device performance.
- Coordinate file ownership before concurrent edits; do not reset others' changes.
- Do not contact teammates or publish/push changes without user authorization.
