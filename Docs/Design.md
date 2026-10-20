# Design - Funding, cones and remembered branches

## Continuous journey
Keep the classroom, proximity-triggered theft, seven chase markers, aerial campus-tree
reveal and original generated tree. The Highland calf steals a fictional research funding
envelope. English humour stays in brief flavour lines and decorations. There are no
quiz gates, automatic ascents or transitions into the historical race game.

## Small learning loop
1. Start with one-level exploration. Choose an edge, grip an orange branch hold and
   pull downward. Each actual node crossing updates the live count. Partial movement
   does not count a level; release to rest without falling.
2. After two edges, collect a cone. A connection workshop shows the actual start,
   middle and endpoint node IDs. Drag the orange plug to the cyan socket, or select
   those endpoints consecutively with controller rays. Two remembered one-edge paths
   become a two-edge connection for that specific start node.
3. Explore the next two edges and collect a second cone. Build the second two-edge
   connection, then compose both adjacent connections into four. Their shared middle
   ancestor is visible; composition animates before the next stage opens.
4. The funding moves to the original generated target. A deliberate button starts a
   fade back to the same tree's starting node. Memory and collected cones remain.
5. Select the stored four-edge connection. Physically climb all four edges in one
   selection; then explore any remaining edges. With the default Mono seed, this is
   4 + 1 + 1 to the six-level target. Collect the third cone to recover the funding.
6. Results show actual route decisions, levels, cones and reused connections. New
   Target keeps memory and cycles through 3/5/7/4/6-level goals after the original
   target; Replay from Classroom
   resets everything. Both remain in AdventureController.

## Algorithm contract

After a recovery, RECORD NEW ROUTES offers unrecorded connections whose adjacent
halves are already known. Construction is optional and cancellable. Newly explored
edges can therefore become twos and then fours throughout subsequent searches.
Results compare decisions with the minimum possible using memory available at the
start of that search. Later research does not retroactively change the rating.
This rewards applying knowledge without a timer or a physical climbing shortcut.

OPTIONAL QUIZ offers three rotating audit topics: composition, avoiding overshoot,
and actual ancestors. Wrong answers explain and allow retry. Skip always works.
One correct answer earns one stamp per search; reopening cannot duplicate it.
Quizzes never replace exploration, movement or cone collection.
Connections are indexed by start node and power-of-two span. A two-edge connection
requires both observed parent edges; a four-edge connection requires both adjacent
stored two-edge connections. This is binary lifting, not binary search. Connections
cannot be used at unrelated nodes or beyond the target. This slice stops at four.

TreeGenerator.Generate(13,16,18) and TreeLayout.ComputeX(tree,1.3) retain the original
35-node topology and coordinates. RaceState supplies the original start and final goal;
exploration temporarily targets the ancestor four edges above the same start.

Each physical edge still requires 0.65 units of accepted downward input. The first
four-level exploration uses four route decisions. A later four-level selection uses
one decision but the same physical distance and effort. No claim of reduced physical
climbing or teleportation-based speed-up is made. Unassisted exploration always remains
available, and taking it is not penalized as a wrong answer.

## Presentation
Approved Poly Haven masonry, selected Kenney desks/chairs, and one preview-approved AI
poster enrich the existing classroom/campus. Source files and licences are tracked in
AssetRegister.md. The Highland calf uses the approved adapted Quaternius rig. Horse remains a project-authored placeholder; external horse
review is pending download availability. No university photograph is imported.

VR retains tracked rotation, manual grip input, stationary faded map/target changes,
no forced camera pitch or roll, and no head bob. Mouse drag is the desktop analogue.
Actual desktop input and method-driven integration are separate validation categories;
headset comfort, reach, stereo, haptics and device performance remain hardware checks.

## Optional hands-on campus life (2026-10-18)
CampusLifeState is separate from AdventureState. Coffee and archive tasks never unlock
climbing, award cones, reveal stored connections or gate the classroom exit.
Coffee: move empty cup to brewer, press BREW, watch a 2.5-second fill/stream, then serve
on the saucer. A full cup shows steam. Stamp: press the movable stamp onto the form;
move the visibly stamped form into the in tray; pull the now-unlocked drawer and take
the route card. The card displays the current tree's start, parent and grandparent.
It suggests composition but still requires observing and constructing the actual edges.

Desktop uses tabletop-plane dragging with forgiving docking radii. Drawer drag is downward
on screen; VR uses the hand's physical pull toward the player. VR props require a grip
within 12 cm; BREW and the route card also accept trigger rays. Invalid releases return
to the last valid dock. No thrown props or loose-object physics are simulated. Holding
pauses locomotion and snap turns; focus/tracking loss cancels safely. Reset clears both
state machines. Only Arrival advances brewing or accepts campus interaction input.

The middle desk row is removed to create an approach aisle; four classroom desks remain.
A 1.2-metre centre gap keeps the exit route open. Authored movement bounds reject the
worktops and remaining desk rows. Real seated/standing controller reach remains untested.
