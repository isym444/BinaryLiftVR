# Proposed revision: classroom opening and original-tree climbing

Status: approved and implemented in Session 06 (2026-10-18). This document preserves
the proposal; README, Design and Handoff describe the current implementation. The
storyboard is a planning illustration, not a game screenshot. The classroom uses
code-displaying monitors and computing signage rather than a separate algorithm board.

## Requested experience
1. Start inside a University of Glasgow-inspired classroom. Include desks, monitors,
   an algorithm board, a visible courtyard exit and ordinary environmental movement.
   Allow exploration before the story starts. Leaving the room and approaching the
   courtyard encounter triggers the theft; spawning or touching the notebook alone
   must not start the chase.
2. Retain and detail the campus chase. Add readable monkey anticipation, reaching,
   theft, turn, running, cornering and looking-back animation. Continue the everyday
   electronic-city treatment: sandstone, restrained cyan guidance and orange cues.
3. Reveal the pursuit layout from above. Design connected paths and junctions whose
   topology matches the following tree. Highlight the traveled route; simplify roads
   into edges and junctions into nodes, then transition into the next scene.
4. Restore the original playable tree scene, its branching platforms and valid
   1/2/4/8-level selection. Replace the current separate two-trial adventure course.
   Selecting a route highlights its actual ancestor chain. The player then grips and
   pulls along that chain, including changes in branch direction. Release pauses.
   Selection must not teleport the player or autonomously complete the ascent.
5. Show live progress: CLIMBED 0 / 4, CLIMBED 1 / 4, and so on, plus remaining levels.
   Increase the count only when a node boundary is actually crossed. Fractional motion
   may update a progress bar; it must not count as an additional completed level.
6. Finish once with results, replay and return-to-start. Remove the user-facing link
   to a second original-race experience. The previous ORIGINAL RACE entry was an
   intentional earlier implementation choice but conflicts with this revised flow.

## Interpretation to confirm
"Climb yourself after choosing" is interpreted as player-driven grip/pull movement,
not automatic traversal. Longer ancestor selections reduce route decisions and stops;
the preview does not promise reduced physical distance or faster automatic motion.
Existing horse companion and cone collection remain part of the story.

## Camera proposal for VR
Desktop can use a rising camera and overhead reveal. In a headset, propose a brief
fade to a stationary elevated observation position with free head tracking, then a
fade into the tree scene. Do not animate the headset's pitch or force it to look down.
This adaptation requires user agreement and later headset comfort validation.

## Preview and scope
Previews/revised-experience-storyboard.png is a code-drawn schematic storyboard,
not a game screenshot, render-quality target, imported asset or generated media asset.
Previews/draw_storyboard.py contains the reproducible drawing source. It uses geometric
shapes and an installed system font; no external images or models were downloaded.
The schematic campus plan does not claim geographic accuracy.
All game text, source and project documentation must remain English.
External assets still require free-license/source/preview approval before import.

## Implementation acceptance checks
- Spawn, explore and leave the classroom before the theft can trigger.
- Trigger theft once; show the complete pursuit and overhead transformation.
- Preserve correspondence between path junctions and the playable tree nodes.
- Choose a valid ancestor span, grip, pull, release, regrip and reach each boundary.
- Compare the live counter against actual crossed nodes, including a multi-edge route.
- Confirm there is one end state with no link into a second legacy game.
- Re-run desktop input playthrough and logic/integration checks; report headset tests
  separately. Update development log, README and handoff after implementation.
