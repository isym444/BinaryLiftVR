# Cone Courier - Classroom to Canopy

English-only Unity VR project set in a University of Glasgow-inspired electronic campus.
Current flow: classroom exploration -> exit -> monkey theft -> seven connected chase
waypoints -> aerial route reveal -> original generated tree -> manual exploration -> cone collection -> two/four-edge route memory
-> new funding target -> manual reuse -> results -> replay from the classroom. There is no second-game handoff.

## Run
Open Assets/Scenes/Main.unity in Unity 6000.0.85f1 and press Play. Scenery is generated
at runtime; Edit Mode intentionally has no game camera. Stop Play before editing C#.
Use Assets > Refresh after editing. Double-click the Game tab for a larger view.

## Desktop controls
- WASD: explore the classroom/courtyard. Hold right mouse to look; Q/E: snap turn.
- Optional EXIT TO COURTYARD button walks through the door. Walking near the funding envelope
  outside triggers the theft once; starting the game does not trigger it automatically.
- Follow each orange waypoint by walking or clicking its beacon. WASD cancels auto-walk.
- Start with 1 LEVEL. Explore two edges, collect a cone, then drag the orange memory
  plug to the cyan socket (or click each). Explore two more edges and repeat; join
  the two remembered routes into a four-edge connection. No quiz is involved.
- FOLLOW THE FUNDING returns to the same tree start with memory intact and a new goal.
  Hover available 1/2/4 LEVELS to preview; click to select. Memory is specific to nodes.
- Click and drag an orange branch hold downward. Release to rest, then regrip.
  Selecting a route or holding still never moves the player. Each actual node crossing
  increments CLIMBED; THIS ROUTE shows completed levels within the current selection.
- Grey connections are undiscovered at this node or pass the goal. This slice stops
  at four-level memory. Seed 13 explores four levels, then revisits the start for the
  original six-level goal: four remembered levels plus two individual edges.
- Recover the final cone. NEW TARGET keeps memory and alternates five/six-level goals;
  REPLAY FROM CLASSROOM clears progress. Neither launches the legacy game.
- R restarts with a fade. There is no chase timer or falling penalty.

## VR controls and presentation
Left stick moves; either controller trigger selects. Grip near an orange hold and pull
down to climb; alternate hands, release to rest. Two-hand input is averaged. Right stick
snaps when no hold is held; B/Y restarts. Grip near the cone or select it with a ray.
Select the orange plug and then the cyan socket to compose a memory connection.
Desktop reveal raises and tilts the camera. VR fades to a stationary overlook, preserving
tracked head rotation; look down at the campus. Map/tree changes use a fade.
No forced head bob, camera roll or falling. Device comfort and reach still need testing.

## Original tree contract
TreeGenerator.Generate(13,16,18), RaceState.Create and TreeLayout.ComputeX(tree,1.3)
provide the original 35-node topology, final target and layout. Vertical spacing is one metre.
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

Approved Poly Haven sandstone, Kenney desks/chairs and a separately preview-approved
AI funding poster are now used. Characters and architecture remain project-built.
Read Docs/AssetRegister.md for provenance, licences and the pending horse review.
The learning loop is specified in Docs/Design.md. No audio or university-photo pixels
are imported. This remains a stylized campus prototype, not a surveyed reconstruction.

Read AGENTS.md, Docs/Handoff.md, Docs/Design.md and Docs/TeamWorkflow.md before changes.
Keep English development logs and handoffs current. Runtime milestone logs remain local
under Application.persistentDataPath/QuestLogs; no account data or pose streams are logged.
The user authorized GitHub uploads to https://github.com/isym444/BinaryLiftVR.git on
Fexas only. Verify branch and remote before each push; do not push main or force-push.
