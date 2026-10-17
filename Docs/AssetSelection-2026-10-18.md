# Campus asset selection - 2026-10-18

Status: proposal for user review; no asset import or gameplay changes in this step.
The user requested asset selection before the approved learning-loop implementation.
All individual external assets below are free; no paid bundle or subscription is needed.

## Art direction and real campus references

Retain the current scene sequence and original tree. Refine a teaching interior,
East Quad courtyard and Cloisters-inspired chase with sandstone, wet paving, everyday
furniture, restrained cyan illumination and orange interaction cues. This is a stylized
campus-inspired route, not a surveyed reconstruction of rooms or circulation.

- [Official Quadrangles reference and photograph](https://www.gla.ac.uk/explore/visit/campustours/prospectivestudents/studentsselfguided/quadrangles/).
  Hunter Halls on the north side of East Quad are used for teaching and examinations.
- [Official Cloisters reference and photographs](https://www.gla.ac.uk/explore/visit/campustours/prospectivestudents/studentsselfguided/cloisters/).
  The Cloisters connect the two quadrangles; use the columns and ribbed vault as references.
- These photographs and the user's statue photo are reference-only. No reproduction
  permission is assumed, no university image or logo is proposed for game import.
- Campus meshes remain project-authored. A generic city kit is not the primary campus solution.

## Proposed free assets

### Sandstone Blocks 08 - Poly Haven
- Author: Rob Tuytel. Licence: CC0. Cost: zero.
- [Official source, preview and downloads](https://polyhaven.com/a/sandstone_blocks_08).
- Proposed use: masonry on existing campus meshes, with tuned colour and texture scale.
- [Publisher AI policy](https://blog.polyhaven.com/ai-and-poly-haven/) states that assets
  are not made with AI. This is a material, not a scan of the university.
- Start evaluation with 1K maps; inspect seams and VR readability before choosing 2K.
  Do not add displacement geometry or 8K textures by default.
- Status: awaiting user approval; shader integration and headset performance untested.

### Furniture Kit - Kenney
- Author/publisher: Kenney. Licence: CC0. Cost: zero for the individual pack.
- [Official source and visual preview](https://kenney.nl/assets/furniture-kit).
- Source lists 140 files and a 2018 release. Proposed use: selected desks, tables and
  chairs for the teaching interior after inspection of the actual archive.
- Kenney's [publisher bundle page](https://kenney.itch.io/kenney-game-assets) declares
  no generative AI use. The paid bundle is not needed; use the free individual pack.
- Status: awaiting user approval; individual mesh names, materials and scale uninspected.

### Ultimate Animated Animal Pack - Quaternius
- Author/publisher: Quaternius. Licence: CC0. Cost: zero for this pack.
- [Official source and visual preview](https://quaternius.com/packs/ultimateanimatedanimals.html).
- Source lists 12 animals, more than 12 animations per animal and FBX/OBJ/Blend/glTF;
  dated July 2021. The publisher's catalogue includes horse-tagged animal content.
- Proposed evaluation: a horse companion with the project's orange cone attached.
  Confirm the exact horse mesh and clips in the archive before committing to replacement.
- Publication date and author credit are provenance evidence, not an independent
  audit of every file or a blanket no-AI guarantee.
- Status: pack candidate only; no rig, clips, geometry or Unity compatibility tested.
  A suitable monkey has not been selected. Keep the current monkey until a reviewed
  replacement is approved; do not claim that this pack contains one.

## Small amount of AI media

The user explicitly permitted limited AI-generated assets during this selection step.
Recommended initial scope: one or two optional decorative campus poster illustrations.
Keep important labels, algorithm diagrams and interaction prompts as editable English
text or code-authored geometry. Major architecture and characters use the selections
above or existing project meshes. This scope is a recommendation, not an agreed quota.

No generated candidate exists yet. Before import, present its preview, intended use,
generation tool, terms and any cost; record prompt, date, edits and user approval.
Do not represent generated artwork as a photograph or university-issued material.

## Funding story and proposed English copy

Replace the notebook with a fictional research funding envelope or small grant case.
Use project geometry and editable labels; an external model or generated image is
not needed for this prop. The monkey steals the grant paperwork, and recovered cones
mark exploration progress and remembered connections. No gameplay change is made yet.

- Case label: "RESEARCH FUND - NOT FOR BANANAS"
- Theft reaction: "Our funding has developed legs. Follow it!"
- Horse introduction: "Department of Unstable Funding. I handle fieldwork."
- Two-level connection: "Two steps remembered. Less paperwork next time."
- Four-level connection: "Two plus two. Finally, a budget that adds up."
- Recovery: "Funding recovered. Banana expenses rejected."

Humour belongs in short optional remarks and props. Climbing instructions and live
level counts stay direct, readable and unambiguous. This is fictional campus comedy.

## Approved next gameplay scope, after asset selection

Explore one level at a time, collect cones, establish a two-edge ancestor connection,
compose two adjacent two-edge connections into four edges, then change the target and
reuse applicable learned connections. Store connections per node, not as a universal
power-up. Preserve manual traversal of every edge and update counts only on crossings.
Connections must remain valid ancestor paths. Learning happens through exploration and
construction, without quiz gates. Preserve the single ending and original tree structure.

Implementation must include pure logic checks and an actual desktop input walkthrough;
headset validation must be reported separately. Update English logs and handoff, then
publish only to Fexas. The current gameplay still has the previous single-cone flow.
