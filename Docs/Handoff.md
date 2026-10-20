# Handoff - Highland calf integration (2026-10-18)

## Session 12 changes
The user approved the revised calf and explicitly requested its integration everywhere,
including poster art and all later appearances/copy. Session 11 approval restrictions
below are historical. Hamish remains unused; this is the Quaternius-based adaptation.

Tools/CalfPreview/CalfModel.js is shared by the browser and offline Export.mjs. The
export hashes the original CC0 source and writes HighlandCalf.calf. The narrow editor
importer creates a native prefab, one skinned mesh/material, vertex palette, 42 bones
and all 13 source animation clips. No runtime parser or package is added. Keep native
asset IDs/metas stable. Do not hand-edit the exported JSON. Rebuild with Prepare.ps1
then node Tools/CalfPreview/Export.mjs. Shader: Resources/CalfCoat.shader.

HighlandCalf owns clip crossfades, head proportions and mouth-attached funding.
CampusWorld instantiates it for all controllers, including dormant historical modes.
AdventureController uses grounded locomotion, faces actual travel, idles when stopped,
transfers the envelope during theft, removes it on recovery and restores it on a new
search. Replay resets pose/carrying. Tree topology, holds and progression are unchanged.
The poster edit uses the built-in imagegen tool at the user's explicit request;
see PosterGeneration-2026-10-18.md and AssetRegister.md for provenance.

Validation: HighlandCalfValidation-2026-10-18.md. Native asset checks passed (18),
logic tests passed, final game/editor compilation passed. Actual input covered exit,
theft and first checkpoint only. The user prohibited further computer-controlled
UI testing and requested direct push. Do not resume it without renewed consent.
Next permitted QA: full AdventureValidation and headset playthrough. Neither was
completed for this integration; do not claim headset performance certification.

## Session 11 changes
CampusDetail adds pointed stone window frames, mullions, sills, guttering, a moving
evening clock, a dressed coffee kiosk, pavement menu, entry mat, noticeboard, umbrella
stand, two gently moving banners and six pooled puddle/ripple effects. Static masonry
and trim are combined by material. Custom meshes/materials are owned and disposed by
CampusWorld. Cones now use three continuous tapered bands instead of stacked cylinders.
CampusLife adds rounded machine sides, vents, a pressure needle and a brewing status
light; only existing coffee state drives them. Coffee hints fit on shorter lines.
No new external assets, packages, sounds or AI media were imported in this pass.

Hamish The Highland Cow was preview-approved, including blocky appearance. The user
then could not complete the required Sketchfab account flow and requested another
candidate after campus work. It is NOT imported. Keep monkey naming coherent until
an accessible approved character is actually integrated. Do not automate sign-up.
Latest verification and remaining art limits: CampusDetailValidation-2026-10-18.md.

The user then approved a Quaternius Bull adaptation PREVIEW. Tools/CalfPreview contains
the authored local review and setup; source files stay in ignored Logs outside Assets.
The calf has compact proportions, short horns and head/neck/torso-bound coat locks.
Actual browser review covered Idle, Walk, Gallop and Eating. It is not yet a Unity
asset or a finished groom/skin. Next character step: obtain appearance approval for
the revised calf, then export/import, optimise coat meshes, validate all required
clips and update chase/text coherently. Preserve the horse companion and algorithm.
Session 11 regression: 138 assertions passed, plus actual mouse coffee/exit input.
Saved calf review: Evidence/HighlandCalf-Preview-2026-10-18.png and
Evidence/HighlandCalf-Front-2026-10-18.png. The review viewer is reproducible using
Tools/CalfPreview/Prepare.ps1; downloaded base/vendor files are intentionally ignored.

## Campus life baseline
The user prioritized environment and campus interaction while a free animated Highland
calf is sought. Keep the existing monkey until a suitable model is approved and inspected.
The current approved cow has no animations and its official download requires login.
See AssetRegister.md for the decision and approved environment provenance.

CampusLifeState owns optional coffee and archive progression independently of the
algorithm. CampusLife owns physical props, input, docking, brewing, stamp impression,
drawer animation and clue. Cup/Stamp/Form/Drawer transforms are test-visible. Build takes
AdventureWorld, XRRig, AdventureState and a local event logger; the route clue derives
from actual parent IDs. Tick advances only during Arrival; Reset clears the entire task.
Do not replace mouse dragging or tracked near-grip with text-only buttons.

New CampusAssets loads approved FBX, preserves native materials where present, fits to
named footprints and applies the Food Kit atlas. CampusPBR combines diffuse/normal/
roughness for wood and bark. Import rules cap texture resolution at 1024 with mipmaps.
Four desks remain: the middle row is removed for approach space at the two worktops.
Movement collision bounds must match this arrangement and retain the centre exit aisle.
Warm point lights cast no additional real-time shadows; the existing sun retains shadows.
No mobile GPU, draw-call budget, headset comfort or seated reach measurement is claimed.

Interaction contract: invalid drop returns safely; coffee cannot move while brewing;
form requires stamp; drawer requires submission; clue requires opening. All are optional.
Holding pauses movement/snap; tracking/focus loss cancels. R/replay resets props alongside
AdventureState. Trigger rays only operate BREW/card; physical items use near grip in VR.
Desktop drawer pulls downward on screen. No free-throw rigidbody simulation is present.

Remaining art is explicit: architecture, tree shape/platforms, horse, monkey, climbing
apparatus, coffee-machine housing, stamp and form use project-authored geometry. Imported
materials and dressing improve them; they are not final bespoke models. No new AI media.

Latest verification: CampusLifeValidation-2026-10-18.md; actual mouse flow and reset
passed. Final integration report: CampusLifeSmoke-2026-10-18.txt. Read
CampusArtCoverage-2026-10-18.md before claiming all models/textures are final.

## Prior gameplay contracts (still apply)

## Three-cycle improvement contracts
See ImprovementCycles-2026-10-18.md for per-cycle changes, evidence and decisions.
Cycle 1 adds procedural campus surfaces, an open branch connection workbench and
post-introduction targets at 3/5/7/4/6 levels. Preserve source-node-specific memory.
The user now permits small optional algorithm quizzes; do not gate the core climb.
Cycle 2 adds a Quiz phase, entered only from Complete and returning there on skip.
AnswerQuiz permits retries, awards one ResearchStamp per search and preserves all
climb/cone counters. The next target resets QuizSolved but keeps earned stamps.
CampusLife now owns arrival-only physical tasks; GreetHorse remains available while resting.
Optional audits cover composition, remaining-distance bounds and actual ancestors.

## Current scope
Single AdventureController journey: classroom exploration, funding theft, seven campus
waypoints, aerial map reveal, original tree, four individual exploration edges, two
cone discoveries, two two-edge constructions, four-edge composition, new target,
manual reuse, funding recovery and results. New Target keeps memory inside this game;
Replay from Classroom resets it. No RaceController handoff exists.

All game text, code, documents and logs are English. Sandstone and furniture imports
were approved in the user request; the AI poster was separately approved after preview.
AssetRegister.md and PosterGeneration-2026-10-18.md record provenance. No other AI or
external asset is approved by implication. University photographs remain reference-only.

## Gameplay ownership and contracts
- AdventureState is Unity-free. OriginalLevels/OriginalGoal preserve the generator's
  race target. RequiredLevels/Goal describe the active search, initially four edges.
- TotalLevels, Selections and TotalPull are lifetime counters; MissionLevels and
  MissionSelections reset at a target change. Remaining is mission-relative.
- Observed edges and constructed connections are indexed by their source node.
  Two edges require both observed parents; four requires both stored adjacent twos.
  Knows/CanChoose reject unrelated, undiscovered and overshooting connections.
- Clue pauses at exploration levels two and four. CollectCone enters Weaving.
  Weave constructs the pending path; after the second two it requests four-edge
  composition. Retargeting follows that composition. This is binary lifting.
- NextTarget preserves tree and memory, returns to StartNode, and chooses the original
  final goal first. Further searches cycle through 3/5/7/4/6-level goals. They do not load
  another controller, tree topology or game.
- BeginResearch is Complete-only and offers a new connection only when its two
  adjacent halves exist. Researching distinguishes optional construction from tutorial
  Weaving. CancelResearch changes no memory; successful research returns to Complete.
- BestRouteDecisions is frozen at NextTarget using MinimumDecisions over actual
  source-node memory. EfficientRecoveries counts matching routes. Further research
  must never rewrite that search's rating. ResearchedConnections counts new builds.
- Optional Quiz is Complete-only, always skippable and awards at most one stamp per
  search. QuizSolved resets on NextTarget. Quiz must not mutate climbing or cone state.
- Pull requires finite input in (0,0.2], accumulates 0.65 per physical edge, and follows
  horizontal and vertical ancestor displacement. No idle or selection-time ascent.
- AdventureController owns input, transitions, animation and local event logs.
  Desktop uses OnGUI mouse events; preserve this to avoid losing short drags or making
  release accidentally collect a cone. Connection construction supports drag-to-socket
  and sequential endpoint clicks; VR uses endpoint trigger selections.
- XRRig retains headset rotation. Aerial and target-return transitions fade in VR.
  No forced head pitch, head bob, camera roll or falling penalty.
- AdventureWorld owns workshop, remembered branch traces, counts, world choices and
  the approved classroom art. CampusWorld supplies common art and sandstone material.
- ApprovedAssetImport bounds texture dimensions to 1024 and disables furniture animation,
  generated colliders. New nature/furniture folders retain embedded materials; food uses
  its approved atlas. No new dependency or package was added by this revision.

## Validation
Current three-cycle revision: all three Unity reports pass. Actual mouse input passed
classroom/chase/map, four single exploration edges, two/four composition, six-level
4+1+1 recovery, research cancel/build, another four composition, a three-level 2+1
recovery, quiz wrong/correct/back and clean replay at 01:14:47 UTC. See
ThreeCyclesManual-2026-10-18.log, Cycle3-Smoke-2026-10-18.txt and AdventureValidation.md.
Five connections persisted into the repeat search. The research benchmark remains
frozen for the completed mission. Unity was stopped after successful validation.
Procedural surfaces and quiz/research controls still need target-headset profiling.

Previous baseline evidence:
Pure logic: Scripts/Test-Logic.ps1 reports ALL PASSED, including 40 adventure seeds,
node-specific memory, composition, partial edges, effort invariants, original topology,
repeated searches and reset. Existing tree/race/layout regression checks remain.
Unity method-driven integration passed the complete loop and clean replay, including
approved artwork/furniture loading, new-target text margins and absence of RaceController.
Actual mouse-input walkthrough passed the entire classroom/chase/explore/compose/reuse
flow, a repeat five-level target using 2+2+1, and clean classroom replay at 00:20:40 UTC.
See AdventureValidation.md, LearningLoopManual-2026-10-18.log and Evidence screenshots.
The final new-target margin adjustment changes presentation only; integration was rerun.
Stop Play before C# edits. Unity domain reload discards runtime-only state.

No headset connected. Stereo, physical reach, grip tracking, two-hand feel, haptics,
comfort and on-device performance remain NOT RUN. The inherited 0.4 m HUD needs a
headset comfort pass. Desktop pull units are not measured human movement. The climb
follows constrained ancestor branches, not unrestricted bark physics. No APK tested.

## Art follow-up
Public source inventory confirms Horse.fbx and Horse_White.fbx. Google Drive refused
the actual horse download due to quota. Do not import the HTML response as a model.
No rig/animation inspection or model replacement was completed. Current horse and
monkey remain project geometry. Source and licence links are in AssetRegister.md.

## Collaboration and publication
User authorizes normal uploads to https://github.com/isym444/BinaryLiftVR.git, Fexas
only. Verify branch, origin and remote ancestry before pushing. Never main or force-push.
No permission to message teammates. Preserve local .cursor and the pre-existing edits
in XR settings, Packages/manifest.json, Packages/packages-lock.json and ProjectSettings.
These unrelated files are outside this contribution. Do not commit Unity caches, account
information, downloaded review pages or the full asset archive. Keep source .meta files.

Next owner: perform VRValidation.md on hardware, especially physical connector reach,
text readability, target-return fade and texture/frame cost. Extend route-memory
content only while preserving per-node prerequisites and manual edge traversal.
