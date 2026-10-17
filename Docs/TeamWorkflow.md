# Team workflow

## Required reading
Read README.md, Docs/Handoff.md, Docs/Design.md, and the latest entry in
Docs/DevelopmentLog.md before editing. Inspect git status and preserve other people's work.

## Language and assets
- Use English for code identifiers, comments, logs, filenames, UI, documentation,
  commit messages, and pull request descriptions.
- Do not use AI-generated media. Primitive blockouts have been explicitly approved.
- Use only free external assets after the user approves the specific source and preview.
- Record asset provenance, license, cost, approval, and intended use in Docs/AssetRegister.md.
- Never treat a reference photo or a free license as permission to import an asset.

## Every contribution
1. Record scope, assumptions, baseline changes, and intended files in DevelopmentLog.md.
2. Keep changes focused. Coordinate file ownership before parallel edits.
3. Add meaningful tests for progression, algorithm, and input-state changes.
4. Record commands, actual results, and limits of validation. Label untested hardware clearly.
5. Update Handoff.md with current state, known issues, and exact next actions.
6. Update README.md if launch, controls, dependencies, or user-facing behaviour changes.
7. Submit a reviewable diff. Do not push, merge, contact teammates, or modify their work
   without the relevant user authorization.

## Suggested work areas
- Gameplay: AdventureState, AdventureController and AdventureWorld; coordinate progression changes.
- Environment: CampusWorld; replace named placeholders without changing target contracts.
- XR: XRRig, comfort settings, hardware checks and input bindings.
- QA: pure logic tests, desktop smoke checks, and the headset checklist.
- Art/audio: approved assets only, with provenance and performance budgets.

## Handoff template
- Objective and accepted constraints
- Changed files and public interfaces
- Verification performed, commands, outcomes
- Known issues, risks, and untested hardware
- Next concrete steps and suggested owner
- Asset approvals still pending

Runtime telemetry must remain local by default. Do not record names, account details,
headset identifiers, head/hand pose streams, or transmit logs to a service.

## Authorized GitHub destination
The user authorized uploads of this project to https://github.com/isym444/BinaryLiftVR.git,
branch Fexas, beginning with Session 06. Verify origin and the current branch before
every upload. Use a normal push to Fexas; never main, another branch, or force-push.
Review staged files for caches, personal settings and credentials before committing.
Sanitized project validation evidence may accompany the requested project upload.
This authorization does not authorize contacting teammates or changing repository access.
