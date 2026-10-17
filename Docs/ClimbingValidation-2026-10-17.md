# Physical climbing validation - 2026-10-17

- Pure logic: Scripts/Test-Logic.ps1 -> ALL PASSED.
- Unity 6000.0.85f1 compilation and fresh Play Mode: passed.
- TreeClimbValidation: PASS, including two simulated tracked hands moving downward,
  non-doubled body displacement, tracking-loss belay, unique nearby collection and trophy.
- Complete raw evidence: TreeClimbSmoke-2026-10-17.txt.
- Visual inspection: actual Unity view showed solid trunk and bark at player scale.
  Reduced gloss and reshaped knots after inspection.
- The test injects tracked-state fields; no controller or headset was connected.
- A complete manual climb, rope effort comparison, stereo, haptics, comfort and device
  performance have NOT been verified. Android build NOT RUN.
- Old terminal hot-reload null references were observed before stopping that runtime.
  Fresh climbing integration completed without an exception in its result.

These results establish a functioning code-level blockout, not VR production readiness.
