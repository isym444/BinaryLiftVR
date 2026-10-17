# Adventure validation - current revision 2026-10-18

## Current results
Unity 6000.0.85f1 Windows Editor, Main scene. Scripts/Test-Logic.ps1: ALL PASSED.
Tests retain the exhaustive tree/layout/race coverage and add 40 adventure seeds,
phase guards, 1/2/4/8 spans, each real ancestor crossing, invalid pulls, partial edges,
no movement from selection, effort accounting and terminal completion.

Final method-driven Unity integration: PASS. See AdventureSmoke-2026-10-18.txt.
Checks classroom exit, seven waypoints, aerial/tree transition, original 35-node
structure, no idle climbing, all six crossings, cone, one ending and clean replay.
No RaceController exists at completion. Direct calls in this runner do not prove input.

Two real mouse-input journeys completed separately:
- First: 23:12-23:17 UTC, 4+2 selected levels, every level counted, separate cone
  pickup, result and replay. See AdventureManual-2026-10-18.log.
- Post-polish: after the automated runner returned to the classroom at 23:20:17 UTC,
  real clicks followed all seven checkpoints. Rounded grips were dragged through
  1+4+1 levels. A partial first edge stayed at zero completed levels while released;
  no additional movement occurred during the pause. Six levels, three connections,
  separate cone pickup at 23:25:53 UTC and clean replay at 23:26:10 UTC passed.
  See AdventureManual-Polished-2026-10-18.log; its initial reset is the automated
  runner's handoff, and all following gameplay actions are real mouse inputs.
  The only later gameplay-source change corrects singular LEVEL text; it does not
  change logic, input or layout. Unity compilation was checked again afterward.

Current evidence: Evidence/Adventure-Arrival-231243.png,
Adventure-Chase-232113.png, Adventure-Mapping-231534.png,
Adventure-Climbing-232505.png, Adventure-Clue-232548.png and
Adventure-Complete-232601.png. Earlier screenshots below are historical.
Evidence filenames use UTC times; the session date uses Europe/London (2026-10-18).

No headset was connected: stereo, physical reach, tracking loss, two-hand feel,
haptics, comfort and device performance remain NOT RUN. The 0.4 m HUD needs review.
Android modules/APK are unavailable. Desktop pull units are normalized mouse input,
not measured human motion. Procedural character art remains a placeholder and no
external/AI media or audio has been imported. Follow VRValidation.md on hardware.

---

# Historical validation - superseded adventure, 2026-10-17

The remainder describes the previous two-trial slice, not the current runtime.

## Scope
Unity 6000.0.85f1 Windows Editor, Main scene, desktop Game view. The adventure is
runtime-built and starts beside the existing independent race. All assets in this
slice are approved Unity primitives. No external or AI media was imported.

## Logic
Scripts/Test-Logic.ps1: ALL PASSED. Checks phase ordering, locked upgrades, invalid
pull rejection, two ordinary edges, unique cone collection, exact starting-node reset,
and a two-edge ancestor connection with lower accepted pull distance.

## Automated integration
BinaryLift > Validate Adventure follows the four timed walking checkpoints, verifies
map/tree transition, invokes ordinary and upgraded pulls, checks the horse trophy,
and validates completion. See AdventureSmoke-2026-10-17.txt. This runner calls gameplay
methods directly for climbing; it does not prove mouse input or VR tracking.

## Actual desktop input
A full mouse-input run reached completion at 22:25:36 UTC. The displayed result was
2 connections / 1.30 m pull before, 1 connection / 0.80 m after. A partial climb held
its position while released. Replay returned to arrival; Original Race opened its
ready screen. This exposed an unwanted post-drag selection, repaired by routing all
desktop pointer actions through one event path.

The post-repair input-only replay ran from 22:29:13 to 22:34:59 UTC and PASSED:
notebook theft, four manually selected walking checkpoints, tree arrival, both ordinary
climbs, separate cone pickup, faded return, two-level climb and comparison. The cone
remained visible and uncollected after landing until a separate click. A 54% upgraded
climb remained stationary between strokes. See AdventureManual-2026-10-17.log.
The automated runner had finished before this replay; no gameplay methods were called
directly during it. A subsequent presentation-only change hides the duplicate HUD
during Choosing, where the existing route sign supplies instructions.

Evidence in Docs/Evidence: Adventure-Arrival-221122.png, Adventure-Chase-220005.png,
Adventure-Clue-223350.png, Adventure-Climbing-223450.png and
Adventure-Complete-223517.png. Clue, climbing and completion images are from the final
manual run; arrival/chase images are earlier representative views from this session.

## Defects found and repaired during execution
- Main-thread-only MaterialPropertyBlock initialization prevented hover exceptions.
- Depth-tested world text prevents distant labels bleeding through architecture.
- The climbing lane was moved in front of the graph to avoid camera/platform overlap.
- Continuous arch tangents replaced incorrectly rotated radial blocks.
- End buttons were raised and the ordinary HUD hidden during comparison.
- Event-based dragging preserves strokes between frames, release and focus loss.
- Desktop selection uses the same event stream to prevent post-drag cone collection.
- The instruction panel sits in front of nearby handles at unchanged angular size.

## Hardware and art limits
No headset was connected for this validation. Physical reach, alternate/two-hand grips,
tracking loss, haptics, stereo rendering, comfort and device performance are NOT RUN.
The head-following panel's 0.4 m depth needs comfort review. Android modules/APK are
not available. Pull metres shown on desktop are normalized input units, not measured
human hand travel. Geometry and character animation remain development blockouts;
there are no imported character rigs, textures or audio assets. This does not establish
final-art quality or parity with a commercial third-person game.
