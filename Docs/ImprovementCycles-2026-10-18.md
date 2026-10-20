# Three improvement cycles - 2026-10-18

## Brief and baseline
User requested three uninterrupted cycles, with playability, humour, interaction and
content as the evaluation criteria. Small algorithm quizzes are explicitly allowed.
Baseline: Fexas 495c764, original tree and manual climb. Existing unrelated package,
XR and project-setting changes remain local. No new external or generated media.

## Cycle 1 - A place to explore and routes worth remembering
Changes: project-authored procedural wood/paving shader, rain-dark patches, warm
occupied windows, lamps and campus noticeboards. Existing approved sandstone remains.
The memory workshop is now an open branch-mounted apparatus with a shared middle
anchor, start/via/end labels, a three-point animated cable and markers on real nodes.
After the original funding goal, target distances cycle through 3, 5, 7, 4 and 6.
Each has a fictional campus budget name; oversized connections stay unavailable.

Validation: pure logic passed across 40 seeds, including all five target distances,
memory preservation, ancestry validity and unchanged manual effort. First complete
Unity integration passed. Visual inspection found an overbright floor; explicit
surface colour/smoothness initialisation was added. A shader compilation assertion
then caught the actual cause: a Standard lighting function used SurfaceOutput instead
of SurfaceOutputStandard. Corrected the signature; final checks include shader errors.
Further evidence and the next-cycle decision are recorded below as work completes.

Focused real-input check: prepared at the first workbench using the editor fixture,
then dragged orange to cyan at 00:50:59 UTC. The node-specific two-edge connection
was built and choices returned. R reset to the classroom at 00:51:08. Verified the
corrected wood surface visually. Evidence: Adventure-Weaving-005053.png and
Adventure-Arrival-005118.png. The fixture's earlier climbing was method-driven.

Review decision: the apparatus is clearer, but the game still gives little explanatory
feedback and the classroom props do not respond. Cycle 2 will add optional auditing
questions about composition, remaining distance and actual ancestors, with explanatory
retry and no climb gate. Coffee, paperwork and companion interactions will add playful
responses. Keep the main loop available when the player skips every question.

## Validation categories
Method-driven integration exercises the game runtime but is not real input. The
editor's Prepare Workshop Input Check establishes a clearly labelled fixture before
manual clicks/drags. It does not itself count as a successful input walkthrough.
No headset is connected; stereo, reach, comfort and device performance remain NOT RUN.

Cycle 1 final integration: PASS at 00:52 UTC, including shader compilation, full flow
and replay. See Cycle1-Smoke-2026-10-18.txt. Cycle 1 is complete.


## Cycle 2 - Optional audits and playful responses
Implemented three rotating quiz topics: compose adjacent twos, avoid overshooting
three remaining levels, and identify an actual two-edge ancestor in the current tree.
Quiz is available only after funding recovery, can be skipped, explains wrong answers,
allows retry, and grants at most one audit stamp per search. It never changes cones,
climb distance or movement. Reopening cannot farm stamps. Forty-seed tests pass.
Coffee and paperwork in the classroom now respond with brief animation and humour.
The horse nods on cone recovery or greeting. Functional instructions stay explicit.
Added an editor results fixture for isolated real-input checks; its setup is labelled
method-driven and does not substitute for the complete integration test.

Cycle 2 actual input: at the prepared results, opened OPTIONAL QUIZ, deliberately
answered eight, read the explanation, retried four, earned one stamp and returned
at 00:56:52 UTC. Replay cleared progress. Coffee and paperwork responded to actual
clicks at 00:57:16 and 00:57:23. Screenshot: Adventure-Quiz-005645.png.
The quiz return button was near the viewport edge, so it was moved upward 0.10 local
units before final integration. Next-cycle review: stored routes still stop growing
after the tutorial. Add optional construction of newly explored connections and a
fair route-efficiency result based on the player's actual available memory.


Cycle 2 completed: final integration PASS at 00:59 UTC, including quiz viewport bounds,
wrong-answer retry, single stamp, return and clean replay. Actual quiz and prop inputs
passed separately. See Cycle2-Smoke-2026-10-18.txt.

## Cycle 3 - Continued route research and meaningful replay
Review of cycle 2: quizzes explain the algorithm, but route knowledge stops expanding
after the tutorial. Add optional post-search construction from physically observed
edges, compose further fours, compare route decisions against the actual available
memory at search start, and show progression without time pressure. Keep construction
optional, ensure each connection has real prerequisites, and retain manual climbing.

Cycle 3 implementation: post-search RECORD NEW ROUTES offers only constructible,
unrecorded connections. It prefers a composable four, otherwise the highest explored
pair. Each requires two adjacent known halves, can be cancelled, and returns to the
same results. A finite per-node connection set prevents duplicate rewards. Scoring
freezes the minimum route-decision count at mission start using dynamic programming;
tests compare it against exhaustive route enumeration. Further research cannot change
past ratings. The test suite passed all forty seeds, cancellation, exhausted research,
unchanged climb effort/cones, and the new six-level 4+2 possibility.

Final cycle 3 integration PASS at 01:07 UTC. A viewport assertion caught the results
heading at the top margin; lowered it and reran the entire integration successfully.
The successful report is Cycle3-Smoke-2026-10-18.txt. It includes shader compilation,
quiz retry, research cancellation/building, immutable historical rating, repeat target,
manual-effort invariants, one controller and clean replay. Actual input follows below.

Cycle 3 actual input passed from a clean classroom through chase/map, four discovery
edges, two/two/four composition, six-level 4+1+1 funding recovery, optional research
cancel/reopen, another two and four, three-level 2+1 search, quiz wrong/correct/back,
and clean replay at 01:14:47 UTC. See ThreeCyclesManual-2026-10-18.log and the three
011xxx screenshots listed in AdventureValidation.md. No gameplay methods were used
during this manual segment. The integration runner only established the clean start.

Final review: the three requested cycles are complete. Exploration now builds a
growing, reusable memory library; varied target lengths demand different combinations;
optional audits explain those choices without blocking play. Campus interactions and
surface detail support the setting. Further priorities are hardware reach/readability,
performance profiling, and approved character art. These are documented follow-ups,
not claims of completed headset testing or final production art.
