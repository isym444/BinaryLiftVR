# Asset register

Last reviewed: 2026-10-18. The user explicitly approved sandstone and furniture, then
approved the generated funding poster after seeing its preview. Horse review continues.
Checksums and source byte sizes are recorded in ApprovedAssetHashes.json.

## Imported: Poly Haven Sandstone Blocks 08
- Author: Rob Tuytel. Cost: zero. Licence: CC0 1.0.
- Source/preview: https://polyhaven.com/a/sandstone_blocks_08
- File: Assets/Resources/Approved/Sandstone.jpg (original 1K diffuse map).
- Download: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/1k/sandstone_blocks_08/sandstone_blocks_08_diff_1k.jpg
- Publisher licence: https://polyhaven.com/license
- Legal text: Assets/ThirdParty/PolyHaven/CC0-1.0.txt, preserved from the SPDX licence
  repository: https://raw.githubusercontent.com/spdx/license-list-data/main/text/CC0-1.0.txt
- Approval: user approved this asset on 2026-10-18 after the linked source/preview review.
- Use: project-built campus masonry, world-space UV shader at a three-metre repeat.
- Changes: no source image edits; Unity mipmaps and compression, maximum 1024 pixels.
- No displacement or high-resolution maps. No headset performance claim.
- Provenance: Poly Haven publishes a no-AI asset policy; this is not a campus scan.

## Imported: Kenney Furniture Kit
- Author: Kenney. Cost: zero. Licence: CC0; no attribution required, credited here.
- Source/preview: https://kenney.nl/assets/furniture-kit
- Download: https://kenney.nl/media/pages/assets/furniture-kit/440e0608a4-1677580847/kenney_furniture-kit.zip
- Archive licence identifies Furniture Kit 2.0, creation date 2018-10-20. The website
  lists a 2018 release; the archive version is recorded instead of assuming it is 1.0.
- Files: desk.fbx and chairDesk.fbx only, from the archive's Models/FBX format directory.
- Retained licence: Assets/ThirdParty/Kenney/License.txt, unchanged.
- Approval: user approved the furniture pack on 2026-10-18.
- Use: four classroom desks and four desk chairs (middle row removed for campus tasks). Project-authored solid-colour finishes,
  fitted to the existing furniture footprint; original model files unchanged.
- Import: no animation, generated colliders or embedded materials. No new package needed.
- Kenney's publisher bundle page declares no generative AI use. No paid bundle purchased.

## Imported: Research Fund poster (AI-generated)
- Tool: built-in image_gen, 2026-10-18; no external stock asset or university logo.
- File: Assets/Resources/Approved/FundingPoster.png. Original output retained unchanged.
- Cost: no external purchase; generated through the current session. No claim about
  account billing or exclusive copyright. Not a CC0 stock asset.
- Terms reference: https://openai.com/policies/terms-of-use/ (generation service terms;
  do not relabel this output as a third-party CC0 asset).
- User preview approval: explicit reply, "can use this poster", on 2026-10-18.
- Use: one framed decorative classroom notice. English gameplay prompts remain editable
  text, not baked into artwork. Unity maximum texture dimension 1024, mipmaps/compression.
- Prompt: see PosterGeneration-2026-10-18.md. No uploaded reference image was used.

## Horse candidate: review only
- Quaternius Ultimate Animated Animal Pack, July 2021, CC0, free individual download.
- Source: https://quaternius.com/packs/ultimateanimatedanimals.html
- Preview: https://quaternius.com/assets/images/fullres/ultimateanimatedanimal.jpg
- Publisher-linked public FBX directory confirms Horse.fbx and Horse_White.fbx,
  each approximately 3.6 MB, modified 2022-02-08.
- Horse.fbx download returned a Google Drive quota-exceeded page, not a valid FBX.
  No rig, geometry or animation clip inspection could be completed. No horse imported.
- Source advertises more than 12 animations per animal; actual clip compatibility and
  device cost remain unverified. No monkey appears in the FBX inventory inspected.
- Existing project-built horse/monkey remain. The supplied statue photo is reference-only.

## Remaining prototype art
Tree, cones, characters, envelope, architecture and other props are project-authored
Unity geometry. Current audio inventory: none; device haptics remain code-driven.
WorldText uses Unity's bundled LegacyRuntime font atlas. External city/sound packs from
the earlier shortlist were not approved or imported. University photographs remain
reference-only and were not copied into the repository.

## Future imports
External assets remain free and require source/preview/licence approval before import.
Limited AI assets are allowed, but each needs a labelled preview and approval. Preserve
source, licence, cost, author/tool, approval, modifications and hardware checks here.

## Project-authored surfaces added 2026-10-18

CampusSurface.shader supplies procedural wood grain and paving seams. Rain-dark
patches, warm windows, lamps, noticeboards and the branch apparatus use project
geometry/materials. These additions contain no new external or AI-generated media.
The shader uses Unity's built-in Standard lighting with SurfaceOutputStandard.
Existing approved sandstone, furniture and poster provenance above still applies.

## Imported: campus life expansion (2026-10-18)
The user approved these environment sources after preview, then prioritized their
integration while the animated Highland calf search continues. All cost zero.

- Kenney Food Kit, CC0: https://kenney.nl/assets/food-kit
  Imported unchanged FBX models: mug, cup-saucer, cookie, donut; original colormap.png.
  Use: movable coffee cup, serving saucer and decorative snacks. Runtime atlas material.
  Download: https://kenney.nl/media/pages/assets/food-kit/83086fa91c-1719418518/kenney_food-kit.zip
  Unchanged bundled licence: Assets/ThirdParty/Kenney/food/License.txt.
- Kenney Nature Kit, CC0: https://kenney.nl/assets/nature-kit
  Imported unchanged FBX: plant_bushDetailed, flower_purpleA, tree_oak.
  Use: courtyard planting and six trees. Embedded solid-colour materials retained.
  Download: https://kenney.nl/media/pages/assets/nature-kit/37ac38a37b-1677698939/kenney_nature-kit.zip
  Unchanged bundled licence: Assets/ThirdParty/Kenney/nature/License.txt.
- Previously approved Kenney Furniture Kit: additionally imported books, bookcaseOpen,
  lampRoundTable, pottedPlant and bench. Embedded materials retained for this subfolder.
  Use: shelves, desk dressing, entrance plants and courtyard seating.
  Unchanged bundled licence: Assets/ThirdParty/Kenney/furniture/License.txt.
- Poly Haven Bark Brown 02, Rob Tuytel, CC0: https://polyhaven.com/a/bark_brown_02
  Use: existing climbing-tree geometry, without changing its topology or holds.
- Poly Haven Wood Planks, Amal Kumar, CC0: https://polyhaven.com/a/wood_planks
  Use: classroom flooring, campus worktops and sliding drawer.
  Each Poly Haven material uses original 1K diffuse, OpenGL normal and roughness JPGs.
  Source URL pattern: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/1k/{id}/{id}_{map}_1k.jpg
  Map names are diff, nor_gl and rough. Local names omit the final _1k suffix.
  Publisher licence: https://polyhaven.com/license ; retained CC0 text remains in
  Assets/ThirdParty/PolyHaven/CC0-1.0.txt. No source image edits or AI generation.

All imported source files are under Assets/Resources/Approved. Unity enables mipmaps,
compression and a 1024-pixel cap; normal maps import as normals, roughness as linear
and repeatable surfaces as Repeat. CampusPBR uses all three maps under Standard lighting.
No claim of measured headset performance. The new model imports disable animations
and generated colliders; interaction colliders are deliberately authored in code.

## Highland calf: not imported
Appearance candidate: JessSwynn, Highland Cow, CC BY 4.0:
https://sketchfab.com/3d-models/highland-cow-63b31445ac7249d3a02e1242d0a9be15
User approved appearance for further inspection, not a final rigged replacement.
Public metadata reports 46,812 faces and zero animation clips. Official download
requires login (401); no authentication bypass attempted. Rig compatibility is unknown.
The user chose environment and campus interactions first, while searching for a free
animated calf. The current monkey therefore remains intentionally in this revision.
Quaternius Ultimate Animated Animals lists an animated generic cow/bull, but its shape
is not a shaggy Highland calf; it remains an unapproved alternative, not an imported
replacement. Paid rigged offers and unapproved AI/static cows were excluded. A suitable
free animated Highland calf has not yet been verified. Keep this decision open.
