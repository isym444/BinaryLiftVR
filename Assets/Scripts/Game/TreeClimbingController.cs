using System.Collections.Generic;
using UnityEngine;

namespace BinaryLift
{
    /// <summary>Primitive tree climbing: anchored hands move the rig, never the camera pose.</summary>
    public sealed class TreeClimbingController : MonoBehaviour
    {
        public XRRig Rig { get; private set; }
        public CampusWorld World { get; private set; }
        public ClimbProgress Progress { get; private set; }
        public readonly List<Collider> Bark = new List<Collider>();
        public readonly List<Vector3> Nodes = new List<Vector3>();
        readonly Dictionary<int, Transform> cones = new Dictionary<int, Transform>();
        readonly bool[] held = new bool[2], previousGrip = new bool[2];
        readonly Vector3[] anchors = new Vector3[2];
        readonly Transform[] gloves = new Transform[2];
        LineRenderer rope, preview;
        TextMesh instruction, wrist;
        Material wood, leaves;
        int nearest, pulls, ropeTarget = -1, ropeFrom;
        float ropeDistance;
        Vector3 previousRopeHand;
        bool ropeHeld, previousTrigger;
        string eventMessage = "";
        float eventUntil;
        System.IO.StreamWriter journal;

        void Awake()
        {
            Progress = new ClimbProgress();
            World = new GameObject("University courtyard").AddComponent<CampusWorld>();
            World.Build(new CampusQuestState(), true);
            Rig = XRRig.Create();
            foreach (Camera camera in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                if (camera != Rig.Head) Destroy(camera.gameObject);
            RenderSettings.skybox = null;
            RenderSettings.ambientLight = new Color(.65f,.70f,.74f);
            Rig.Head.backgroundColor = new Color(.10f,.17f,.22f);
            var sun = new GameObject("Campus daylight").AddComponent<Light>();
            sun.transform.SetParent(transform); sun.type = LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(38,-30,0); sun.intensity = 1.1f;
            sun.shadows = LightShadows.None;
            wood = World.Mat(new Color(.25f,.18f,.12f));
            wood.SetFloat("_Glossiness", .08f);
            leaves = World.Mat(new Color(.12f,.30f,.19f));
            BuildTree();
            Rig.TeleportFeet(new Vector3(0,0,3.75f));
            World.Horse.position = new Vector3(2.7f,0,2.6f);
            World.CompanionLine.transform.position = new Vector3(2.7f,2.4f,2.6f);
            World.CompanionLine.text = "I WILL WATCH YOUR CONES.\nLook for orange among the branches.";
            World.Monkey.position = Nodes[7] + new Vector3(.5f,.3f,.3f);
            instruction = World.Text("GRAB THE BARK. PULL YOURSELF UP.\nAlternate hands. Rest whenever you need.\nFind the cones hidden in the forks.", transform,
                new Vector3(-2,1.65f,4.5f), .10f, World.White.color);
            wrist = World.Text("", Rig.LeftHand, Vector3.zero, .065f, World.White.color);
            wrist.transform.SetParent(transform, true);
            rope = World.Line("Safety belay", transform, World.Orange, .018f); rope.positionCount = 2;
            preview = World.Line("Ancestor rope through real forks", transform, World.Cyan, .025f);
            for (int i=0;i<2;i++)
                gloves[i] = World.Primitive("Anchored glove", PrimitiveType.Sphere, transform, Vector3.zero,
                    Vector3.one*.12f, World.Orange).transform;
            string directory = System.IO.Path.Combine(Application.persistentDataPath,"QuestLogs");
            System.IO.Directory.CreateDirectory(directory);
            journal = new System.IO.StreamWriter(System.IO.Path.Combine(directory,"climb-"+System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")+".log"));
            journal.AutoFlush = true;
            Record("session_started");
        }

        void BuildTree()
        {
            var tree = World.Root("Living tree - climbable bark and forks", transform);
            Vector3 origin = new Vector3(0,0,5);
            for (int i=0;i<8;i++) Nodes.Add(origin + new Vector3(Mathf.Sin(i*.5f)*.35f,i*1.1f,0));
            Nodes.Add(Nodes[3]+new Vector3(-1.1f,.65f,.15f));
            Nodes.Add(Nodes[8]+new Vector3(-1.1f,.6f,.25f));
            Nodes.Add(Nodes[5]+new Vector3(1.15f,.65f,.1f));
            Nodes.Add(Nodes[10]+new Vector3(1.1f,.6f,.3f));
            Nodes.Add(Nodes[7]+new Vector3(-.25f,1.1f,0));
            for(int i=1;i<Nodes.Count;i++)
            {
                Vector3 a=Nodes[Progress.Tree.Parent[i]], b=Nodes[i];
                float radius=i<8?Mathf.Lerp(.65f,.27f,i/8f):.25f;
                Branch(tree,a,b,radius);
                // Shallow raised bark ridges follow the trunk rather than floating ladder rungs.
                for(int j=0;j<5;j++)
                {
                    Vector3 c=Vector3.Lerp(a,b,(j+.5f)/5f);
                    var knot=World.Primitive("Bark knot",PrimitiveType.Sphere,tree,
                        c+new Vector3((j%2==0?-.2f:.2f),0,-radius*.88f),new Vector3(.18f,.25f,.09f),wood);
                    var hit=knot.AddComponent<SphereCollider>(); Bark.Add(hit);
                }
                if(i>=5)
                {
                    for(int j=0;j<3;j++)
                        World.Primitive("Leaf canopy",PrimitiveType.Sphere,tree,b+new Vector3((j-1)*.8f,.8f,.8f),
                            new Vector3(1.6f,1.1f,1.4f),leaves);
                }
            }
            for(int i=0;i<7;i++)
            {
                float angle=i*Mathf.PI*2/7;
                Branch(tree,origin+Vector3.up*.35f,origin+new Vector3(Mathf.Cos(angle)*1.5f,.05f,Mathf.Sin(angle)*1.5f),.23f);
            }
            foreach(int node in new[]{3,7,9,11})
            {
                Vector3 p=Nodes[node]+new Vector3(node==9?-.23f:.25f,.1f,-.45f);
                Branch(tree,Nodes[node],p+Vector3.down*.05f,.19f);
                cones[node]=World.BuildCone(tree,p,.30f);
            }
        }

        void Branch(Transform parent,Vector3 a,Vector3 b,float radius)
        {
            var branch=World.Primitive("Graspable branch",PrimitiveType.Cylinder,parent,(a+b)*.5f,
                new Vector3(radius*2,Vector3.Distance(a,b)*.5f,radius*2),wood);
            branch.transform.rotation=Quaternion.FromToRotation(Vector3.up,b-a);
            var collider=branch.AddComponent<CapsuleCollider>();
            collider.radius=.5f; collider.height=2; Bark.Add(collider);
        }

        void Update()
        {
            Rig.Poll();
            if(Rig.RestartPressed) ResetClimb();
            nearest=NearestNode(Rig.Head.transform.position);
            bool trigger=Rig.XRActive ? UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand)
                .TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton,out bool down) && down : Input.GetKey(KeyCode.Space);
            if(!held[0]&&!held[1]) UpdateRope(trigger);
            else { ropeHeld=false; previousTrigger=trigger; }
            if(!ropeHeld)
            {
                if(Rig.XRActive) UpdateHands(); else DesktopClimb();
            }
            CollectNearby();
            if(!held[0]&&!held[1]&&!ropeHeld && Rig.SnapDirection!=0) Rig.SnapTurn(Rig.SnapDirection);
            for(int i=0;i<2;i++)
            {
                gloves[i].gameObject.SetActive(held[i]); gloves[i].position=anchors[i];
            }
            Vector3 chest=Rig.Head.transform.position-Vector3.up*.4f;
            rope.SetPosition(0,chest); rope.SetPosition(1,Nodes[nearest]+Vector3.up*.3f);
            UpdatePreview();
            wrist.transform.position=Rig.XRActive && Rig.LeftTracked ? Rig.LeftHand.position+Vector3.up*.12f :
                Rig.Head.transform.position+Rig.Head.transform.forward*.8f-Vector3.up*.38f;
            wrist.gameObject.SetActive(!Rig.XRActive || (Rig.LeftTracked && !held[0]));
            wrist.transform.rotation=Rig.Head.transform.rotation;
            wrist.text=Time.time<eventUntil?eventMessage:"CONES "+Progress.Cones+"/4  |  GRIPS "+pulls+"\nANCESTOR ROPE: "+Progress.Reach+" FORK"+(Progress.Reach>1?"S":"")+" / HOLD RIGHT TRIGGER + PULL";
        }

        public int NearestNode(Vector3 point)
        {
            int result=0;float distance=float.MaxValue;
            for(int i=0;i<Nodes.Count;i++) {float d=(Nodes[i]-point).sqrMagnitude;if(d<distance){result=i;distance=d;}}
            return result;
        }

        public bool TouchingBark(Vector3 point,out Vector3 contact)
        {
            float distance=.16f; contact=point;bool found=false;
            foreach(Collider collider in Bark)
            {
                Vector3 p=collider.ClosestPoint(point);float d=Vector3.Distance(point,p);
                if(d<distance){distance=d;contact=p;found=true;}
            }
            return found;
        }

        void UpdateHands()
        {
            Vector3 delta=Vector3.zero;int count=0;
            for(int i=0;i<2;i++)
            {
                bool tracked=i==0?Rig.LeftTracked:Rig.RightTracked;
                bool grip=tracked&&(i==0?Rig.LeftGrip:Rig.RightGrip);
                Transform hand=i==0?Rig.LeftHand:Rig.RightHand;
                if(!grip)held[i]=false;
                if(grip&&!previousGrip[i]&&TouchingBark(hand.position,out Vector3 contact))
                {held[i]=true;anchors[i]=hand.position;pulls++;Rig.Pulse(i==1);Record("bark_grip");}
                if(held[i])
                {
                    Vector3 offset=anchors[i]-hand.position;
                    // A discontinuous tracking pose must release, never throw the player.
                    if(offset.magnitude>.65f) held[i]=false;
                    else {delta+=offset;count++;}
                }
                previousGrip[i]=grip;
            }
            if(count>0) MoveBody(delta/count);
        }

        void MoveBody(Vector3 delta)
        {
            delta=Vector3.ClampMagnitude(delta,.10f);
            Vector3 next=Rig.transform.position+delta;
            next.y=Mathf.Clamp(next.y,0,9);
            // A conservative head sphere prevents pulling the viewpoint through solid bark.
            Vector3 head=Rig.Head.transform.position+(next-Rig.transform.position);
            foreach(Collider bark in Bark)
                if(Vector3.Distance(bark.ClosestPoint(head),head)<.16f) return;
            Rig.transform.position=next;
        }

        void DesktopClimb()
        {
            // Desktop is a reach-limited approximation, not a substitute for two tracked hands.
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray=Rig.MouseRay;
                if(Physics.Raycast(ray,out RaycastHit hit,1.5f)&&Bark.Contains(hit.collider))
                {held[1]=true;anchors[1]=hit.point;pulls++;Record("desktop_grip");}
            }
            if(!Input.GetMouseButton(0))held[1]=false;
            if(held[1])
            {
                Vector3 movement=new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0)*Time.deltaTime*.65f;
                if(Vector3.Distance(Rig.Head.transform.position+movement,anchors[1])<1.7f) MoveBody(movement);
            }
        }

        void UpdateRope(bool trigger)
        {
            if(Rig.XRActive&&!Rig.RightTracked){ropeHeld=false;previousTrigger=trigger;return;}
            // Measure in tracking space: moving the rig must not manufacture another pull.
            Vector3 hand=Rig.XRActive?Rig.RightHand.localPosition:Rig.Head.transform.localPosition;
            if(trigger&&!previousTrigger)
            {
                ropeTarget=Progress.Ancestor(nearest);ropeFrom=nearest;ropeDistance=0;
                ropeHeld=ropeTarget>=0;
                previousRopeHand=hand;
                if(ropeHeld){pulls++;Record("ancestor_rope_grip");}
            }
            if(!trigger)ropeHeld=false;
            if(ropeHeld)
            {
                // Only deliberate hand movement advances the rope; holding a button does not travel.
                float pull=Rig.XRActive?Mathf.Max(0,Vector3.Dot(hand-previousRopeHand,-(Rig.Head.transform.localRotation*Vector3.forward))):
                    Mathf.Max(0,-Input.GetAxis("Mouse Y"))*.025f;
                previousRopeHand=hand;
                if(pull>.20f){ropeHeld=false;return;}
                if(pull<=0)return;
                ropeDistance+=pull*Progress.Reach;
                while(ropeHeld)
                {
                    int parent=Progress.Tree.Parent[ropeFrom];
                    if(parent<0){ropeHeld=false;break;}
                    float length=Vector3.Distance(Nodes[ropeFrom],Nodes[parent]);
                    Vector3 a=Nodes[ropeFrom]+new Vector3(0,-1.2f,-1.0f);
                    Vector3 b=Nodes[parent]+new Vector3(0,-1.2f,-1.0f);
                    Vector3 destination=Vector3.Lerp(a,b,Mathf.Clamp01(ropeDistance/length));
                    destination.y=Mathf.Max(0,destination.y);
                    // Bounded movement; no instant displacement and no automatic camera rotation.
                    MoveBody(Vector3.ClampMagnitude(destination-Rig.transform.position,Time.deltaTime*1.2f));
                    if(ropeDistance<length || Vector3.Distance(Rig.transform.position,destination)>.1f)break;
                    ropeDistance-=length;ropeFrom=parent;
                    if(parent==ropeTarget)ropeHeld=false;
                }
            }
            previousTrigger=trigger;
        }

        void UpdatePreview()
        {
            int target=ropeHeld?ropeTarget:Progress.Ancestor(nearest);
            preview.enabled=target>=0;
            if(target<0)return;
            var route=new List<Vector3>();int node=ropeHeld?ropeFrom:nearest;
            while(node>=0)
            {
                route.Add(Nodes[node]+new Vector3(0,0,-.85f));
                if(node==target)break;
                node=Progress.Tree.Parent[node];
            }
            preview.positionCount=route.Count;preview.SetPositions(route.ToArray());
        }

        void CollectNearby()
        {
            foreach(var pair in cones)
            {
                if(Progress.Collected(pair.Key))continue;
                Vector3 center=pair.Value.position+Vector3.up*.12f;
                bool reached=Rig.XRActive?
                    (Rig.LeftTracked&&Rig.LeftGrip&&Vector3.Distance(Rig.LeftHand.position,center)<.25f)||
                    (Rig.RightTracked&&Rig.RightGrip&&Vector3.Distance(Rig.RightHand.position,center)<.25f):
                    Input.GetKeyDown(KeyCode.F)&&Vector3.Distance(Rig.Head.transform.position,center)<1.3f;
                if(!reached||!Progress.Collect(pair.Key))continue;
                pair.Value.gameObject.SetActive(false);
                World.TrophyCones[Progress.Cones-1].gameObject.SetActive(true);
                eventMessage=Progress.Cones==4?"ALL CONES FOUND.\nFollow your rope back to your companion.":
                    "CONE FOUND / "+Progress.Reach+"-FORK ROPE\nPull back along the cyan ancestor path.";
                eventUntil=Time.time+5;
                World.CompanionLine.text="CONES SAFE: "+Progress.Cones+"/4\nI am here when you return.";
                Rig.Pulse(true,.4f,.1f);Record("cone_collected");
            }
        }

        public void ResetClimb()
        {
            Progress=new ClimbProgress();held[0]=held[1]=ropeHeld=false;
            previousGrip[0]=previousGrip[1]=previousTrigger=false;pulls=0;eventUntil=0;
            Rig.TeleportFeet(new Vector3(0,0,3.75f));
            foreach(var cone in cones.Values)cone.gameObject.SetActive(true);
            foreach(var trophy in World.TrophyCones)trophy.gameObject.SetActive(false);
            World.CompanionLine.text="I WILL WATCH YOUR CONES.\nLook for orange among the branches.";
            Record("restart");
        }
        void Record(string action)
        {
            journal?.WriteLine(System.DateTime.UtcNow.ToString("o")+" event="+action+" cones="+Progress.Cones+" grips="+pulls+" node="+nearest);
        }
        void OnDestroy()
        {
            journal?.Dispose();
            if(World!=null)Destroy(World.gameObject);
            if(Rig!=null)Destroy(Rig.gameObject);
        }
    }
}
