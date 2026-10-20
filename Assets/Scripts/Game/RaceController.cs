using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BinaryLift
{
    public enum TargetKind { Platform, Portal, Start, Retry, NewRound }

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
        TextMesh hint;
        Transform startButton, retryButton, newButton;
        TextMesh[] portalLabels;
        LineRenderer routePreview;
        bool waitingForStart;
        bool activeRight = true;
        string feedback = "";
        float feedbackUntil;
        readonly List<Material> roundMaterials = new List<Material>();
        readonly List<Material> overlayMaterials = new List<Material>();
        public RaceState State => race;
        public bool IsBusy => busy;
        public bool WaitingForStart => waitingForStart;
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
            sun.transform.SetParent(transform, false);
            sun.type = LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.6f);

            rig = XRRig.Create();
            foreach (var cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                if (cam != rig.Head) Destroy(cam.gameObject);

            BuildOverlays();
            StartRound(13);
        }

        // ---------------------------------------------------------------- round setup

        public void StartRound(int newSeed)
        {
            seed = newSeed;
            StopAllCoroutines();
            busy = false;
            SetFade(0f);
            hover = null;
            bannerUntil = 0f;
            if (worldRoot != null) Destroy(worldRoot.gameObject);
            foreach (var material in roundMaterials) Destroy(material);
            roundMaterials.Clear();
            billboards.Clear();
            waitingForStart = true;
            feedbackUntil = 0;

            tree = TreeGenerator.Generate(seed, TrunkLength, ExtraNodes);
            race = RaceState.Create(tree, seed);
            shownPhase = race.Phase;

            BuildWorld();
            PlaceRig(race.Player);
            RepositionPortals();
            portalRoot.gameObject.SetActive(false);
            ShowBanner("BINARY LIFT\nReach the gold platform before the red runner.\nClick a numbered portal to climb 1, 2, 4 or 8 levels.\nThe runner climbs one level at a time.");
            UpdateActionButtons();
            Debug.Log("Race | ready | seed=" + seed + " | levels=" + race.K);
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
            var routeGo = new GameObject("Hovered ancestor path");
            routeGo.transform.SetParent(worldRoot, false);
            routePreview = routeGo.AddComponent<LineRenderer>();
            routePreview.sharedMaterial = NewMat(unlitShader, HoverColor);
            routePreview.startWidth = routePreview.endWidth = .045f;
            routePreview.enabled = false;
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
            portalLabels = new TextMesh[n];

            for (int j = 0; j < n; j++)
            {
                var holder = new GameObject("Portal " + (1 << j));
                holder.transform.SetParent(portalRoot, false);
                var sphere = holder.AddComponent<SphereCollider>();
                sphere.radius = 0.27f;
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

                var label = MakeText((1 << j).ToString(), holder.transform, new Vector3(0f, 0f, -.04f), 0.015f, Color.white);
                portalLabels[j] = label;
                billboards.Add(label.transform);
                portalHolders[j] = holder.transform;
            }
        }

        static Color PortalColor(int j, bool valid)
        {
            return valid ? Color.Lerp(new Color(.05f,.35f,.43f), new Color(.18f,.65f,.72f), j/4f) : new Color(.13f,.15f,.18f);
        }

        void RepositionPortals()
        {
            portalRoot.position = new Vector3(rig.Head.transform.position.x, rig.Head.transform.position.y - 1.6f, rig.Head.transform.position.z);
            portalRoot.rotation = Quaternion.Euler(0f, rig.Head.transform.eulerAngles.y, 0f);

            int n = portalHolders.Length;
            for (int j = 0; j < n; j++)
            {
                float angle = (j - (n - 1) / 2f) * 22f;
                Quaternion rot = Quaternion.Euler(0f, angle, 0f);
                portalHolders[j].localPosition = rot * Vector3.forward * 1.65f + Vector3.up * 1.25f;
                portalHolders[j].localRotation = rot;
                portalMats[j].color = PortalColor(j, tree.Up[j][race.Player] >= 0);
            }
        }

        void PlaceRig(int node)
        {
            Vector3 feet = nodePos[node] + new Vector3(0f, PlatformThickness / 2f, -ViewDistance);
            rig.TeleportFeet(feet);
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

            hud = MakeText("", head, new Vector3(0f, .67f, 2.2f), 0.0058f, Color.white);
            hud.fontSize = 100;
            hud.anchor = TextAnchor.UpperCenter;

            banner = MakeText("", head, new Vector3(0f, .12f, 2.2f), 0.008f, Color.white);
            banner.fontSize = 100;
            hint = MakeText("", head, new Vector3(0f, -.75f, 2.2f), .0048f, Color.white);
            startButton = ActionButton("START RACE", TargetKind.Start, new Vector3(0,-.42f,2.2f));
            retryButton = ActionButton("RETRY SAME TREE", TargetKind.Retry, new Vector3(-.6f,-.42f,2.2f));
            newButton = ActionButton("NEW TREE", TargetKind.NewRound, new Vector3(.6f,-.42f,2.2f));

            var fade = GameObject.CreatePrimitive(PrimitiveType.Quad);
            fade.name = "Fade";
            Destroy(fade.GetComponent<Collider>());
            fade.transform.SetParent(head, false);
            fade.transform.localPosition = new Vector3(0f, 0f, 0.3f);
            fade.transform.localScale = Vector3.one * 3f;
            fadeMat = NewMat(unlitShader, new Color(0f, 0f, 0f, 0f));
            fadeMat.renderQueue = 5000;
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

        Transform ActionButton(string label, TargetKind kind, Vector3 position)
        {
            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.name = label;
            button.transform.SetParent(rig.Head.transform, false);
            button.transform.localPosition = position;
            button.transform.localScale = new Vector3(1.05f,.24f,.05f);
            button.GetComponent<Renderer>().sharedMaterial = NewMat(unlitShader, new Color(.08f,.36f,.43f));
            button.AddComponent<HoverTarget>().Kind = kind;
            var labelText = MakeText(label,rig.Head.transform,position+new Vector3(0,0,-.04f),.0055f,Color.white);
            labelText.transform.SetParent(button.transform,true);
            return button.transform;
        }

        void UpdateActionButtons()
        {
            bool finished = race.Phase == RacePhase.Won || race.Phase == RacePhase.Lost;
            startButton.gameObject.SetActive(waitingForStart);
            retryButton.gameObject.SetActive(finished);
            newButton.gameObject.SetActive(finished);
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
            Debug.Log("Race | started | seed=" + seed);

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

        public bool SelectTarget(TargetKind kind, int index = 0)
        {
            if (busy) return false;
            if (kind == TargetKind.Start && waitingForStart)
            {
                waitingForStart = false;
                UpdateActionButtons();
                portalRoot.gameObject.SetActive(true);
                StartCoroutine(RoundRoutine());
                return true;
            }
            bool finished = race.Phase == RacePhase.Won || race.Phase == RacePhase.Lost;
            if (finished && (kind == TargetKind.Retry || kind == TargetKind.NewRound))
            { StartRound(kind == TargetKind.Retry ? seed : seed + 1); return true; }
            if (race.Phase != RacePhase.Running) return false;
            bool ok = kind == TargetKind.Platform ? race.TryStepTo(index)
                : kind == TargetKind.Portal && race.TryPortal(index);
            if (ok)
            {
                rig.Pulse(activeRight);
                feedback = "Moved to depth " + tree.Depth[race.Player] + " / " + race.Moves + " jumps";
                Debug.Log("Race | moved | depth=" + tree.Depth[race.Player] + " | moves=" + race.Moves);
                StartCoroutine(TeleportRoutine());
            }
            else feedback = kind == TargetKind.Platform ? "Choose a connected platform or a numbered portal." : "This portal goes beyond the root. Choose a smaller jump.";
            feedbackUntil = Time.time + 2;
            return ok;
        }

        // ---------------------------------------------------------------- per-frame

        void Update()
        {
            rig.Poll();
            if (rig.RestartPressed) { StartRound(seed); return; }

            UpdateHover();
            if (rig.RightSelectPressed || rig.LeftSelectPressed)
            {
                if (hover != null) SelectTarget(hover.Kind, hover.Index);
                else { feedback="Point at a numbered portal or a connected platform."; feedbackUntil=Time.time+2; }
            }

            if (race.Phase != shownPhase)
            {
                shownPhase = race.Phase;
                UpdateActionButtons();
                if (shownPhase == RacePhase.Won)
                {
                    ShowBanner("YOU WIN!\n" + race.Moves + " jumps vs " + race.K + " one-level steps\nBest possible: " + TreeModel.PopCount(race.K) + " jumps");
                    Debug.Log("Race | won | moves=" + race.Moves + " | ordinary_steps=" + race.K);
                }
                else if (shownPhase == RacePhase.Lost)
                {
                    ShowBanner("THE RUNNER ARRIVED FIRST\nUse larger jumps, then smaller ones to land on gold.\nRetry the same tree to compare your route.");
                    Debug.Log("Race | lost | moves=" + race.Moves);
                }
                if (race.Phase == RacePhase.Won || race.Phase == RacePhase.Lost) portalRoot.gameObject.SetActive(false);
            }
            if (bannerUntil > 0f && Time.time > bannerUntil) { banner.text = ""; bannerUntil = 0f; }

            avatar.position = Vector3.Lerp(avatar.position, avatarTarget, 1f - Mathf.Exp(-10f * Time.deltaTime));
            UpdateColors();
            UpdateHud();
            UpdatePreview();
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
                if(rig.RightSelectPressed) activeRight=true;
                else if(rig.LeftSelectPressed) activeRight=false;
                Ray ray;
                if (rig.TryGetHandRay(activeRight, out ray) || rig.TryGetHandRay(!activeRight,out ray))
                {
                    Cast(ray,out hover,out point);
                    pointerLine.enabled=true;
                    pointerLine.SetPosition(0,ray.origin);
                    pointerLine.SetPosition(1,point);
                    return;
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
            for(int j=0;j<portalMats.Length;j++)
            {
                bool valid=tree.Up[j][race.Player]>=0;
                portalMats[j].color=PortalColor(j,valid);
                portalLabels[j].color=valid?Color.white:Color.gray;
                portalHolders[j].localScale=Vector3.one;
            }
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
            else if (hover.Kind == TargetKind.Portal)
            {
                int dest = tree.Up[hover.Index][race.Player];
                if (dest >= 0) platformMats[dest].color = PreviewColor;
                portalMats[hover.Index].color = dest >= 0 ? HoverColor : new Color(.5f,.18f,.12f);
                portalHolders[hover.Index].localScale = Vector3.one * 1.08f;
            }
        }

        void UpdateHud()
        {
            int depth = tree.Depth[race.Player];
            int goalDepth = tree.Depth[race.Target];
            string text = "GOLD TARGET / " + race.K + " levels up\n"
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
            hint.text = waitingForStart ? "Point + trigger / mouse click to start. Take your time to read."
                : busy ? "JUMPING..."
                : Time.time < feedbackUntil ? feedback
                : race.Phase == RacePhase.Countdown ? "Get ready. Choose a numbered portal when GO appears."
                : race.Phase != RacePhase.Running ? "Select RETRY or NEW TREE. B / Y / R retries this tree."
                : hover != null && hover.Kind == TargetKind.Portal ? "CLICK / TRIGGER: jump " + (1 << hover.Index) + " ancestor levels"
                : "Point + trigger / click: jump  |  Right mouse: look  |  B / Y / R: retry";
        }

        void UpdatePreview()
        {
            routePreview.enabled=false;
            if(hover==null || race.Phase!=RacePhase.Running || busy) return;
            var points=new List<Vector3>();
            if(hover.Kind==TargetKind.Portal)
            {
                int destination=tree.Up[hover.Index][race.Player];
                if(destination<0)return;
                for(int v=race.Player;v>=0;v=tree.Parent[v])
                {points.Add(nodePos[v]+Vector3.back*.08f);if(v==destination)break;}
            }
            else if(hover.Kind==TargetKind.Platform && (tree.Parent[race.Player]==hover.Index || tree.Parent[hover.Index]==race.Player))
            {points.Add(nodePos[race.Player]);points.Add(nodePos[hover.Index]);}
            if(points.Count<2)return;
            routePreview.positionCount=points.Count;routePreview.SetPositions(points.ToArray());routePreview.enabled=true;
        }

        // ---------------------------------------------------------------- helpers

        Material NewMat(Shader shader, Color color)
        {
            var material = new Material(shader) { color = color };
            if(worldRoot==null)overlayMaterials.Add(material);else roundMaterials.Add(material);
            return material;
        }

        void OnDestroy()
        {
            if(worldRoot!=null)Destroy(worldRoot.gameObject);
            if(rig!=null)Destroy(rig.gameObject);
            if(reticle!=null)Destroy(reticle.gameObject);
            foreach(var material in roundMaterials)if(material!=null)Destroy(material);
            foreach(var material in overlayMaterials)if(material!=null)Destroy(material);
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
            if (Object.FindFirstObjectByType<RaceController>() != null ||
                Object.FindFirstObjectByType<CampusQuestController>() != null ||
                Object.FindFirstObjectByType<TreeClimbingController>() != null ||
                Object.FindFirstObjectByType<AdventureController>() != null) return;
            new GameObject("ConeCourierAdventure").AddComponent<AdventureController>();
        }
    }
}
