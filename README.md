# Cone Courier - Classroom to Canopy

English-only Unity VR project set in a University of Glasgow-inspired electronic campus.
Current flow: classroom exploration -> exit -> monkey theft -> seven connected chase
waypoints -> aerial route reveal -> original generated tree -> manual ascent -> cone
recovery -> results -> replay from the classroom. There is no second-game handoff.

## Run
Open Assets/Scenes/Main.unity in Unity 6000.0.85f1 and press Play. Scenery is generated
at runtime; Edit Mode intentionally has no game camera. Stop Play before editing C#.
Use Assets > Refresh after editing. Double-click the Game tab for a larger view.

## Desktop controls
- WASD: explore the classroom/courtyard. Hold right mouse to look; Q/E: snap turn.
- Optional EXIT TO COURTYARD button walks through the door. Walking near the notebook
  outside triggers the theft once; starting the game does not trigger it automatically.
- Follow each orange waypoint by walking or clicking its beacon. WASD cancels auto-walk.
- Hover 1/2/4/8 LEVELS to preview a valid ancestor path; click to select it.
- Click and drag an orange branch hold downward. Release to rest, then regrip.
  Selecting a route or holding still never moves the player. Each actual node crossing
  increments CLIMBED; THIS ROUTE shows completed levels within the current selection.
- Connections beyond the goal are disabled. The fixed seed 13 route has six levels,
  so 8 LEVELS is unavailable in this slice; logic tests cover valid eight-level routes.
- Recover the cone at the gold node. Results offer only REPLAY FROM CLASSROOM.
- R restarts with a fade. There is no chase timer or falling penalty.

## VR controls and presentation
Left stick moves; either controller trigger selects. Grip near an orange hold and pull
down to climb; alternate hands, release to rest. Two-hand input is averaged. Right stick
snaps when no hold is held; B/Y restarts. Grip near the cone or select it with a ray.
Desktop reveal raises and tilts the camera. VR fades to a stationary overlook, preserving
tracked head rotation; look down at the campus. Map/tree changes use a fade.
No forced head bob, camera roll or falling. Device comfort and reach still need testing.

## Original tree contract
TreeGenerator.Generate(13,16,18), RaceState.Create and TreeLayout.ComputeX(tree,1.3)
provide the original 35-node topology, target and layout. Vertical spacing is one metre.
Selection schedules a real ancestor chain; manual motion follows every edge, including
its horizontal displacement. Each edge requires 0.65 m of accumulated downward input.
Larger selections reduce decisions and stopping points, not physical distance or effort.
RaceController and rejected prototypes remain as historical source, with no adventure
button or completion path that launches them.

## Validation and collaboration
Run Scripts/Test-Logic.ps1 (Unity Mono; optional -UnityEditorRoot for another installation).
BinaryLift > Validate Adventure (Ctrl+Shift+J) runs method-driven integration.
BinaryLift > Capture Adventure Screenshot (Ctrl+Shift+K) records the current view.
Read Docs/AdventureValidation.md for actual input evidence and hardware limitations.
No headset or APK validation is claimed; Android modules are not installed.

All visuals are approved code-built Unity primitives and materials. No AI media,
external art, reference-photo pixels or audio assets were imported. This is a playable
prototype with placeholder characters, not a final-art campus reconstruction.

Read AGENTS.md, Docs/Handoff.md, Docs/Design.md and Docs/TeamWorkflow.md before changes.
Keep English development logs and handoffs current. Runtime milestone logs remain local
under Application.persistentDataPath/QuestLogs; no account data or pose streams are logged.
The user authorized GitHub uploads to https://github.com/isym444/BinaryLiftVR.git on
Fexas only. Verify branch and remote before each push; do not push main or force-push.
