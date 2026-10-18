# Highland calf adaptation review

Local character review only. Nothing here loads in Unity or replaces the monkey.
User approved adapting this free base for a second preview on 2026-10-18. The final
appearance still needs their approval before game integration.

Base: Quaternius, Ultimate Animated Animals, Bull.gltf, CC0.
https://quaternius.com/packs/ultimateanimatedanimals.html
Creator's public download folder:
https://drive.google.com/drive/folders/1uJ3N5HfB7jKTseJUNQr3N4YaN0UuEtHk
Public file: 1M9gyr2UikIDW_Ynp-OfgiK-JjI7ziPd3. No authentication is needed.
SHA-256: 535DA2992EB125D517725159A2EDDEB520B72595BCB3021985FC43D2C84BFF76

The original has 2,418 triangles, 42 joints and 13 animation clips. Added coat meshes
are bound to head, neck and torso bones. Changes: warm coat palette, compact overall
proportions, enlarged head, shorter/wider muzzle, new short curved horns, forehead,
cheek, chest and flank locks. Source clips are unchanged. This is an art direction
study, not final skinning, groom, LODs, export or Unity validation. No AI imagery.

Run Prepare.ps1 from this folder or the project root. It downloads the public base
and pinned Three.js 0.160.1 (MIT) into ignored Logs/AssetReview/CalfPreview, verifies
the base hash, and copies the authored viewer. With Python installed, serve only
that folder: python -m http.server 8766 --bind 127.0.0.1 --directory Logs/AssetReview/CalfPreview
Open http://127.0.0.1:8766 in a browser. Rotate/zoom with the mouse, choose Idle,
Walk, Gallop or Eating, and compare Front/Side/Three-quarter views. No upload occurs.

Observed preview: source clips play, new head/neck coat follows the eating pose, legs
move in Walk/Gallop, and camera/pause controls work. Full-frame clipping, foot contact,
all 13 clips, avatar export and target-headset performance still require integration QA.
The calf's enlarged head and lock density should receive art review before import.

Saved review captures: [three-quarter](../../Docs/Evidence/HighlandCalf-Preview-2026-10-18.png)
and [front](../../Docs/Evidence/HighlandCalf-Front-2026-10-18.png). These are actual
browser renders of the animated study, not in-game screenshots.
