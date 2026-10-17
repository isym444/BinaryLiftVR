# Binary Lift VR (MVP)

Race an NPC to the K-th ancestor of a tree. The NPC walks parent by parent (K steps);
you can teleport through power-of-two portals (2^j-th ancestor) -- use the set bits of K.

## Build and run
1. Open the project in Unity 6000.0.85f1 (Unity Hub > Add project from disk; licence must be active).
2. Menu **BinaryLift > Configure Project**: creates the empty scene, sets Android/IL2CPP/ARM64/Vulkan,
   and enables OpenXR + Meta Quest Support + Touch controller profile for Android.
3. **File > Build Profiles**: Android, Run Device = your Quest (developer mode on, USB debugging allowed).
   Or from a terminal: `Unity -batchmode -nographics -quit -projectPath . -buildTarget Android -executeMethod BinaryLift.EditorTools.BinaryLiftSetup.BuildAndroid`
   (writes Builds/BinaryLift.apk; install with `adb install -r Builds/BinaryLift.apk`).

## Controls
Point with the right controller (cyan line) or just look at a target (white dot).
Trigger / A / X selects. B / Y restarts. In the editor (no headset): mouse click selects, hold right mouse to look, R restarts.
If the controller ray points the wrong way, change `XRRig.PointerPitch`.

## Logic tests (no Unity needed)
    cd LogicTests && dotnet run
