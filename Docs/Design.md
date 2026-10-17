# Design - Classroom to Canopy

## Continuous journey
1. Explore a computing classroom with desks, screens, lecture notes and cups. A lit
   automatic door opens as the player approaches. Exit first; theft only triggers in
   the courtyard near the notebook. Optional click-to-walk is an accessibility fallback.
2. The monkey anticipates, reaches, takes the notebook, turns and runs. Body bounce,
   arm swing, eased turns and head movement animate the pursuit through the cloisters.
   Every checkpoint corresponds to an ancestor on the same tree used for climbing.
3. Desktop camera rises to show the campus routes. The graph expands and lifts from
   the ground before a fade into the tree. VR uses a faded stationary overlook with
   head tracking retained, not forced aerial camera motion. Geometry is a stylized
   University of Glasgow-inspired setting, not an accurate campus survey.
4. The original 35-node tree has the original parent array, horizontal layout and
   one-metre vertical spacing. Start and goal come from the original race generator.
5. At the goal, collect one cone, show it on the companion horse and display results.
   Replay returns to a clean classroom. No action starts the legacy race.

## Prepare, contact, exert, finish
Hover 1/2/4/8 to preview real ancestors; choose an available connection. Orange branch
holds appear along every edge of that path. Grip a reachable hold and pull downward;
movement follows the edge's horizontal and vertical direction. Desktop uses a downward
mouse drag. Release or stay still to stop; a support deck travels beneath the player.
The gloved hand stays at its contact point. Tracking discontinuities above 0.20 m are
rejected. Two hands average their input to prevent double-speed ascent.

Each edge needs 0.65 m accumulated downward input. Partial edges do not increment the
level count. Node crossings update the total and the selected-route count, with a
haptic cue on supported hardware. Finish the chosen span before choosing again.
Larger binary-lifting selections reduce planning decisions and stops while retaining
physical traversal. They do not teleport or remove physical distance. Connections
that overshoot the gold target are visibly disabled. Seed 13 provides six levels;
eight-level choices are covered by logic tests on longer generated routes.

## Presentation and validation boundary
Sandstone arches, windows, planters, street furniture, classroom props, cyan route
lighting and orange interaction cues use approved code-built primitive models.
No AI media, imported audio or external assets. Character animation is a procedural
placeholder; final rigged characters require separately approved free assets.
Desktop input validation and method-driven integration are recorded separately.
Headset comfort, stereo, physical reach and performance remain hardware checks.
