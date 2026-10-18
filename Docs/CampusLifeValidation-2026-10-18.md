# Campus life validation - 2026-10-18

Environment: Unity 6000.0.85f1, Windows desktop Play Mode, mouse and keyboard.
Headset/controller hardware, mobile build, GPU profiling and audio: NOT TESTED.

## Logic
Scripts/Test-Logic.ps1: ALL PASSED. Includes ordered campus actions, invalid placement,
invalid time, brewing movement lock, fill completion, no duplicate rewards and clean
reset, plus all existing algorithm/adventure regression cases.

## Actual input walkthrough
The final manual run used real Unity Game-view mouse/keyboard events; no direct state
mutation or editor fixture was used for the campus actions. The journal records UTC:
- 09:03:36: dragged empty cup into visible drip tray, including a turned camera view.
- 09:03:45 to 09:03:48: clicked BREW; brew finished after approximately 2.5 seconds.
- 09:03:55: dragged full cup onto saucer; completion hint and steam visible.
- 09:04:24: dragged stamp onto form; visible ink impression and instruction changed.
- 09:04:42: dragged stamped form into the in tray; drawer became available.
- 09:04:59: dragged drawer handle downward; drawer slid open and card appeared.
- 09:05:18: clicked card; real route 23 > 13 > 12 appeared in the clue hint.
- 09:05:36: R reset returned cup/form/stamp, closed drawer and cleared both tasks.
- 09:05:50 to 09:05:52: exited without redoing tasks; theft/chase started normally.

Journal: CampusLifeManual-2026-10-18.log. It contains only gameplay events/counters.
Evidence: Evidence/Adventure-Arrival-090404.png (served coffee),
Evidence/Adventure-Arrival-090509.png (opened drawer/card),
Evidence/Adventure-Chase-090614.png (courtyard and chase after reset/exit).
No supported screen-recording tool was available; screenshots and actual-input event
logs are provided instead. Do not describe these as a video or headset recording.

## Issues discovered and fixed
Initial camera-facing drag plane rejected a visually plausible cup drop after a snap
turn. The final version drags desktop props along the tabletop plane. The complete
coffee and archive retest above passed after this correction. Invalid drops returned
safely during the earlier test. Added a visible drip tray to clarify the docking target.
Removed the middle desk row and updated movement bounds so the worktops have an approach
area. Fixed controller initialization overwriting the new ambient lighting. Added prop
cursor feedback and disabled snap/locomotion while holding. Device reach remains open.

## Automated Unity integration
See CampusLifeSmoke-2026-10-18.txt for the final method-driven regression. It exercises
classroom, chase, map transition, original tree, 1/2/4 progression, cones, new targets,
research, quiz and replay; verifies PBR shader compilation and absence of RaceController.
This proves integration contracts, not human controller comfort. Prior full manual
algorithm walkthrough remains documented in ImprovementCycles-2026-10-18.md.
