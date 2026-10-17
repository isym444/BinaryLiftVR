using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BinaryLift
{
    public enum TargetKind { Platform, Portal }

    /// <summary>Marks a collider the player can point at.</summary>
    public sealed class HoverTarget : MonoBehaviour
    {
        public TargetKind Kind;
        public int Index;
    }

    /// <summary>
    /// Builds the whole game procedurally at runtime (no scene authoring needed):
    /// tree of platforms, power-of-two portals, the NPC, HUD and the race loop.
    /// </summary>
    public sealed class RaceController : MonoBehaviour
    {
        const float LevelHeight = 1.0f;
        const float Spacing = 1.3f;
        const float PlatformRadius = 0.45f;
        const float PlatformThickness = 0.1f;
        const float ViewDistance = 3f; // the player floats this far in front of the tree plane so the whole tree is visible
        const float NpcStepSeconds = 1.5f;
        const int CountdownSeconds = 3;
        const int TrunkLength = 16;
        const int ExtraNodes = 18;

        static readonly Color PlatformColor = new Color(0.35f, 0.45f, 0.62f);
        static readonly Color StartColor = new Color(0.3f, 0.8f, 0.4f);
        static readonly Color GoalColor = new Color(1f, 0.8f, 0.1f);
        static readonly Color PlayerColor = new Color(0.95f, 0.95f, 0.95f);
        static readonly Color HoverColor = new Color(0.3f, 0.9f, 1f);
        static readonly Color PreviewColor = new Color(1f, 0.25f, 0.85f);

        static Font font;
        static Shader litShader, unlitShader;

        XRRig rig;
        int seed;
        TreeModel tree;
        RaceState race;
        Vector3[] nodePos;

        Transform worldRoot;
        Material[] platformMats;
        Transform portalRoot;
        Transform[] portalHolders;
        Material[] portalMats;
        Transform npc, npcLabel, pad, avatar, avatarLabel;
        Vector3 avatarTarget;
        Transform reticle;
        LineRenderer pointerLine;
        Material fadeMat;

        TextMesh hud, banner;
        float bannerUntil;
        RacePhase shownPhase = RacePhase.Countdown;

        HoverTarget hover;
        bool busy;
        readonly List<Transform> billboards = new List<Transform>();

        void Awake()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            litShader = Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
            unlitShader = Shader.Find("Sprites/Default");

            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.6f);

            rig = XRRig.Create();
            foreach (var cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                if (cam != rig.Head) Destroy(cam.gameObject);

            BuildOverlays();
            StartRound(Random.Range(1, 100000));
        }

        // ---------------------------------------------------------------- round setup

        void StartRound(int newSeed)
        {
            seed = newSeed;
            StopAllCoroutines();
            busy = false;
            SetFade(0f);
            hover = null;
            bannerUntil = 0f;
            if (worldRoot != null) Destroy(worldRoot.gameObject);
            billboards.Clear();

            tree = TreeGenerator.Generate(seed, TrunkLength, ExtraNodes);
            race = RaceState.Create(tree, seed);
            shownPhase = race.Phase;

            BuildWorld();
            PlaceRig(race.Player);
            RepositionPortals();
            StartCoroutine(RoundRoutine());
        }

        void BuildWorld()
        {
            worldRoot = new GameObject("World").transform;

            float[] x = TreeLayout.ComputeX(tree, Spacing);
            nodePos = new Vector3[tree.Count];
            for (int v = 0; v < tree.Count; v++)
                nodePos[v] = new Vector3(x[v] - x[race.Start], (tree.MaxDepth - tree.Depth[v]) * LevelHeight, 0f);

            platformMats = new Material[tree.Count];
            for (int v = 0; v < tree.Count; v++)
            {
                var plat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                plat.name = "Node " + v;
                Destroy(plat.GetComponent<Collider>());
                plat.transform.SetParent(worldRoot, false);
                plat.transform.position = nodePos[v];
                plat.transform.localScale = new Vector3(PlatformRadius * 2f, PlatformThickness / 2f, PlatformRadius * 2f);

                var box = plat.AddComponent<BoxCollider>();
                box.size = new Vector3(1.2f, 3f, 1.2f);
                var target = plat.AddComponent<HoverTarget>();
                target.Kind = TargetKind.Platform;
                target.Index = v;

                platformMats[v] = NewMat(litShader, PlatformColor);
                plat.GetComponent<Renderer>().sharedMaterial = platformMats[v];

                string label = "d" + tree.Depth[v];
                if (v == race.Start) label = "START\n" + label;
                if (v == race.Target) label = "GOAL\n" + label;
                var text = MakeText(label, worldRoot, nodePos[v] + Vector3.up * 0.4f, 0.02f, Color.white);
                billboards.Add(text.transform);

                if (v > 0) MakeEdge(nodePos[tree.Parent[v]], nodePos[v]);
            }

            var padGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            padGo.name = "PlayerPad";
            Destroy(padGo.GetComponent<Collider>());
            pad = padGo.transform;
            pad.SetParent(worldRoot, false);
            pad.localScale = new Vector3(1.4f, 0.03f, 1.4f);
            padGo.GetComponent<Renderer>().sharedMaterial = NewMat(litShader, new Color(0.2f, 0.85f, 0.55f));

            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "GoalPillar";
            Destroy(pillar.GetComponent<Collider>());
            pillar.transform.SetParent(worldRoot, false);
            pillar.transform.position = nodePos[race.Target] + Vector3.up * 1.6f;
            pillar.transform.localScale = new Vector3(0.06f, 1.5f, 0.06f);
            pillar.GetComponent<Renderer>().sharedMaterial = NewMat(litShader, GoalColor);

            var npcGo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcGo.name = "NPC";
            Destroy(npcGo.GetComponent<Collider>());
            npc = npcGo.transform;
            npc.SetParent(worldRoot, false);
            npc.localScale = new Vector3(0.35f, 0.4f, 0.35f);
            npc.position = NpcPosition(race.Npc);
            npcGo.GetComponent<Renderer>().sharedMaterial = NewMat(litShader, new Color(0.9f, 0.2f, 0.2f));
            npcLabel = MakeText("NPC", worldRoot, Vector3.zero, 0.02f, new Color(1f, 0.5f, 0.5f)).transform;
            billboards.Add(npcLabel);

            var avatarGo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            avatarGo.name = "PlayerAvatar";
            Destroy(avatarGo.GetComponent<Collider>());
            avatar = avatarGo.transform;
            avatar.SetParent(worldRoot, false);
            avatar.localScale = new Vector3(0.35f, 0.4f, 0.35f);
            avatarTarget = AvatarPosition(race.Player);
            avatar.position = avatarTarget;
            avatarGo.GetComponent<Renderer>().sharedMaterial = NewMat(litShader, new Color(0.2f, 0.6f, 1f));
            avatarLabel = MakeText("YOU", worldRoot, Vector3.zero, 0.02f, new Color(0.5f, 0.85f, 1f)).transform;
            billboards.Add(avatarLabel);

            BuildPortals();
        }

        void MakeEdge(Vector3 a, Vector3 b)
        {
            var edge = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            edge.name = "Edge";
            Destroy(edge.GetComponent<Collider>());
            edge.transform.SetParent(worldRoot, false);
            Vector3 dir = b - a;
            edge.transform.position = (a + b) * 0.5f;
            edge.transform.up = dir.normalized;
            edge.transform.localScale = new Vector3(0.05f, dir.magnitude * 0.5f, 0.05f);
            edge.GetComponent<Renderer>().sharedMaterial = NewMat(litShader, new Color(0.5f, 0.5f, 0.55f));
        }

        void BuildPortals()
        {
            int n = tree.Bits;
            portalRoot = new GameObject("Portals").transform;
            portalRoot.SetParent(worldRoot, false);
            portalHolders = new Transform[n];
            portalMats = new Material[n];

            for (int j = 0; j < n; j++)
            {
                var holder = new GameObject("Portal " + (1 << j));
                holder.transform.SetParent(portalRoot, false);
                var sphere = holder.AddComponent<SphereCollider>();
                sphere.radius = 0.4f;
                var target = holder.AddComponent<HoverTarget>();
                target.Kind = TargetKind.Portal;
                target.Index = j;

                var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                disc.name = "Disc";
                Destroy(disc.GetComponent<Collider>());
                disc.transform.SetParent(holder.transform, false);
                disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                disc.transform.localScale = new Vector3(0.5f, 0.015f, 0.5f);
                portalMats[j] = NewMat(litShader, PortalColor(j, true));
                disc.GetComponent<Renderer>().sharedMaterial = portalMats[j];

                var label = MakeText((1 << j).ToString(), holder.transform, new Vector3(0f, 0.45f, 0f), 0.025f, Color.white);
                billboards.Add(label.transform);
                portalHolders[j] = holder.transform;
            }
        }

        static Color PortalColor(int j, bool valid)
        {
            return valid ? Color.HSVToRGB((j * 0.17f) % 1f, 0.75f, 1f) : new Color(0.25f, 0.25f, 0.25f);
        }

        void RepositionPortals()
        {
            portalRoot.position = rig.transform.position;
            portalRoot.rotation = Quaternion.Euler(0f, rig.Head.transform.eulerAngles.y, 0f);

            int n = portalHolders.Length;
            for (int j = 0; j < n; j++)
            {
                float angle = (j - (n - 1) / 2f) * 22f;
                Quaternion rot = Quaternion.Euler(0f, angle, 0f);
                portalHolders[j].localPosition = rot * Vector3.forward * 1.4f + Vector3.up * 1.3f;
                portalHolders[j].localRotation = rot;
                portalMats[j].color = PortalColor(j, tree.Up[j][race.Player] >= 0);
            }
        }

        void PlaceRig(int node)
        {
            Vector3 feet = nodePos[node] + new Vector3(0f, PlatformThickness / 2f, -ViewDistance);
            rig.transform.position = feet;
            pad.position = feet - Vector3.up * 0.015f;
            avatarTarget = AvatarPosition(node);
        }

        Vector3 AvatarPosition(int node)
        {
            return nodePos[node] + new Vector3(-0.22f, PlatformThickness / 2f + 0.4f, 0f);
        }

        Vector3 NpcPosition(int node)
        {
            return nodePos[node] + new Vector3(0.22f, PlatformThickness / 2f + 0.4f, 0f);
        }

        // ---------------------------------------------------------------- overlays (HUD, fade, reticle)

        void BuildOverlays()
        {
            Transform head = rig.Head.transform;

            hud = MakeText("", head, new Vector3(0f, -0.32f, 1.4f), 0.0045f, Color.white);
            hud.fontSize = 100;
            hud.anchor = TextAnchor.UpperCenter;

            banner = MakeText("", head, new Vector3(0f, 0.15f, 2.2f), 0.018f, Color.white);
            banner.fontSize = 100;

            var fade = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fade.name = "Fade";
            Destroy(fade.GetComponent<Collider>());
            fade.transform.SetParent(head, false);
            fade.transform.localPosition = new Vector3(0f, 0f, 0.3f);
            fade.transform.localScale = Vector3.one * 3f;
            fadeMat = NewMat(unlitShader, new Color(0f, 0f, 0f, 0f));
            fade.GetComponent<Renderer>().sharedMaterial = fadeMat;

            var ret = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ret.name = "Reticle";
            Destroy(ret.GetComponent<Collider>());
            reticle = ret.transform;
            ret.GetComponent<Renderer>().sharedMaterial = NewMat(unlitShader, Color.white);

            var lineGo = new GameObject("PointerLine");
            lineGo.transform.SetParent(transform, false);
            pointerLine = lineGo.AddComponent<LineRenderer>();
            pointerLine.positionCount = 2;
            pointerLine.startWidth = pointerLine.endWidth = 0.01f;
            pointerLine.sharedMaterial = NewMat(unlitShader, HoverColor);
            pointerLine.enabled = false;
        }

        void SetFade(float alpha)
        {
            if (fadeMat == null) return;
            Color c = fadeMat.color;
            c.a = alpha;
            fadeMat.color = c;
        }

        IEnumerator Fade(float from, float to, float seconds)
        {
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                SetFade(Mathf.Lerp(from, to, t / seconds));
                yield return null;
            }
            SetFade(to);
        }

        void ShowBanner(string text, float seconds = 0f)
        {
            banner.text = text;
            bannerUntil = seconds > 0f ? Time.time + seconds : 0f;
        }

        // ---------------------------------------------------------------- race loop

        IEnumerator RoundRoutine()
        {
            for (int i = CountdownSeconds; i > 0; i--)
            {
                ShowBanner("Get ready  " + i);
                yield return new WaitForSeconds(1f);
            }

            ShowBanner("GO!", 1f);
            race.Begin();

            while (race.Phase == RacePhase.Running)
            {
                yield return new WaitForSeconds(NpcStepSeconds);
                if (race.Phase != RacePhase.Running) yield break;

                race.StepNpc();
                Vector3 from = npc.position, to = NpcPosition(race.Npc);
                for (float t = 0f; t < 0.5f; t += Time.deltaTime)
                {
                    npc.position = Vector3.Lerp(from, to, t / 0.5f);
                    yield return null;
                }
                npc.position = to;
            }
        }

        IEnumerator TeleportRoutine()
        {
            busy = true;
            yield return Fade(0f, 1f, 0.08f);
            PlaceRig(race.Player);
            RepositionPortals();
            yield return Fade(1f, 0f, 0.15f);
            busy = false;
        }

        void Select()
        {
            if (busy || hover == null || race.Phase != RacePhase.Running) return;
            bool ok = hover.Kind == TargetKind.Platform
                ? race.TryStepTo(hover.Index)
                : race.TryPortal(hover.Index);
            if (ok) StartCoroutine(TeleportRoutine());
        }

        // ---------------------------------------------------------------- per-frame

        void Update()
        {
            rig.Poll();
            if (rig.RestartPressed) { StartRound(seed + 1); return; }

            UpdateHover();
            if (rig.SelectPressed) Select();

            if (race.Phase != shownPhase)
            {
                shownPhase = race.Phase;
                if (shownPhase == RacePhase.Won)
                    ShowBanner("YOU WIN!\n" + race.Moves + " jumps vs " + race.K + " steps\nB / R = new round");
                else if (shownPhase == RacePhase.Lost)
                    ShowBanner("NPC WINS\nB / R = new round");
            }
            if (bannerUntil > 0f && Time.time > bannerUntil) { banner.text = ""; bannerUntil = 0f; }

            avatar.position = Vector3.Lerp(avatar.position, avatarTarget, 1f - Mathf.Exp(-10f * Time.deltaTime));
            UpdateColors();
            UpdateHud();
        }

        void LateUpdate()
        {
            if (npc != null && npcLabel != null) npcLabel.position = npc.position + Vector3.up * 0.7f;
            if (avatar != null && avatarLabel != null) avatarLabel.position = avatar.position + Vector3.up * 0.7f;
            Vector3 cam = rig.Head.transform.position;
            foreach (var t in billboards)
                if (t != null) t.rotation = Quaternion.LookRotation(t.position - cam);
        }

        bool Cast(Ray ray, out HoverTarget target, out Vector3 point)
        {
            target = null;
            point = ray.origin + ray.direction * 4f;
            if (Physics.Raycast(ray, out RaycastHit hit, 80f))
            {
                point = hit.point;
                target = hit.collider.GetComponentInParent<HoverTarget>();
            }
            return target != null;
        }

        void UpdateHover()
        {
            hover = null;
            Vector3 point;
            pointerLine.enabled = false;
            reticle.gameObject.SetActive(false);

            if (rig.XRActive)
            {
                bool found = false;
                foreach (bool right in new[] { true, false })
                {
                    if (!rig.TryGetHandRay(right, out Ray ray)) continue;
                    found = Cast(ray, out hover, out point);
                    if (right)
                    {
                        pointerLine.enabled = true;
                        pointerLine.SetPosition(0, ray.origin);
                        pointerLine.SetPosition(1, point);
                    }
                    if (found) return;
                }

                Ray gaze = rig.GazeRay;
                Cast(gaze, out hover, out point);
                reticle.gameObject.SetActive(true);
                reticle.position = point;
                reticle.localScale = Vector3.one * 0.012f * Mathf.Max(1f, Vector3.Distance(gaze.origin, point));
            }
            else
            {
                try { Cast(rig.MouseRay, out hover, out point); }
                catch (System.InvalidOperationException) { }
            }
        }

        void UpdateColors()
        {
            for (int v = 0; v < tree.Count; v++)
            {
                Color c = PlatformColor;
                if (v == race.Start) c = StartColor;
                if (v == race.Target) c = GoalColor;
                if (v == race.Player) c = PlayerColor;
                platformMats[v].color = c;
            }

            if (hover == null) return;
            if (hover.Kind == TargetKind.Platform)
            {
                int v = hover.Index;
                bool adjacent = v != race.Player && (tree.Parent[race.Player] == v || tree.Parent[v] == race.Player);
                if (adjacent) platformMats[v].color = HoverColor;
            }
            else
            {
                int dest = tree.Up[hover.Index][race.Player];
                if (dest >= 0) platformMats[dest].color = PreviewColor;
            }
        }

        void UpdateHud()
        {
            int depth = tree.Depth[race.Player];
            int goalDepth = tree.Depth[race.Target];
            string text = "Climb " + race.K + " levels\n"
                          + "You: depth " + depth + "   Goal: depth " + goalDepth + "\n"
                          + "Jumps " + race.Moves + "   NPC " + race.NpcSteps + "/" + race.K;

            if (hover != null && hover.Kind == TargetKind.Portal)
            {
                int dest = tree.Up[hover.Index][race.Player];
                text += dest >= 0
                    ? "\nPortal +" + (1 << hover.Index) + " -> depth " + tree.Depth[dest]
                    : "\nPortal +" + (1 << hover.Index) + " goes past the root";
            }
            hud.text = text;
        }

        // ---------------------------------------------------------------- helpers

        static Material NewMat(Shader shader, Color color)
        {
            return new Material(shader) { color = color };
        }

        static TextMesh MakeText(string s, Transform parent, Vector3 position, float characterSize, Color color)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position; // worldRoot sits at the origin, so local == world there

            var tm = go.AddComponent<TextMesh>();
            tm.font = font;
            tm.text = s;
            tm.fontSize = 100;
            tm.characterSize = characterSize;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color;
            go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
            return tm;
        }
    }

    static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            if (Object.FindFirstObjectByType<RaceController>() != null) return;
            new GameObject("BinaryLiftGame").AddComponent<RaceController>();
        }
    }
}
