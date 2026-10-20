# Campus detail validation - 2026-10-18

## Scope and environment
Unity 6000.0.85f1, Windows desktop Play Mode. Fexas baseline 65b4094.
No headset was connected for this check. Frame time, stereo, mobile GPU cost,
tracked reach and real controller input are NOT validated by desktop results.

## Executed checks
- Scripts/Test-Logic.ps1: ALL PASSED.
- Unity compiled the added CampusDetail component and refreshed the scene.
- Actual mouse input: cup moved from its starting dock to the brewer; BREW triggered;
  liquid filled; the full cup was dragged onto its saucer. Visible hints confirmed
  serving. CampusDetailManual-2026-10-18.log records the input/brew/placement events.
- Actual classroom exit initiated walking, then the funding theft and Chase phase.
  New facade frames, clock, banners and water were visible without blocking the aisle.
- Reviewed coffee-machine housing, gauge and tray in Play Mode. Initial long hints
  overlapped the poster; shortened and split them, then inspected final layout after
  regression replay. No input/collider changes were needed.
- BinaryLift > Validate Adventure: PASS, 138 assertions. Includes all seven chase
  checkpoints, original 35-node tree, stationary-selection guards, manual-pull logic,
  cone collection, two/four-edge memory, new targets, quiz/research, results and clean
  replay. Report: CampusDetailSmoke-2026-10-18.txt (09:31:28 UTC).
  This is a method-driven regression, not an actual mouse traversal of every branch.
- Scoped git diff --check passed for edited game source and development log.
  Existing Unity-generated asset metadata whitespace is outside this change.

## Evidence
- Evidence/Adventure-Chase-092912.png: actual desktop courtyard, stone frames and clock.
- Evidence/Adventure-Arrival-092847.png: actual served coffee before hint wrapping.
  The final hint copy is shorter; no other geometry in this view changed.
- The available desktop automation exposes screenshots but no RecordScreen tool;
  no video or headset playthrough is claimed.

## Remaining art and technical limits
This is a finishing pass on authored geometry, not a claim of final bespoke art for
every object. Horse, monkey, main tree shape and major architecture remain authored
prototype forms. The approved materials remain unchanged. Custom stone/trim meshes
are batched by material, but no on-device draw-call or shadow budget was measured.
Clock and banners are cosmetic, and six rain rings reuse fixed objects. No new
particle systems, cameras, packages, external textures or AI images were introduced.

## Character research boundary
Hamish was approved then deferred by the user because of the required account flow.
No Hamish file was imported. An alternative Quaternius Bull glTF was retrieved from
the creator's public Drive without authentication for inspection only, outside Assets.
It has 13 animation clips, 42 joints and 2,418 triangles. It is a generic bull, not
a finished Highland calf. The user approved producing an adaptation preview, which
is now available in Tools/CalfPreview. Browser inspection covered Idle, Walk, Gallop
and Eating, including head-following coat locks; no browser warnings/errors appeared.
Prepare.ps1 and the JavaScript syntax check passed. Appearance approval and Unity
integration remain pending. Review captures: Evidence/HighlandCalf-Preview-2026-10-18.png
and Evidence/HighlandCalf-Front-2026-10-18.png.
