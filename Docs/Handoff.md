# Handoff - Classroom to Canopy (2026-10-18)

## Accepted scope and current runtime
The default experience is a single first-person adventure: classroom exploration,
exit, proximity-triggered monkey theft, seven connected campus checkpoints, aerial
map reveal, original tree overview, manual branch climbing, cone, results and replay.
No entry or ending launches RaceController. The previous two-edge upgrade trial is
superseded. All content is English and all art uses approved code-built primitives.

## Ownership and implementation contracts
- AdventureState owns phase guards, original tree/start/goal, selected span, completed
  edges, remaining levels, decision count and accepted pull amount. It is Unity-free.
- TreeGenerator.Generate(13,16,18), RaceState.Create and TreeLayout.ComputeX(tree,1.3)
  preserve original topology/layout. Unity Mono's current seed produces node 23 to 8,
  six levels. Avoid hardcoding these node IDs in gameplay; derive them from the model.
- Choose(1/2/4/8) only schedules a valid ancestor span. Pull accepts finite input in
  (0,0.2] and requires 0.65 units per edge. Node increments only at an edge boundary.
  Larger spans reduce decisions, never physical edge traversal or effort.
- AdventureController owns the rig, input, chase, animation, transitions and local log.
  ApplyPull interpolates the current real parent edge, including horizontal motion.
  No Update-time or selection-time automatic climbing is permitted.
- Desktop pointer actions all use OnGUI events. Preserve this: Input polling loses
  short drags and can turn release into an accidental cone click. Release/focus loss
  stops holding. VR grip input is proximity-based, averaged across two tracked hands;
  discontinuities above 0.20 m release the affected hand.
- AdventureWorld builds classroom, branch traces, retained node arena, rounded holds,
  recovery balcony and results. CampusWorld provides shared primitive art helpers.
- XRRig.SetDesktopView is desktop-only. VR aerial/overview transitions use stationary
  faded placements and preserve root yaw and head tracking. Do not force head pitch.
- RaceController and historical CampusQuest/TreeClimbing sources remain for regression
  history. They are not launched by the adventure or its results screen.
- AdventureValidation runs timed walking plus direct action/pull calls. It verifies
  topology count, choice without movement, no idle movement, cone, single ending and
  replay. This is separate from actual mouse input evidence.

## Validation and known boundaries
Pure logic and final Unity integration pass. A complete real mouse journey also passed;
see AdventureValidation.md and the dated logs for the distinction between builds/runs.
Stop Play before C# edits; domain reload discards runtime-only state.
The seed-13 goal has six levels; the eight-level button is correctly disabled. Longer
seeds exercise eight-level traversal in logic tests. There is no in-game seed picker.
The climb is guided along ancestor branches, not unrestricted free-bark physics.
Classroom boundaries/furniture checks are a simple desktop/stick movement constraint,
not a room-scale collision or guardian system. The HUD at 0.4 m needs comfort review.
No headset was connected. Stereo, tracking, reach, haptics, comfort and performance are
NOT RUN. Android modules/APK remain unavailable. No audio or external character rigs.

## Publication and collaboration
User authorizes uploads to https://github.com/isym444/BinaryLiftVR.git on Fexas only.
Verify remote URL, branch and remote ancestry before normal pushes. Do not use main
or force-push. Exclude Library/Temp/Logs caches, installers, credentials and personal
.cursor settings. Sanitized project evidence is stored in Docs. Keep source .meta files.
Every meaningful step requires English DevelopmentLog, validation and handoff updates.
No authorization to message teammates was given. External free assets still require
preview/source/license approval. The supplied statue photograph remains reference-only.

## Next owner actions
Hardware owner: follow VRValidation.md, particularly close grips/HUD, sideways branch
motion, two-hand averaging, stationary aerial overlook and complete replay.
Art owner: propose free non-AI rigged horse/monkey and campus assets for approval before
import. Use current named primitives as replacement points and preserve target contracts.
Gameplay owner: retain single-flow completion and original tree structure; extend seeds
or progression only after agreeing the next experience with the user.

## Published implementation
Implementation commit eb6d436f317564696b30e9cd622fb428e39ddef9 was pushed to Fexas
and verified against the remote. Main was verified unchanged at
15279ec90778d1ff7e01b3e04f51e833d8b57184. This handoff receipt is a documentation-only
follow-up; the tested gameplay implementation is the commit named above.
