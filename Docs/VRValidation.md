# VR validation - Classroom to Canopy

Status: NOT RUN ON HEADSET. Desktop success does not establish headset readiness.
Record device model, runtime, controller type, build, date and tester; no serials/accounts.

## Entry and locomotion
- Build and launch on the intended device after installing matching Android modules.
- Confirm OpenXR/controller profile, both-eye rendering, floor origin and seated height.
- Explore the classroom, open the door, approach the notebook and trigger theft once.
- Complete all seven checkpoints with left-stick movement and with optional ray walking.
- Check clear routes around desks and the exit; the prototype is not a room-scale collision solver.

## Transitions and comfort
- At the aerial reveal, verify fade coverage and a stationary overlook with head rotation
  and existing snap yaw retained. Looking down must reveal the traced campus graph.
- Check the tree overview and fade to the climbing start without forced pitch or roll.
- Verify no camera bob, involuntary turning or falling when input stops.
- Assess the head-following instruction panel at 0.4 m and all close grip distances.
  Move or redesign the panel if uncomfortable; current depth is not hardware-approved.

## Manual climbing and collection
- Select each available 1/2/4/8 connection. Selecting alone must not move the player.
- Seed 13 has six target levels; use a longer seed in a test harness to exercise eight.
- Reach and grip each rounded orange hold with either hand, then pull downward.
- Alternate hands, use both hands, pause and release. Two hands must not double speed.
- Verify motion follows the actual branch direction, including sideways connections.
- Cross every node once; total and selected-route counters must agree with the graph.
- Lose tracking or introduce a pose discontinuity: release safely without extra progress.
- Check physical reach while seated, glove contact, personal space and haptic comfort.
- Collect the final cone by nearby grip and by trigger ray in separate runs.
- Results must contain only replay; no legacy controller or second game may start.
- Replay/reset must clear counters, trophy, grips and route reveal, returning to classroom.

## Performance and production gaps
- Profile CPU/GPU, draw calls, memory and thermals on the actual standalone headset.
- Inspect the full campus/35-node overview and dense branch holds as worst-case views.
- Repeat ten minutes of climbing and restarts; check object/material counts.
- No device performance claim, verified APK, audio, persistent save, guardian integration
  or final character rigs is included. All external art/audio needs separate approval.
