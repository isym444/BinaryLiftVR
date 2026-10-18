# Three-cycle validation - current revision 2026-10-18

## Current revision evidence
All three optimization cycles are complete. Scripts/Test-Logic.ps1: ALL PASSED,
including forty seeds, varied target distances, optional quizzes, unique stamps,
research prerequisites/cancellation/exhaustion and decision benchmarks checked against
exhaustive routes. The original tree and physical effort contracts remain covered.
Cycle1/2/3-Smoke-2026-10-18.txt each records a passing Unity integration run.
Cycle 3 final run passed at 01:07 UTC after correcting the results heading margin.
These tests invoke gameplay methods; they are separate from actual input below.

ThreeCyclesManual-2026-10-18.log isolates the actual mouse walkthrough after the
integration runner's clean classroom reset at 01:07:43 UTC:

1. Exit classroom, trigger theft, click seven pursuit beacons and watch the map/tree
   transition. Original tree entered at 01:09:40.
2. Manually climb four individual edges, collecting two cones. Drag the orange plug
   to cyan to build both twos and compose four at 01:11:09.
3. Follow the funding; physically climb six levels using 4+1+1. Recover cone three at
   01:12:27. Results show three decisions and a memory-best benchmark of three.
4. Enter research, cancel, reopen, and build node 10 -> 9 -> 8 at 01:12:52. Then
   compose node 12 -> 10 -> 8 into four at 01:13:16. Five connections are retained;
   the completed mission's score stays unchanged.
5. Start the three-level Coffee Contingency target. Four is unavailable. Select two,
   manually traverse both edges, then select/climb one. Cone four recovered 01:14:00.
6. Open the remaining-distance quiz. Deliberately choose four, read the explanation,
   retry two, receive one stamp and return to the same results.
7. Replay Classroom at 01:14:47: cones, stamps, memory and counters reset. No second
   game starts. Unity was stopped after validation to avoid idle rendering.

Screenshots: Evidence/Adventure-Climbing-011146.png (live 3-of-4 progression),
Adventure-Weaving-011308.png (additional four-level research),
Adventure-Complete-011439.png (second recovery with five connections and one stamp).
Cycle 1/2 focused input evidence is detailed in ImprovementCycles-2026-10-18.md.

No headset is connected. Stereo, physical reach, comfort, haptics, target-device
performance and APK validation remain NOT RUN. This remains a stylized prototype.

## Previous learning-loop revision evidence
The records below predate the three-cycle update and are retained as history.

## Scope and results
Unity 6000.0.85f1 Windows Editor, Main scene. Scripts/Test-Logic.ps1: ALL PASSED.
Coverage includes 40 adventure seeds, unchanged tree topology, source-node-specific
memory, two/four-edge composition prerequisites, partial/rest/invalid pulls, equal
physical effort per edge, target changes, unique rewards and clean reset.

Unity method-driven integration: PASS (LearningLoopSmoke-2026-10-18.txt). It checks
the full exploration/composition/reuse cycle, approved art loading, original 35 nodes,
no RaceController, classroom reset and new-target text viewport margins.
This runner calls gameplay methods directly; real input evidence is separate.

## Actual desktop mouse walkthrough
LearningLoopManual-2026-10-18.log contains the isolated manual run after the automated
runner returned to the classroom at 00:11:07 UTC. All subsequent gameplay used real
mouse clicks and drags, with fresh screenshots between actions.

1. Exit classroom, trigger funding theft, follow seven campus markers and map reveal.
2. Enter the original tree at 00:14:13. Select single levels. A short first pull and
   released pause keep the counter at zero; a later pull completes that edge.
3. At level two collect cone one separately, then drag the plug to the socket:
   node 23 -> 13 -> 12 becomes a two-level connection.
4. Climb two more individual edges, collect cone two, build 12 -> 11 -> 10, then
   combine 23 -> 12 -> 10 into four levels at 00:17:53.
5. Follow the funding to a six-level target. Select four, manually pull through all
   four crossings, then select/climb one plus one. The HUD counts every crossed node.
6. Collect cone three at 00:19:17. Results: six levels, three route decisions, one
   remembered connection reused. No other game starts.
7. Select New Target / Keep Memory. The target changes to five levels. Use 2+2+1,
   manually climb every edge, and collect cone four at 00:20:29. Lifetime totals:
   fifteen levels and three reused connections. Results stay in the same adventure.
8. Replay from Classroom at 00:20:40 clears cones, climb count and route memory.

## Presentation fixes and evidence
The first attempt exposed clipped workshop instructions and platform occlusion.
Panels were moved to a compact foreground dock and the recovery balcony now follows
the active goal. The full manual walkthrough above used these fixes. After that run,
the new-target panel alone moved down 0.08 world units to give its heading more margin;
the final integration adds a projected-text bounds check. Input and progression did
not change after the successful manual run.

Current screenshots in Evidence/:
- Adventure-Arrival-000315.png: approved sandstone, furniture and poster.
- Adventure-Weaving-001743.png: real four-level composition workshop.
- Adventure-Climbing-001833.png: live second crossing of a selected four-level route.
- Adventure-Complete-001930.png: first recovered funding result.
- Adventure-Complete-002033.png: repeat search result with four cones.

No headset was connected. Stereo, physical reach, controller tracking, haptics,
comfort and on-device performance remain NOT RUN. No APK was built or tested.
The inherited 0.4 m instruction HUD and close workshop require hardware review.
Desktop pull units are normalized mouse input, not measured human motion. Character
models remain project geometry; approved imported art is listed in AssetRegister.md.

---

# Historical validation - original manual-climb revision, 2026-10-18

The following results predate the learning-loop and approved-art changes above.

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
there are no imported character rigs or audio assets. Approved environment textures
were added in the subsequent learning-loop revision. This does not establish
final-art quality or parity with a commercial third-person game.

## Campus life expansion - 2026-10-18
Read CampusLifeValidation-2026-10-18.md for actual mouse interaction timestamps,
CampusLifeManual-2026-10-18.log for events, and CampusLifeSmoke-2026-10-18.txt for the
method-driven whole-adventure regression. New physical campus tasks do not gate the
original route-memory/manual-climbing loop. Hardware reach and performance remain open.
