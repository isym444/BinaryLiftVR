# Handoff - Funding and route memory (2026-10-18)

## Three-cycle improvement contracts
See ImprovementCycles-2026-10-18.md for per-cycle changes, evidence and decisions.
Cycle 1 adds procedural campus surfaces, an open branch connection workbench and
post-introduction targets at 3/5/7/4/6 levels. Preserve source-node-specific memory.
The user now permits small optional algorithm quizzes; do not gate the core climb.
Cycle 2 adds a Quiz phase, entered only from Complete and returning there on skip.
AnswerQuiz permits retries, awards one ResearchStamp per search and preserves all
climb/cone counters. The next target resets QuizSolved but keeps earned stamps.
Coffee/Paperwork are arrival-only interactions; GreetHorse is available while resting.
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
  generated colliders and embedded materials. No new dependency or package was added.

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
