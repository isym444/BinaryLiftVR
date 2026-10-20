# Highland calf integration validation - 2026-10-18

## Completed
- Pure algorithm/progression tests: Scripts/Test-Logic.ps1, ALL PASSED.
- Unity 6000.0.85f1 imported the native calf and compiled the coat shader.
- Native asset checks: 18 PASS assertions, including all 13 clips sampled at eight
  points for finite skin deformation within bounds. 42 bones, 7,594 triangles,
  one skinned mesh/material. Report: HighlandCalfAssetChecks-2026-10-18.txt.
- Actual desktop input before the user stopped computer use: clicked classroom exit,
  observed the revised calf poster, exited, triggered funding theft, observed the
  animated calf and FOLLOW THE CALF copy, then clicked the first chase beacon and
  reached checkpoint 0. Evidence: HighlandCalfManual-2026-10-18.log.
- Final game and editor C# sources compiled successfully using Unity's Mono compiler
  and its existing reference response files, with outputs redirected to ignored Logs.
  This check did not operate the editor or change the running game.
- JavaScript syntax checks passed for the shared adaptation and preview.
- Runtime source scan found no remaining monkey/banana references. Historical logs
  and original generation prompts retain their historical wording deliberately.

## Validation boundary
The user stopped computer use and then explicitly requested no further desktop
control/testing, asking for checks and direct publication. No further UI automation
was performed. The complete post-integration chase/climb/replay walkthrough and
updated AdventureValidation regression were NOT run. Existing gameplay logic is
unchanged apart from calf-themed strings; the prior campus regression is not evidence
that the new actor passed a complete run. Headset/controller, stereo, comfort and
GPU frame-time checks were NOT run. Final face-player placement was compile-checked.

## Implementation and review
The importer combines original skin and bone-attached coat into native Unity data.
A shader lookup issue during the first import was corrected to an explicit asset
reference and the import/animation checks then passed. The calf idles when stopped,
plays locomotion during pursuit, carries funding on a head-bound mouth socket and
turns to the player at tree destinations. Recovery hides carried funding; new searches
restore it; replay resets pose and funding. AdventureValidation now contains checks
for these lifecycle transitions for the next permitted full regression.

## Reproduction
Tools/CalfPreview/Prepare.ps1, then node Tools/CalfPreview/Export.mjs.
Unity: BinaryLift > Validate Highland Calf; BinaryLift > Validate Adventure.
The native asset is committed, so normal game loading does not require Node or a
network connection. Source provenance is in SourceArt/HighlandCalf/README.md.
