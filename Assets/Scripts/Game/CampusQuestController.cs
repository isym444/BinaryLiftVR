using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BinaryLift
{
    /// <summary>First-person VR slice: recover cones, dock them, and learn binary lifting.</summary>
    public sealed class CampusQuestController : MonoBehaviour
    {
        public CampusQuestState State { get; private set; }
        public CampusWorld World { get; private set; }
        public XRRig Rig { get; private set; }
        public bool Busy { get; private set; }
        QuestJournal journal;
        QuestTarget hovered;
        LineRenderer pointer;
        Transform reticle, fade;
        Material fadeMaterial;
        bool activeRight = true, heldRight, heldWithGrip, prevRightGrip, prevLeftGrip;
        int scanIndex = -1;
        float scanClock, introClock;
        List<int> bfs;
        string message = "";
        float messageUntil;
        readonly Vector3 coneStand = new Vector3(-1.9f,.81f,1.25f);

        void Awake()
        {
            State = new CampusQuestState();
            World = new GameObject("Gilmorehill campus blockout").AddComponent<CampusWorld>();
            World.Build(State);
            Rig = XRRig.Create();
            foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                if (cam != Rig.Head) Destroy(cam.gameObject);
            Rig.Head.backgroundColor = new Color(.075f,.13f,.19f);
            Rig.Head.farClipPlane = 130;
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.60f,.65f,.70f);
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = Rig.Head.backgroundColor;
            RenderSettings.fogStartDistance = 40; RenderSettings.fogEndDistance = 110;
            var sun = new GameObject("Evening light").AddComponent<Light>();
            sun.transform.SetParent(transform); sun.type = LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(38,-32,0); sun.intensity = 1.1f;
            sun.color = new Color(1,.84f,.65f); sun.shadows = LightShadows.None;
            QualitySettings.antiAliasing = 4;
            pointer = World.Line("Interaction ray",transform,World.Cyan,.006f);
            pointer.positionCount = 2;
            reticle = World.Primitive("Target reticle",PrimitiveType.Sphere,transform,Vector3.zero,Vector3.one*.018f,World.Cyan).transform;
            fadeMaterial = World.Mat(new Color(0,0,0,0),true);
            fadeMaterial.renderQueue = 5000;
            fade = World.Primitive("Comfort fade",PrimitiveType.Quad,Rig.Head.transform,new Vector3(0,0,.07f),Vector3.one*4,fadeMaterial).transform;
            fade.gameObject.SetActive(false);
            journal = new QuestJournal(); journal.Write("session_started",State);
            PlaceStation(); RefreshPresentation();
        }

        void Update()
        {
            Rig.Poll();
            if (Rig.RightSelectPressed || Rig.RightGrip) activeRight = true;
            else if (Rig.LeftSelectPressed || Rig.LeftGrip) activeRight = false;
            if (!Busy && Rig.SnapDirection != 0) StartCoroutine(Turn(Rig.SnapDirection));
            UpdateTarget();
            if (!Busy)
            {
                if (Rig.RestartPressed) RestartQuest();
                else if (Rig.RightSelectPressed || Rig.LeftSelectPressed) Activate(hovered);
                ProcessGrip();
            }
            prevRightGrip = Rig.RightGrip; prevLeftGrip = Rig.LeftGrip;
            UpdateCarriedCone();
            UpdateMap();
            AnimateCharacters();
            World.Hint.text = WrapHint(Busy ? "TRANSFERRING..." : Time.unscaledTime < messageUntil ? message :
                hovered != null ? Describe(hovered) : "POINT + TRIGGER  /  Q,E or stick: snap turn");
        }

        void PlaceStation()
        {
            Vector3 position = World.NodePosition(State.Node);
            Rig.TeleportFeet(position);
            World.Station.position = position;
            World.Calf.position = World.NodePosition(State.Goal) + new Vector3(3.4f,.6f,5);
        }

        void UpdateTarget()
        {
            if (hovered != null) hovered.Highlight(false);
            hovered = null;
            Ray ray;
            if (Rig.XRActive)
            {
                if (!Rig.TryGetHandRay(activeRight,out ray))
                {
                    activeRight = !activeRight;
                    if (!Rig.TryGetHandRay(activeRight,out ray)) ray = Rig.GazeRay;
                }
            }
            else ray = Rig.MouseRay;
            Vector3 point = ray.GetPoint(6);
            if (!Busy && Physics.Raycast(ray,out RaycastHit hit,6))
            {
                hovered = hit.collider.GetComponentInParent<QuestTarget>(); point = hit.point;
            }
            pointer.enabled = Rig.XRActive && !Busy;
            pointer.SetPosition(0,ray.origin); pointer.SetPosition(1,point);
            reticle.gameObject.SetActive(hovered != null && !Busy); reticle.position = point;
            if (hovered != null) hovered.Highlight(true);
            World.RouteLine.enabled = hovered != null && hovered.Action == QuestAction.Jump && State.CanJump(hovered.Value) && !Busy;
            if (World.RouteLine.enabled)
            {
                var route = State.Preview(hovered.Value); World.RouteLine.positionCount = route.Count;
                for (int i=0;i<route.Count;i++) World.RouteLine.SetPosition(i,World.NodePosition(route[i])+Vector3.up*.07f);
            }
        }

        string Describe(QuestTarget target)
        {
            if (target.Action != QuestAction.Jump) return target.Caption;
            if (target.Value > State.MaxJump) return "LOCKED / recover another cone";
            if (!State.CanJump(target.Value)) return State.Phase == QuestPhase.CarryingCone ? "STORE YOUR CONE ON THE HORSE" : "This jump passes the next clue";
            return "JUMP " + target.Value + " / depth " + State.Node + " -> " + (State.Node-target.Value) + " / trigger to confirm";
        }

        public void Activate(QuestTarget target)
        {
            if (Busy || target == null) return;
            switch (target.Action)
            {
                case QuestAction.Begin:
                    if (State.Begin()) { journal.Write("trail_started",State); RefreshPresentation(); }
                    break;
                case QuestAction.Jump:
                    if (State.CanJump(target.Value)) StartCoroutine(Travel(target.Value));
                    else Say(Describe(target));
                    break;
                case QuestAction.Cone: GrabCone(activeRight,false); break;
                case QuestAction.Saddle: Deposit(); break;
                case QuestAction.Scan:
                    bfs = State.BreadthFirstOrder(); scanIndex = 0; scanClock = 0;
                    journal.Write("bfs_scan_started",State);
                    Say("BFS scans level by level. Lifting follows ancestors.",5); break;
                case QuestAction.Restart: RestartQuest(); break;
            }
        }

        void GrabCone(bool right,bool grip)
        {
            if (!State.PickUpCone()) return;
            heldRight = right; heldWithGrip = grip;
            World.ConeTarget.GetComponent<Collider>().enabled = false;
            Rig.Pulse(right,.4f,.1f); journal.Write("cone_picked_up",State);
            Say("Bring the cone to the orange saddle dock.",4); RefreshPresentation();
        }

        void Deposit()
        {
            if (!State.DepositCone()) { Say("Recover the clue cone first."); return; }
            journal.Write("cone_stored",State); Rig.Pulse(heldRight,.6f,.12f);
            heldWithGrip = false; ResetConeTransform();
            RefreshPresentation();
            Say(State.Phase == QuestPhase.Complete ? "ROUTE RESTORED / all four cones recovered" :
                "ROUTE MEMORY RESTORED / " + State.MaxJump + "-step jump unlocked",4);
            if (State.Phase == QuestPhase.Complete) journal.Write("quest_completed",State);
        }

        void ProcessGrip()
        {
            if (!Rig.XRActive) return;
            if (State.Phase == QuestPhase.Exploring)
            {
                Vector3 cone = World.Cone.position + Vector3.up*.25f;
                if (Rig.RightTracked && Rig.RightGrip && !prevRightGrip && Vector3.Distance(Rig.RightHand.position,cone)<.42f)
                    GrabCone(true,true);
                else if (Rig.LeftTracked && Rig.LeftGrip && !prevLeftGrip && Vector3.Distance(Rig.LeftHand.position,cone)<.42f)
                    GrabCone(false,true);
            }
            else if (State.Phase == QuestPhase.CarryingCone && heldWithGrip)
            {
                bool tracked = heldRight ? Rig.RightTracked : Rig.LeftTracked;
                bool grip = heldRight ? Rig.RightGrip : Rig.LeftGrip;
                if (tracked && grip) return;
                Transform hand = heldRight ? Rig.RightHand : Rig.LeftHand;
                if (tracked && Vector3.Distance(hand.position,World.Saddle.position)<.55f) Deposit();
                else ReturnConeToStand();
            }
        }

        void ReturnConeToStand()
        {
            if (!State.ReturnCone()) return;
            heldWithGrip=false; ResetConeTransform(); RefreshPresentation();
            journal.Write("cone_returned_to_stand",State); Say("Cone returned to its stand. Try again.");
        }

        void UpdateCarriedCone()
        {
            if (State.Phase != QuestPhase.CarryingCone) return;
            if (Rig.XRActive)
            {
                bool tracked=heldRight?Rig.RightTracked:Rig.LeftTracked;
                if (!tracked) { ReturnConeToStand(); return; }
                Transform hand=heldRight?Rig.RightHand:Rig.LeftHand;
                World.Cone.position=hand.position+hand.forward*.12f;
                World.Cone.rotation=hand.rotation;
            }
            else
            {
                World.Cone.position=Rig.Head.transform.TransformPoint(new Vector3(.6f,-.95f,1.4f));
                World.Cone.rotation=Quaternion.identity;
            }
        }

        void ResetConeTransform()
        {
            World.Cone.SetParent(World.Station,false); World.Cone.localPosition=coneStand;
            World.Cone.localRotation=Quaternion.identity;
            World.ConeTarget.GetComponent<Collider>().enabled=true;
        }

        IEnumerator Travel(int levels)
        {
            Busy=true;
            yield return Fade(0,1,.10f);
            if (State.TryJump(levels))
            {
                PlaceStation(); RefreshPresentation();
                journal.Write("jump_completed",State,levels); Rig.Pulse(activeRight);
            }
            yield return Fade(1,0,.16f); Busy=false;
        }

        IEnumerator Turn(int direction)
        {
            Busy=true; yield return Fade(0,1,.07f);
            Rig.SnapTurn(direction); yield return Fade(1,0,.09f); Busy=false;
        }

        IEnumerator Fade(float from,float to,float seconds)
        {
            fade.gameObject.SetActive(true);
            for(float t=0;t<seconds;t+=Time.unscaledDeltaTime)
            { fadeMaterial.color=new Color(0,0,0,Mathf.Lerp(from,to,t/seconds)); yield return null; }
            fadeMaterial.color=new Color(0,0,0,to);
            fade.gameObject.SetActive(to>0);
        }

        public void RestartQuest()
        {
            if (Busy) return;
            StartCoroutine(RestartRoutine());
        }

        IEnumerator RestartRoutine()
        {
            Busy=true; yield return Fade(0,1,.1f);
            State=new CampusQuestState(); heldWithGrip=false; scanIndex=-1; introClock=0;
            ResetConeTransform(); PlaceStation(); RefreshPresentation();
            journal.Write("quest_restarted",State);
            yield return Fade(1,0,.16f); Busy=false;
        }

        void RefreshPresentation()
        {
            bool briefing=State.Phase==QuestPhase.Briefing;
            bool complete=State.Phase==QuestPhase.Complete;
            World.BeginButton.gameObject.SetActive(briefing);
            for(int j=0;j<World.JumpButtons.Count;j++)
            {
                var target=World.JumpButtons[j]; target.gameObject.SetActive(!briefing);
                target.Available=State.CanJump(target.Value);
                target.BaseColor=target.Available?World.Orange.color:World.Muted.color;
                target.Highlight(false);
                World.JumpLabels[j].text=target.Value>State.MaxJump?"LOCKED":target.Value+" STEP"+(target.Value>1?"S":"");
            }
            World.Chapter.text=complete?"CAMPUS CONNECTION RESTORED":"CONE COURIER / " + (State.Cones+1).ToString("00");
            if(briefing)
                World.Objective.text="A calf took our research funding.\nFollow the trail. Recover four clue cones.\nYour horse will remember the shortcuts.";
            else if(complete)
                World.Objective.text="All four clues are back with your companion.\nYou restored the campus route memory.\nBinary lifting: combine powers of two.";
            else if(State.Phase==QuestPhase.CarryingCone)
                World.Objective.text="Clue recovered. Store it on the horse.\nPoint at the orange saddle and trigger,\nor release your grip beside the dock.";
            else if(State.Remaining==0)
                World.Objective.text="You found the next clue cone.\nPick it up from the lit stand on your left.\nBring it to your horse to unlock a shortcut.";
            else
                World.Objective.text=State.Cones==0?"Start with one parent step.\nThe first clue is one station ahead.\nPoint at 1 STEP and confirm.":
                    "Next clue: " + State.Remaining + " ancestor levels ahead.\nChoose a jump and preview the route.\n" + (State.Cones==3?"Try 13 = 8 + 4 + 1.":"Combine smaller jumps or use your new shortcut.");
            World.Status.text="CONES " + State.Cones + "/4   |   DEPTH " + State.Node + "   |   JUMPS " + State.Moves +
                "\n"+(briefing?"COMFORT TELEPORT / NO TIME LIMIT":"AVAILABLE: " + Abilities());
            World.Cone.gameObject.SetActive(State.Phase==QuestPhase.CarryingCone || (!briefing && !complete && State.Remaining==0));
            for(int i=0;i<World.TrophyCones.Count;i++)World.TrophyCones[i].gameObject.SetActive(i<State.Cones);
            World.CompanionLine.text=complete?"GOOD WORK, COURIER.\nAll cones safely home.":State.Phase==QuestPhase.CarryingCone?"OVER HERE!\nStore the cone on my saddle.":"I KNOW A SHORTCUT.\nRecovered cones: " + State.Cones + "/4";
        }

        string Abilities()
        {
            string result="1";
            for(int n=2;n<=State.MaxJump;n*=2)result+=" / "+n;
            return result+" LEVELS";
        }

        void UpdateMap()
        {
            if(scanIndex>=0)
            {
                scanClock+=Time.deltaTime;
                if(scanClock>.15f){scanClock=0;scanIndex++;if(scanIndex>=bfs.Count){scanIndex=-1;journal.Write("bfs_scan_completed",State);}}
            }
            List<int> preview=hovered!=null && hovered.Action==QuestAction.Jump?State.Preview(hovered.Value):null;
            for(int i=0;i<World.MapNodes.Count;i++)
            {
                Color color=World.Muted.color;
                if(scanIndex>=0 && bfs.IndexOf(i)<=scanIndex)color=new Color(.5f,.5f,.8f);
                if(preview!=null && preview.Contains(i))color=World.White.color;
                if(i==State.Goal)color=World.Cyan.color;
                if(i==State.Node)color=World.Orange.color;
                World.MapNodes[i].sharedMaterial.color=color;
            }
        }

        void AnimateCharacters()
        {
            World.Horse.localPosition=new Vector3(1.95f,Mathf.Sin(Time.time*1.5f)*.012f,3.0f);
            if(State.Phase==QuestPhase.Briefing)
            {
                introClock=Mathf.Min(introClock+Time.deltaTime,5);
                World.Calf.position=Vector3.Lerp(new Vector3(-2.4f,.8f,5),new Vector3(3.4f,.8f,8),introClock/5);
            }
            World.Calf.localRotation=Quaternion.Euler(0,Mathf.Sin(Time.time)*12,0);
        }

        void Say(string text,float seconds=3){message=text;messageUntil=Time.unscaledTime+seconds;}

        static string WrapHint(string text)
        {
            if(text.Length<=48) return text;
            int split=text.LastIndexOf(' ',Mathf.Min(48,text.Length-1));
            return split>0?text.Substring(0,split)+"\n"+text.Substring(split+1):text;
        }

        void OnDestroy()
        {
            if(World!=null)Destroy(World.gameObject);
            if(Rig!=null)Destroy(Rig.gameObject);
        }
    }
}
