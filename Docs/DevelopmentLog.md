# Development log

## Session 10 - Campus life and approved art expansion (2026-10-18)
User selected warm stylized art and hands-on campus life. Approved free Kenney Food
and Nature kits, Poly Haven Bark Brown 02 and Wood Planks. Existing furniture kit
approval remains. Highland cow appearance approved for further inspection only.
Sketchfab public metadata: 46,812 faces, zero animation clips, CC BY 4.0; official
download requires authentication (401). User chose to prioritize environment and
campus interactions while continuing the search for a free animated cow. Do not
claim the cow is replaced or the candidate rig has been inspected.
Implement cup placement/brewing/serving and stamp/submission/drawer/clue actions,
with desktop dragging and tracked VR grip support. Preserve original climbing and
all unrelated settings/package edits. Test logic, Unity integration and actual input.

### Implementation and review
Added CampusLifeState (Unity-free guards) and CampusLife (desktop tabletop dragging,
tracked near-grip, forgiving docking, visible coffee fill/stream/steam, stamp impression,
form submission, drawer pull and parent-route card). AdventureController owns its
lifecycle, records local events, cancels grabs on focus loss and prevents movement/snap
while holding. Campus tasks stay optional and award no algorithm progress.

Imported only the approved Food/Nature/Furniture subset and two Poly Haven 1K material
sets, with unchanged publisher licences and updated source hashes. CampusAssets preserves
embedded nature/furniture colours and uses the food atlas; CampusPBR combines diffuse,
normal and roughness. Added books, lamps, shelves, plants, benches and courtyard trees.
Removed the middle desk row for approach space; matched the controller's movement bounds.
Fixed startup overwriting warm ambient lighting; added shadowless local light pools.

The first real mouse test revealed that a camera-facing drag plane rejected cup docking
from a turned view. Changed to a tabletop plane and added a visible drip tray. Retested
the full coffee and archive sequences successfully using actual mouse input. Verified
R clears all completed tasks and exiting with tasks skipped still triggers the chase.
See CampusLifeValidation-2026-10-18.md and its UTC event log/screenshots. No supported
recording tool was available; no video or headset test is claimed.

### Documentation and handoff
Updated README controls, Design, Handoff, AdventureValidation, VRValidation, AssetRegister
and ApprovedAssetHashes. Added CampusArtCoverage to distinguish imported finished source
assets from remaining authored geometry. All source/game/documentation additions are
English. Cow remains deferred by user choice; no suitable free animated Highland calf
has been verified, and the static appearance candidate was not downloaded or imported.

### Verification and publication scope
Scripts/Test-Logic.ps1 returned ALL PASSED. Whole-adventure Unity checks include PBR
shader compilation, prop instantiation, unchanged algorithm progression, no legacy
controller handoff, quiz/research and clean replay. Save the final report alongside the
manual evidence before publication. Stage only this revision's gameplay/editor/assets,
licences, tests and English docs. Preserve unrelated XR/package/project-setting edits
and .cursor. Upload only to the user-authorized Fexas branch with a normal push.

Final Unity integration: PASS at 09:08:06 UTC. Saved CampusLifeSmoke-2026-10-18.txt;
stopped Play afterward. No legacy game controller was instantiated.
Extended .gitattributes to preserve nested FBX/licence source bytes. Renormalization
also restores original CRLF bytes for the two existing desk/chair FBX files, matching
their already-recorded source hashes; diff ignoring line endings confirms no model change.

All development records, source comments, game text, and handoffs are written in English.
Entries describe meaningful work steps, evidence, and unresolved work. Do not claim a
headset test based on desktop Play Mode.

## 2026-10-17 — Session 01: campus cone quest

### 01. Requirements and baseline inspection
- User approved a primitive-only playable prototype with a first-person player and a
  horse companion inspired by Glasgow's cone-wearing equestrian statue.
- Setting: a fictionalised University of Glasgow campus, with everyday electronic
  signage, sandstone architecture, subdued cyan accents, and orange interaction cues.
- Each recovered clue is a cone. Cones unlock binary-lifting abilities.
- Reference photograph is reference-only; it must not be imported into the game.
- No AI-generated media. Free external assets still require user approval before import.
- The project is shared with human teammates and other AI contributors. Documentation
  and handoff records are part of the deliverable, not optional follow-up work.
- Baseline: Unity 6000.0.85f1; runtime-generated legacy race; OpenXR/Quest configuration;
  mouse fallback; engine-independent tree logic and console tests.
- Existing uncommitted changes were present in ProjectSettings/ProjectSettings.asset,
  Assets/XR/XRGeneralSettingsPerBuildTarget.asset, and
  Assets/XR/Settings/OpenXR Package Settings.asset from the previous setup session.
  Preserve these changes; do not reset the repository.

### 02. Implementation decisions
- Keep the existing race implementation as a legacy mode; make the campus quest the
  default bootstrap. Keep algorithm/progression logic independent of Unity.
- Use grounded teleport stations and a world-space route terminal, rather than a
  floating wall of giant portals. Keep the camera under player control.
- Implement a short collection/unlock loop before expanding scope to LCA or free
  traversal. Binary lifting and BFS are distinct concepts; do not call parent stepping BFS.
- Build only reversible primitive placeholders. No external models, textures, audio,
  fonts, generated pictures, or reference-photo pixels will be imported.
- Record runtime milestones locally as JSON Lines, alongside Unity's normal log.

### 03. Rules and presentation implementation
- Added CampusQuestState with four clue stations (depths 19, 17, 13, 0), a start at
  depth 20, and powers-of-two unlocks on deposit. Kept the existing TreeModel lookup table.
- Added explicit Briefing, Exploring, CarryingCone and Complete phases. Invalid jumps,
  clue overshoot, duplicate pickup/deposit, and travel while carrying are rejected.
- Added an actual BFS traversal for the optional map scan; parent travel is described
  as ancestor stepping, not BFS.
- Added CampusWorld: procedural metre-scale campus, a simplified tower and cloisters,
  paving, benches, coffee kiosk, horse/monkey placeholders, four cone trophies,
  world-space terminal, and a route tree. Combined static geometry by shared material.
- Added QuestTarget as a stable interaction contract for future art replacement.
- Changed the default bootstrap to CampusQuestController; retained the legacy race.

### 04. XR interactions and runtime records
- Extended XRRig with per-hand select edges, grips, capability-checked haptics,
  right-stick snap direction, and head-offset-aware teleport placement.
- Implemented ray pickup/docking, near-grip pickup/release, safe return on dropped or
  untracked cones, path previews, fade teleport, and optional 30-degree snap turns.
- Implemented QuestJournal: local JSONL milestone events. No network telemetry or tracking
  pose data. Log failures are nonfatal and do not block gameplay.
- Audio deliberately remains absent because no audio asset has been approved.

### 05. Automated checks
- Extended LogicTests/Program.cs for quest progression, blocked actions, preview paths,
  drop/recovery, six-jump completion, BFS coverage/order, and fresh state.
- Added Scripts/Test-Logic.ps1 to compile/run the console suite with Unity's bundled
  Mono compiler when a .NET SDK is unavailable. Actual result: ALL PASSED.
- Added a repeatable Editor menu integration runner. Initial actual result: PASS for
  the complete four-cone flow and restart. This invokes controller actions directly.
- Unity editor compiled and entered Play Mode successfully. No gameplay exception or
  C# compiler error was observed in the initial successful run.

### 06. Visual inspection and revision
- Inspected the real Unity Game view with computer-use tools.
- Found that the companion was outside much of the default forward view and terminal
  body/button text was too small. Moved the horse farther forward and increased text sizes.
- Wrapped long interaction hints. Replaced material-cloning hover highlights with
  MaterialPropertyBlock to avoid creating unmanaged per-target material instances.
- Fixed the quest select edge to accept either hand independently, including while the
  other controller's trigger remains held.
- Used explicit Assets Refresh (Ctrl+R) before final validation because the editor had
  retained older assemblies during the initial layout rerun.

### 07. Team documentation and asset research
- Replaced the MVP-only README with current setup, gameplay, controls, architecture,
  testing, Quest build prerequisites, and log locations.
- Added Design, TeamWorkflow, AssetRegister, VRValidation, Handoff, and validation records.
- Reviewed official Kenney pages for two free CC0 candidates; linked source previews and
  recorded that approval is pending. No external assets downloaded or imported.
- No teammates were messaged, no remote repository was modified, and no commits were made.

### 08. Actual desktop interaction checks
- Used mouse input on the real Game view to begin, jump one level, pick up the cone and
  dock it on the saddle. Verified the 1/4 trophy count and 2-step unlock on screen.
- Used Q/E snap turns to inspect the clue stand and return to the companion.
- Lowered the desktop-held cone and moved it farther from the camera after noticing
  it occupied too much of the instruction area. This does not alter tracked-hand scale.
- Final verification and hardware limitations are recorded in Validation-2026-10-17.md.

### 09. Persistent contributor rules
- Applied the create-rule skill to encode the already-specified repository-wide policy.
- Added AGENTS.md and an always-applied .cursor/rules/project-collaboration.mdc rule.
- Rules reference the team workflow and preserve English-only project content,
  prior asset approval, no AI media, detailed records, and honest headset validation.

### 10. Final verification and delivery state
- Refreshed Unity sources and reran the complete Editor smoke test after the final
  carry-position adjustment. Actual result: PASS; fresh briefing left running.
- Copied the actual test output to Docs/DesktopSmoke-2026-10-17.txt for shared review.
- Checked source .meta coverage, reviewed repository status, and ran git diff --check.
- Searched new project source/docs for Han characters; none found. English-only project
  content is also enforced by the contributor instructions.
- No C# compiler errors or gameplay exceptions found in the final editor log inspection.
- Headset checks and Android build remain explicitly pending. No external art/audio imported.

## Session 02 - Physical tree climbing (2026-10-17)
### 01. User correction and implementation scope
The user rejected terminal-based quiz progression. The default experience must become
physical hand-over-hand climbing on a volumetric tree with forks, bark, branches and
concealed cones. Preserve campus scenery and the horse at ground level. Implement a
new controller and pure progression model; retain the previous prototype for reference.
Use approved primitives only. Add direct grip locomotion, a safety belay on release,
and a manually pulled ancestor rope to make upgrades change physical traversal.

### 02. Physical environment and movement
Added TreeClimbingController, ClimbProgress and a separate Editor validation runner.
Built an 8.8 m tree with roots, tapered trunk, bark knots, two branches, canopy and
four freely discoverable cones. Default bootstrap now launches climbing. CampusWorld
has a climbing construction mode; historical terminal code is retained, not deleted.
Grip anchors counteract hand displacement, combine two hands, release on tracking loss
and bound discontinuities. A safety belay holds position on release. Added head clearance.

### 03. Algorithm integration and collaboration records
Cones upgrade a manually reeled ancestor rope using TreeModel.KthAncestor. The rope
traces branch junctions instead of cutting across siblings. Added an actual depth-eight
crown node so the eight-edge ability has a valid route. Added logic regression cases.
Rewrote README, Design and Handoff around the corrected experience and its limitations.
No media imported/generated; no teammates messaged or repository changes published.

### 04. Unity inspection and additional checks
Initial Play Mode integration passed construction, contact range, bounded rig motion,
ancestor progression and reset. Inspected the actual tree at the player's eye level.
Reduced bark gloss and flattened bark knots after visual review. Added simulated
tracked-hand regression checks for two-hand displacement, tracking loss, cone pickup
and trophy display. Rope input now uses tracking-local displacement so rig travel cannot
feed back into further pull input. Real headset interaction remains untested.

### 05. Final validation and delivery
Pure logic suite: ALL PASSED. Final fresh Unity tree-climb integration: PASS, including
simulated two-hand motion, tracking loss, unique collection and trophy display. Copied
actual report to Docs/TreeClimbSmoke-2026-10-17.txt. New source .meta files exist.
Recorded untested physical hardware and manual end-to-end traversal explicitly in
ClimbingValidation-2026-10-17.md. The default scene is left running at the tree base.

## Session 03 - Restore original playable race (2026-10-17)
### 01. User rejection and baseline
User rejected the physical-tree prototype as unplayable and asked to optimize the
very first, unmodified game. Git contains one baseline commit, 15279ec. RaceController
is still identical to that baseline except for its bootstrap. Restore that controller
as the default, preserve experiments off the default path, and improve onboarding,
interaction feedback and retry behavior without replacing the original race mechanics.
Prior unit/integration checks did not prove end-to-end playability. This delivery
must distinguish actual mouse playthrough from direct-action tests and headset tests.

## Session 04 - Approved adventure vertical slice
### 01. Scope and baseline
The user approved courtyard arrival, notebook theft, a player-controlled monkey chase,
a visible map-to-tree transition, fixed grip climbing, one cone, and a two-edge upgrade
on the same route. Preserve the playable RaceController as a separate accessible mode.
Do not reuse the rejected free-bark controller. All code/UI/docs remain English; no
external or AI media. Build a deterministic adventure state and dedicated presentation.
Acceptance requires a full desktop input playthrough as well as logic/integration checks.
Headset feel and final art quality must not be inferred from desktop results.

### 02. Implementation
Added AdventureState, AdventureWorld, AdventureController and AdventureValidation.
Preserved RaceController behavior and exposed it from arrival/completion. Added courtyard
landmarks, four chase checkpoints, monkey theft/run animation, road-map transformation,
fade transition, node platforms, fixed handles, gloves/forearms, cone pickup and an exact
same-route comparison. Two ordinary segments require 1.30 m input; one upgraded segment
requires 0.80 m. Desktop click-to-walk supplements WASD; VR supports the left stick.

### 03. First actual input inspection
Discovered MaterialPropertyBlock creation in a MonoBehaviour field initializer caused
hover exceptions and blocked selection; moved construction to first main-thread use.
Observed distant labels showing through foreground panels; added a depth-tested stereo
world-text shader, with font-atlas synchronization. Hid the monkey notebook before theft.
These are findings from real Unity interaction, not assumptions from passing logic tests.

### 04. Chase playthrough and clearance correction
Actual mouse input triggered theft and traversed all four chase checkpoints. The map
transition completed, but the initial arena placed the eye inside an upper platform.
Moved the fixed climbing lane three metres in front of the route graph, preserving the
original race's readable view relationship. Added a supporting rest deck beneath the
player and kept handles in reaching distance. Corrected arch block tangent rotation.
A fresh full playthrough is required after these changes; the interrupted run is not a pass.

### 05. Automated integration and layout review
Full automated adventure regression passed: all chase checkpoints, map transition,
ordinary ascent, cone/trophy, same-route reset, upgraded ascent and comparison. Evidence:
Docs/AdventureSmoke-2026-10-17.txt. Fixed test timeout initialization across Unity domain
reload and moved its shortcut to Ctrl+Shift+J to avoid Unity's Add Component shortcut.
Raised completion buttons and hid the ordinary HUD while the comparison card is open,
after inspecting the actual completion screen. Final manual drag playthrough follows.

### 06. Final input-only run and release-frame repair
Replaced the map morph's simple line with a traversal of the actual branching topology.
Added proximity highlighting before VR grip acquisition. During the manual desktop run,
fast drag-and-release acquired a handle but lost its last movement sample. Moved release
processing after the final displacement sample; the per-frame safety cap remains intact.
Re-ran pure logic tests: ALL PASSED. Restarted the entire manual route after compilation.
The release-order change alone did not resolve the reproduced short-stroke problem.
Replaced desktop polling with MouseDown/MouseDrag/MouseUp event processing, including
final release displacement and focus-loss release. VR tracked-pose input remains separate.
Desktop event displacement is bounded and split into accepted state-machine samples.

### 07. Full manual completion and final polish
Event-based dragging completed both ordinary connections and the two-level connection.
Actual completion displayed 2 / 1.30 m before versus 1 / 0.80 m after. Release/rest was
observed mid-climb. Replay reset the adventure and Original Race opened its ready screen.
The same drag could still be observed by polling as a new selection after landing;
moved all desktop pointer selections to the same event path to prevent unintended cone
collection. Moved the instruction panel in front of reachable handles at unchanged
angular size; verify near-focus comfort on hardware. Final build replay follows.

### 08. Final manual acceptance and evidence
Post-repair input-only replay: 22:29:13 through 22:34:59 UTC, PASS. Four chase checkpoints,
ordinary 2 -> 1 -> 0 climb, separate cone pickup, faded reset, upgraded 2 -> 0 climb and
comparison completed. Rest at 54% stayed stationary until another stroke. Copied only
this replay's milestone lines into Docs/AdventureManual-2026-10-17.log. Captured clue,
climbing and completion PNGs in Docs/Evidence. No automated gameplay calls in this run.
Removed the redundant instruction HUD during route selection because it overlapped
button labels; the separate route sign still supplies guidance. Added integration
assertion for this visibility condition. README, design, validation, asset register,
development log and handoff describe the final implementation and hardware limits.
Final post-polish automated integration finished at 22:37:30 UTC: PASS, including the
new selection-HUD visibility assertion. Final source compiles in Unity. Pure logic:
ALL PASSED. git diff --check: no whitespace errors (existing line-ending warnings only).
No headset, APK, imported assets, commit, push or teammate messages were performed.

## Session 05 - Classroom and original-tree revision preview
The user requested a restatement and visual preview before further gameplay changes.
Recorded classroom exploration before theft, more detailed chase animation, an overhead
path-to-tree reveal, original-tree layout with player-driven grip/pull after selection,
live crossed-level counting, and removal of the separate legacy-game ending link.
Created a four-panel geometric storyboard and reproducible Pillow drawing source in
Docs/Previews. Inspected the rendered image; corrected the lower caption crop and the
four-level example's edge count. No external media, AI media service or game asset
import was used. Proposed a faded stationary aerial view for VR rather than forced
head pitch. Updated README and Handoff to distinguish the proposal from current runtime.
No gameplay files or running Unity state changed. Gameplay tests were not rerun because
this contribution changes documentation and preview drawings only.

## Session 06 - Authorized original-tree revision (2026-10-18)
Implementation started after approval of the classroom and manual-climbing proposal.
Confirmed origin is https://github.com/isym444/BinaryLiftVR.git and remote Fexas exists
at the initial commit. Switched the local checkout to Fexas without discarding work.
This session is authorized to commit and push to Fexas only. Main must remain unchanged.
Plan: preserve generated topology/layout; add classroom exploration, staged theft,
connected campus-path reveal, edge-by-edge manual climbing and a single replay ending.
Validation will separate pure logic, Unity integration, real desktop inputs and unrun
headset checks. All additions use code-built primitives; no external media is imported.

### Session 06 implementation and inspection
Replaced the two-trial AdventureState with original-generator ancestry traversal.
AdventureWorld now uses the original 35-node layout, classroom furniture, opening door,
connected campus traces, 1/2/4/8 selection, branch holds and a single results card.
AdventureController now triggers theft after classroom exit, animates the snatch/turn,
walks every corresponding ancestor checkpoint, presents aerial and tree overviews,
then accepts deliberate pull input along each edge. XRRig gained a desktop-only view
setter; VR transitions retain tracked rotation and root yaw. Removed all adventure
Classic/Repeat actions and the OpenClassic controller handoff.
Logic tests cover 40 route seeds, phase guards, original topology, every crossed node,
invalid input, no movement on choice, effort accounting and terminal completion.
Initial Unity integration passed. A complete real mouse run at 23:12-23:17 UTC passed
all seven checkpoints, four-plus-two manual levels, separate cone pickup and replay.
Its log is AdventureManual-2026-10-18.log (filenames use the local London date).
Visual inspection found an exit sign behind the door, thick near-view grips and a
companion label overlapping results. Corrected the sign, replaced holds with rounded
capsules, hid the redundant label and added a recovery balcony/archive backdrop.
Added a short overview of the original tree before the close climbing viewpoint.
Final integration and post-polish input checks follow below when complete.

### Final automated verification
Final Unity script compilation succeeded after presentation changes. Test-Logic.ps1
reported ALL PASSED. The final AdventureValidation run passed, including original
35-node structure, seven checkpoints, no movement from selection/idle input, all six
crossings, cone completion, absence of RaceController, and a single clean replay.
Saved the report as AdventureSmoke-2026-10-18.txt. A second real-input pass is checking
the rounded holds and final presentation; no gameplay methods are being called in it.
Pre-upload review: origin/Fexas and origin/main both remain at the initial commit.
Credential-pattern scan found only an empty Unity metroCertificatePassword field.
No Chinese text was found in game source, editor tools, docs, README or test scripts.
Personal .cursor data and generated Unity caches are excluded from staging.

### Final input verification and release preparation
Post-polish real-input run completed at 23:26:10 UTC: classroom, seven checkpoints,
rounded grips, 1+4+1 selections, partial-edge pause, all six counts, separate cone and
single-flow replay passed. Saved the isolated manual portion as
AdventureManual-Polished-2026-10-18.log and final climbing/clue/results screenshots.
Corrected singular LEVEL grammar after the run; no movement or input behavior changed.
Trimmed Unity-generated trailing whitespace in new resource metadata. Updated the
current validation report while retaining clearly marked historical results.
README, Design, Handoff, TeamWorkflow, VRValidation and proposal status now describe
this revision and the authorized Fexas destination. No headset test or APK is claimed.
The publication commit includes the previously uncommitted foundation and historical
prototype sources needed to preserve the team's work; legacy modes are not reachable
from the default adventure. Personal .cursor files and line-ending-only XR/project
settings are left local. Main remains at its original commit.

### GitHub publication verified
Published implementation commit eb6d436f317564696b30e9cd622fb428e39ddef9 with a normal
push to origin HEAD:refs/heads/Fexas. A fresh git ls-remote confirmed Fexas matches
that exact commit and main remains 15279ec90778d1ff7e01b3e04f51e833d8b57184.
Fexas now tracks origin/Fexas. This publication receipt is a documentation-only
follow-up to the tested implementation. Pre-existing line-ending-only XR/project
settings and personal .cursor files remain local and were not uploaded.


## Session 07 - Asset selection before the learning loop (2026-10-18)
The user approved exploration, cone collection, two-edge connection construction,
four-edge composition and target reuse, preserving manual climbing and the original
single-flow game. The user requested asset selection before implementation, with real
Glasgow campus references and a humorous fictional research-funding theft premise.
A subsequent explicit instruction permits a small amount of AI-generated media.

Reviewed official University of Glasgow Quadrangles/Cloisters references, Poly Haven
Sandstone Blocks 08 and its publisher AI policy, Kenney Furniture Kit and publisher
provenance, and Quaternius Ultimate Animated Animal Pack. Recorded previews, source,
CC0 licences, free individual download options and untested integration details in
AssetSelection-2026-10-18.md. No suitable monkey replacement has been selected.
No external assets were downloaded/imported and no AI media was generated. Source-page
image links are previews only. No gameplay implementation or new playtest is claimed.

Updated AGENTS.md and TeamWorkflow.md to reflect the user's revised AI permission;
updated AssetRegister.md, README.md and Handoff.md with the selection and next scope.
Historical no-AI statements in earlier logs and validation reports remain accurate for
those sessions. The revised permission does not approve any individual external asset.

Baseline: Fexas with existing changes in XR settings, Packages/manifest.json,
Packages/packages-lock.json, ProjectSettings/ProjectSettings.asset and local .cursor/.
Preserved all these unrelated changes. Validation for this documentation-only step:
review official source statements, inspect diff, check English content and whitespace.
No Unity or headset run is warranted for documentation changes. Next: user selection,
then approved import/integration and the learning loop, with actual input walkthrough.


## Session 08 - Approved assets and remembered connections (2026-10-18)
User approved sandstone and furniture imports, continued horse review, and implementation
of exploration, cones, two-edge/four-edge composition and target reuse. The generated
funding poster preview was separately approved during this session. Scope: AdventureState,
AdventureController/World, approved resource assets, validation and English documentation.
Preserve existing package/XR/project-setting changes and local .cursor data.

### First validation and presentation repair
Logic tests passed and the first Unity integration passed the full new loop. Actual
mouse play verified classroom assets, all chase checkpoints, manual partial-edge rest
and the first cone. It exposed clipped workshop instructions and platform occlusion.
Stopped Play and moved the route/workshop/results panels into a compact foreground
dock; made the recovery balcony track the current target. The final input walkthrough
will be repeated after these fixes. Horse download was blocked by the source quota.

### Implementation and provenance
AdventureState now stores observed parent edges and source-specific two/four-edge
connections. Four individual exploration climbs yield two cones and two workshop
constructions; a final 2+2 composition unlocks four. Subsequent targets retain memory
and cones, with mission-relative counts and unchanged per-edge manual effort. Added
workshop drag/endpoint interactions, animated threads, remembered branch traces,
repeat-target fade, funding humour and one-flow results. Original tree generation,
layout, chase and manual input remain. No automatic ascent was introduced.

Imported the approved Poly Haven Sandstone Blocks 08 diffuse, Kenney Furniture Kit
desk/chairDesk and the individually preview-approved AI funding poster. Kept original
source files, licences, hashes and poster prompt in the repository. Texture imports
cap at 1024 with mipmaps; furniture imports omit unused animation/material/collider
data. Added a simple planar sandstone shader. Horse source inventory was checked,
but Google Drive returned a quota page instead of FBX; no horse replacement imported.

### Final actual-input walkthrough
Second integration passed after the dock repair. Starting from its clean classroom
handoff at 00:11:07 UTC, real mouse inputs completed all seven waypoints, transition,
four single climbs, both cone pickups, two two-edge joins and four-edge composition.
A partial pull followed by release did not advance while idle. On the next target,
4+1+1 physically traversed all six levels and recovered cone three at 00:19:17.
New Target / Keep Memory then completed five levels using 2+2+1 and recovered cone
four at 00:20:29. Replay cleared all state at 00:20:40. No second game was reached.
Saved sanitized manual events and five screenshots. A final presentation-only change
lowers the new-target panel 0.08 world units; its text bounds are checked in the final
Unity integration. Gameplay methods and input did not change after the manual pass.

Updated README, Design, Handoff, TeamWorkflow, AdventureValidation, VRValidation,
AssetRegister, selection status, source hashes and poster generation record in English.
No headset or APK validation is claimed. Existing XR/package/project-setting changes
and .cursor data remain outside this contribution. Publication target is Fexas only.
Final integration passed, including the new-target viewport-margin check. Trimmed
generated metadata whitespace. Git attributes preserve original FBX/licence bytes
without normalizing vendor formatting; approved asset checksums remain unchanged.

## Session 09 - Three autonomous improvement cycles (2026-10-18)
User requested three completed cycles, prioritising playability, humour, interaction
and content. Optional algorithm quizzes are now explicitly allowed. No further opinion
check is needed. Continue on Fexas; preserve unrelated XR/packages/settings and .cursor.
Cycle 1: richer campus surfaces/details, branch-mounted connection apparatus, varied
funding targets. Validate, inspect the result, then choose cycle 2 from that evidence.
Use approved art and project-authored geometry/shaders; no additional external imports.

### Cycle 1 completed
Added procedural campus surfaces, rain-dark paving, warm windows, humorous notices,
an open three-anchor workbench and varied 3/5/7/4/6 funding targets. Logic passed
40 seeds. Fixed the Standard shader output signature after visual inspection exposed
a white fallback. Final Unity integration PASS at 00:52 UTC; real workbench drag and
classroom reset passed. Evidence and the cycle 2 rationale are in ImprovementCycles.


### Cycle 2 implementation
The cycle 1 review identified weak explanatory feedback and passive classroom props.
Added optional, retryable quizzes with tree-derived answers, non-farmable stamps,
coffee/form interactions, horse nods and humorous prompts. No quiz gates the climb.
Logic tests passed for phase guards, skip, wrong/correct answers, replay prevention,
unchanged physical progress and actual ancestor answers across forty seeds. UI checks
and complete integration are next. No new external asset was imported.

### Cycle 2 completed / Cycle 3 scope
Cycle 2 full integration PASS at 00:59 UTC. Actual quiz wrong/correct/back flow and
coffee/form clicks passed; raised quiz exit for margin. Cycle 3 now adds continued
route research, fair decision-efficiency feedback and replay progression. No automatic
climb, forced quiz gate, new topology or second-game transition is permitted.

### Cycle 3 implementation and layout repair
Added optional research/cancellation with real adjacent prerequisites, finite unique
connections, mission-start decision benchmarks and efficient-recovery counts. Forty
seed logic tests passed, including exhaustive benchmark comparison and research
exhaustion. Runtime integration passed the new gameplay flow but found the results
heading too close to the upper viewport edge. Lowered the heading, moved the lower
button row upward, and shortened the non-optimal route message. Final rerun follows.

### Cycle 3 completed
Final Unity integration PASS at 01:07 UTC. Full actual mouse walkthrough passed
through two funding searches, five learned connections, optional research cancellation,
quiz retry/stamp and clean classroom replay at 01:14:47 UTC. Stopped Play afterward.
Saved ThreeCyclesManual-2026-10-18.log, all three smoke reports and six selected
screenshots. Updated README, Design, Handoff, AssetRegister, VRValidation and
AdventureValidation. No new external/generated media or package dependency was added.

Publication scope: gameplay/controller/world/state, editor validation, logic tests,
CampusSurface shader/meta and English documentation/evidence. Preserve and exclude
the existing XR/package/project-setting edits and .cursor. Destination remains Fexas;
remote Fexas was 495c764 and main was 15279ec before publication. No force push.
