using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace BinaryLift
{
    public sealed class AdventureController : MonoBehaviour
    {
        public AdventureState State { get; private set; }
        public XRRig Rig { get; private set; }
        public AdventureWorld World { get; private set; }
        public bool Busy { get; private set; }
        public int Checkpoint { get; private set; }
        Vector3[] route;
        public int CheckpointCount => route.Length;
        AdventureTarget hover;
        LineRenderer rayLine;
        Transform cursor, fade;
        Material fadeMaterial;
        readonly bool[] held=new bool[2],previousGrip=new bool[2];
        readonly Vector3[] previousLocal=new Vector3[2],anchor=new Vector3[2];
        readonly Transform[] gloves=new Transform[2],arms=new Transform[2];
        readonly AdventureTarget[] nearGrip=new AdventureTarget[2];
        Vector3 walkDestination;
        bool walking,activeRight=true,desktopHeld;
        Vector3 mousePrevious;
        float animationClock,flashUntil;
        string flash="";
        StreamWriter journal;
        Quaternion horseFacing;

        void Awake()
        {
            State=new AdventureState();Rig=XRRig.Create();
            World=new GameObject("Cone Courier adventure").AddComponent<AdventureWorld>();World.Build(Rig,State);
            route=new Vector3[State.RequiredLevels+1];
            for(int i=0,n=State.StartNode;i<route.Length;i++,n=State.Tree.Parent[n]){route[i]=World.CampusNodes[n];route[i].y=0;}
            foreach(var camera in FindObjectsByType<Camera>(FindObjectsSortMode.None))if(camera!=Rig.Head)Destroy(camera.gameObject);
            RenderSettings.skybox=null;RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight=new Color(.57f,.64f,.70f);
            Rig.Head.backgroundColor=new Color(.055f,.10f,.15f);
            RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=55;RenderSettings.fogEndDistance=120;RenderSettings.fogColor=Rig.Head.backgroundColor;
            var sun=new GameObject("Late afternoon light").AddComponent<Light>();sun.transform.SetParent(transform);
            sun.type=LightType.Directional;sun.transform.rotation=Quaternion.Euler(38,-28,0);sun.color=new Color(1,.85f,.68f);sun.intensity=1.1f;
            QualitySettings.antiAliasing=4;
            rayLine=World.Art.Line("Controller selection ray",transform,World.Art.Cyan,.007f);rayLine.positionCount=2;
            cursor=World.Art.Primitive("Interaction cursor",PrimitiveType.Sphere,transform,Vector3.zero,Vector3.one*.018f,World.Art.Cyan).transform;
            fadeMaterial=World.Art.Mat(new Color(0,0,0,0),true);fadeMaterial.renderQueue=5000;
            fade=World.Art.Primitive("Comfort transition",PrimitiveType.Quad,Rig.Head.transform,new Vector3(0,0,.065f),Vector3.one*4,fadeMaterial).transform;fade.gameObject.SetActive(false);
            for(int i=0;i<2;i++)
            {
                gloves[i]=World.Art.Primitive("Grip glove",PrimitiveType.Cube,transform,Vector3.zero,new Vector3(.095f,.075f,.13f),World.Art.Orange).transform;
                arms[i]=World.Art.Primitive("Forearm sleeve",PrimitiveType.Capsule,transform,Vector3.zero,new Vector3(.075f,.15f,.075f),World.Art.Dark).transform;
                gloves[i].gameObject.SetActive(false);arms[i].gameObject.SetActive(false);
            }
            Rig.TeleportFeet(new Vector3(0,0,-8));horseFacing=World.Art.Horse.rotation;
            string directory=Path.Combine(Application.persistentDataPath,"QuestLogs");Directory.CreateDirectory(directory);
            journal=new StreamWriter(Path.Combine(directory,"adventure-"+System.DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff")+".log")){AutoFlush=true};
            Record("arrival");Refresh();
        }
        void Update()
        {
            Rig.Poll();
            if(Rig.RightSelectPressed)activeRight=true;else if(Rig.LeftSelectPressed)activeRight=false;
            UpdatePointer();
            if(!Busy)
            {
                if(Rig.RestartPressed){StartCoroutine(Restart());return;}
                if(Rig.XRActive && State.Phase!=AdventurePhase.Climbing && (Rig.RightSelectPressed||Rig.LeftSelectPressed) && hover!=null)Activate(hover.Action);
                if(State.Phase==AdventurePhase.Arrival||State.Phase==AdventurePhase.Courtyard||State.Phase==AdventurePhase.Chase)Walk();
                if(State.Phase==AdventurePhase.Climbing)ClimbInput();
                else CheckNearInteraction();
                if(!held[0]&&!held[1]&&!desktopHeld&&Rig.SnapDirection!=0)StartCoroutine(Snap(Rig.SnapDirection));
            }
            float doorOpen=Rig.transform.position.z>-4 ? 2.6f : 0;
            World.Door.localPosition=Vector3.Lerp(World.Door.localPosition,new Vector3(doorOpen,0,-1),Time.deltaTime*3);
            AnimateMonkey();UpdateGloves();UpdateInstructions();
            previousGrip[0]=Rig.LeftGrip;previousGrip[1]=Rig.RightGrip;
        }
        void UpdatePointer()
        {
            if(hover!=null)hover.Highlight(false);hover=null;
            Ray ray;
            if(Rig.XRActive){if(!Rig.TryGetHandRay(activeRight,out ray))ray=Rig.GazeRay;}
            else ray=Rig.MouseRay;
            Vector3 point=ray.GetPoint(40);
            if(!Busy&&Physics.Raycast(ray,out var hit,70))
            {hover=hit.collider.GetComponentInParent<AdventureTarget>();point=hit.point;}
            if(hover!=null)hover.Highlight(true);
            cursor.gameObject.SetActive(hover!=null&&!Busy);cursor.position=point;
            cursor.localScale=Vector3.one*Mathf.Clamp(Vector3.Distance(ray.origin,point)*.008f,.012f,.10f);
            rayLine.enabled=Rig.XRActive&&!Busy;rayLine.SetPosition(0,ray.origin);rayLine.SetPosition(1,point);
            if(State.Phase==AdventurePhase.Choosing)
            {
                if(hover!=null&&(SpanFor(hover.Action)>0))
                    World.PreviewRoute(State,SpanFor(hover.Action));
                else World.Preview.enabled=false;
            }
        }
        static int SpanFor(AdventureAction action)
        {
            switch(action){case AdventureAction.OneLevel:return 1;case AdventureAction.TwoLevels:return 2;case AdventureAction.FourLevels:return 4;case AdventureAction.EightLevels:return 8;default:return 0;}
        }
        public bool Activate(AdventureAction action)
        {
            if(Busy)return false;
            if(action==AdventureAction.Replay && State.Phase==AdventurePhase.Complete){StartCoroutine(Restart());return true;}
            if(action==AdventureAction.Exit && (State.Phase==AdventurePhase.Arrival||State.Phase==AdventurePhase.Courtyard))
            {walkDestination=new Vector3(0,0,2.5f);walking=true;return true;}
            if(action==AdventureAction.Waypoint && State.Phase==AdventurePhase.Chase)
            {walkDestination=route[Checkpoint];walking=true;return true;}
            int span=SpanFor(action);
            if(span>0)
            {
                if(!State.Choose(span)){Say("That connection passes the goal. Choose a shorter route.");return false;}
                World.BuildGrips(State,Rig.Head.transform.position.y-Rig.transform.position.y);
                World.PreviewRoute(State,span);ReleaseHands();Refresh();Record("connection_selected_"+span);return true;
            }
            if(action==AdventureAction.Cone && State.CollectCone())
            {
                World.Cone.gameObject.SetActive(false);World.Art.TrophyCones[0].gameObject.SetActive(true);
                Rig.Pulse(activeRight,.45f,.12f);Refresh();Record("cone_recovered_journey_complete");return true;
            }
            return false;
        }
        IEnumerator Theft()
        {
            Busy=true;walking=false;World.Intro.gameObject.SetActive(false);Record("theft_anticipation");
            Transform monkey=World.Art.Monkey;
            monkey.rotation=Quaternion.LookRotation(monkey.position-World.Notebook.position);
            Vector3 from=monkey.position;
            for(float t=0;t<.55f;t+=Time.deltaTime)
            {monkey.position=from+Vector3.up*Mathf.Sin(t/.55f*Mathf.PI)*.12f;yield return null;}
            Vector3 snatch=World.Notebook.position+new Vector3(.35f,-.4f,.25f);
            for(float t=0;t<.65f;t+=Time.deltaTime)
            {
                monkey.position=Vector3.Lerp(from,snatch,Mathf.SmoothStep(0,1,t/.65f));
                foreach(Transform limb in monkey)if(limb.name=="Arm")limb.localRotation=Quaternion.Euler(-65*t/.65f,0,0);
                yield return null;
            }
            World.Notebook.gameObject.SetActive(false);monkey.Find("Stolen route notebook").gameObject.SetActive(true);Record("notebook_snatched");
            Quaternion facing=monkey.rotation,target=Quaternion.LookRotation(monkey.position-route[0]);
            for(float t=0;t<.45f;t+=Time.deltaTime){monkey.rotation=Quaternion.Slerp(facing,target,t/.45f);yield return null;}
            World.Art.Horse.rotation=Quaternion.LookRotation(new Vector3(-1,0,3));World.Art.CompanionLine.text="AFTER THAT MONKEY!";
            World.Marker.gameObject.SetActive(true);World.Marker.position=route[0];Busy=false;Refresh();
        }
        void Walk()
        {
            Vector2 input=Vector2.zero;
            if(Rig.XRActive)InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis,out input);
            else input=new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
            if(input.sqrMagnitude>.02f)walking=false;
            Vector3 forward=Vector3.ProjectOnPlane(Rig.Head.transform.forward,Vector3.up).normalized;
            Vector3 movement=(forward*input.y+Vector3.Cross(Vector3.up,forward)*input.x);
            if(walking)movement=(walkDestination-Rig.transform.position).normalized;
            movement.y=0;movement=Vector3.ClampMagnitude(movement,1);
            Vector3 next=Rig.transform.position+movement*Time.deltaTime*1.7f;
            next.x=Mathf.Clamp(next.x,next.z<0?-4.4f:-2.6f,next.z<0?4.4f:2.6f);next.z=Mathf.Clamp(next.z,-11,43);next.y=0;
            // Keep the classroom exit and furniture solid without a moving camera collider.
            if(next.z<-.8f && next.z>-1.25f && Mathf.Abs(next.x)>1.05f)next=Rig.transform.position;
            if(next.z<-1.2f && next.z>-10.5f && Mathf.Abs(next.x)>1.3f && Mathf.Abs(next.x)<4.1f)
            {
                for(int z=-9;z<=-3;z+=3)if(Mathf.Abs(next.z-z)<1.3f){next=Rig.transform.position;break;}
            }
            Rig.transform.position=next;
            if(State.Phase==AdventurePhase.Arrival && next.z>-.5f){State.ExitClassroom();Record("classroom_exited");Refresh();}
            if(State.Phase==AdventurePhase.Courtyard && next.z>2.1f){State.BeginChase();StartCoroutine(Theft());return;}
            if(State.Phase!=AdventurePhase.Chase)return;
            if(Vector3.Distance(new Vector3(Rig.Head.transform.position.x,0,Rig.Head.transform.position.z),route[Checkpoint])<1.0f)
            {
                Record("chase_checkpoint_"+Checkpoint);walking=false;Checkpoint++;
                if(Checkpoint==route.Length){State.ReachGate();StartCoroutine(MapTransition());return;}
                World.Marker.position=route[Checkpoint];Refresh();
            }
        }
        void AnimateMonkey()
        {
            if(Busy||State.Phase!=AdventurePhase.Chase)return;
            Vector3 to=route[Checkpoint];Vector3 feet=Rig.transform.position;
            Vector3 direction=(to-feet).normalized;
            Vector3 target=Vector3.Distance(to,feet)>4?feet+direction*4:to;
            Vector3 previous=World.Art.Monkey.position;
            World.Art.Monkey.position=Vector3.MoveTowards(previous,new Vector3(target.x,.45f,target.z),Time.deltaTime*3.5f);
            animationClock+=Time.deltaTime;
            Vector3 p=World.Art.Monkey.position;p.y=.45f+Mathf.Abs(Mathf.Sin(animationClock*5))*.23f;World.Art.Monkey.position=p;
            if(direction.sqrMagnitude>.001f)World.Art.Monkey.rotation=Quaternion.Slerp(World.Art.Monkey.rotation,Quaternion.LookRotation(new Vector3(-direction.x,0,-direction.z)),Time.deltaTime*7);
            var head=World.Art.Monkey.Find("Head");if(head!=null)head.localRotation=Quaternion.Euler(0,Mathf.Sin(animationClock*1.7f)*28,0);
            foreach(Transform child in World.Art.Monkey)
                if(child.name=="Arm")child.localRotation=Quaternion.Euler(Mathf.Sin(animationClock*7+child.localPosition.x*5)*28,0,0);
        }
        IEnumerator MapTransition()
        {
            Busy=true;walking=false;World.Marker.gameObject.SetActive(false);Record("aerial_reveal_started");
            Vector3 start=Rig.transform.position;
            Vector3 aerial=new Vector3(0,32,9);
            if(Rig.XRActive)
            {
                // A stationary overlook preserves head tracking and avoids forced camera pitch.
                yield return Fade(0,1,.35f);Rig.TeleportFeet(aerial);yield return Fade(1,0,.4f);
            }
            else
            {
                for(float t=0;t<2.8f;t+=Time.deltaTime)
                {float k=Mathf.SmoothStep(0,1,t/2.8f);Rig.TeleportFeet(Vector3.Lerp(start,aerial,k));Rig.SetDesktopView(Mathf.Lerp(0,65,k));yield return null;}
            }
            Say("THE ROUTES FORM A TREE / EACH JUNCTION IS A NODE",5);
            for(int i=0;i<World.Roads.Count;i++){World.Roads[i].startWidth=.16f;World.Roads[i].endWidth=.16f;}
            World.MapLine.enabled=true;World.MapLine.positionCount=route.Length;
            for(int i=0;i<route.Length;i++)World.MapLine.SetPosition(i,route[i]+Vector3.up*.25f);
            yield return new WaitForSeconds(2);
            // The same graph rises above its traced street coordinates before scene change.
            for(float t=0;t<2;t+=Time.deltaTime)
            {
                float k=Mathf.SmoothStep(0,1,t/2);
                for(int i=1;i<State.Tree.Count;i++)
                {
                    int parent=State.Tree.Parent[i];
                    Vector3 a=World.CampusNodes[i],b=World.CampusNodes[parent];
                    a.x*=1+2*k;b.x*=1+2*k;
                    World.Roads[i-1].SetPosition(0,a+Vector3.up*World.Positions[i].y*k*.35f);
                    World.Roads[i-1].SetPosition(1,b+Vector3.up*World.Positions[parent].y*k*.35f);
                }
                for(int i=0,n=State.StartNode;i<route.Length;i++,n=State.Tree.Parent[n])
                {Vector3 p=route[i];p.x*=1+2*k;p.y=.25f+World.Positions[n].y*k*.35f;World.MapLine.SetPosition(i,p);}
                yield return null;
            }
            yield return new WaitForSeconds(1.2f);
            yield return Fade(0,1,.4f);World.MapLine.enabled=false;World.Course.gameObject.SetActive(true);
            if(!Rig.XRActive)Rig.transform.rotation=Quaternion.identity;Rig.SetDesktopView(0);
            // Establish the retained original topology before moving into reaching distance.
            Rig.TeleportFeet(World.CourseOrigin+new Vector3(0,7,-18));
            Say("THE ORIGINAL TREE / FOLLOW ITS BRANCHES TO THE GOLD NODE",3);
            yield return Fade(1,0,.4f);yield return new WaitForSeconds(2.5f);yield return Fade(0,1,.35f);
            State.EnterTree();
            Rig.TeleportFeet(World.CourseOrigin+World.Positions[State.Node]+AdventureWorld.ClimbOffset);
            World.Art.Monkey.position=World.CourseOrigin+World.Positions[State.Goal]+new Vector3(-1.8f,.1f,1.8f);
            World.Art.Horse.position=World.CourseOrigin+World.Positions[State.Goal]+new Vector3(1.8f,0,2);
            World.Art.CompanionLine.transform.position=World.Art.Horse.position+Vector3.up*2.3f;
            World.Art.CompanionLine.text="FOLLOW THE BRANCHES. I WILL MEET YOU ABOVE.";
            flashUntil=0;Refresh();yield return Fade(1,0,.45f);Busy=false;Record("tree_arrival");
        }
        void CheckNearInteraction()
        {
            if(!Rig.XRActive)return;
            AdventureTarget target=State.Phase==AdventurePhase.Clue?World.ConeTarget:null;
            if(target==null)return;
            for(int i=0;i<2;i++)
            {
                bool tracked=i==0?Rig.LeftTracked:Rig.RightTracked,grip=i==0?Rig.LeftGrip:Rig.RightGrip;
                Transform hand=i==0?Rig.LeftHand:Rig.RightHand;
                if(tracked&&grip&&!previousGrip[i]&&Vector3.Distance(target.GetComponent<Collider>().ClosestPoint(hand.position),hand.position)<.12f)
                {activeRight=i==1;Activate(target.Action);return;}
            }
        }
        void ClimbInput()
        {
            if(!Rig.XRActive)return; // Desktop drag events are consumed in OnGUI.
            float amount=0;int count=0;
            for(int i=0;i<2;i++)
            {
                Transform hand=i==0?Rig.LeftHand:Rig.RightHand;
                bool tracked=i==0?Rig.LeftTracked:Rig.RightTracked,grip=tracked&&(i==0?Rig.LeftGrip:Rig.RightGrip);
                if(!grip)held[i]=false;
                if(nearGrip[i]!=null)nearGrip[i].Highlight(false);
                nearGrip[i]=null;
                if(tracked&&!held[i])
                {
                    float nearest=.14f;AdventureTarget best=null;
                    foreach(var handle in World.GripTargets)
                    {
                        float d=Vector3.Distance(handle.GetComponent<Collider>().ClosestPoint(hand.position),hand.position);
                        if(d<nearest){nearest=d;best=handle;}
                    }
                    nearGrip[i]=best;
                    if(best!=null)
                    {
                        best.Highlight(true);
                        if(grip&&!previousGrip[i]){held[i]=true;anchor[i]=hand.position;previousLocal[i]=hand.localPosition;Rig.Pulse(i==1);Record("tracked_grip");}
                    }
                }
                if(held[i])
                {
                    Vector3 delta=previousLocal[i]-hand.localPosition;previousLocal[i]=hand.localPosition;
                    if(delta.magnitude>.2f){held[i]=false;continue;}
                    amount+=Mathf.Max(0,delta.y);count++;
                }
            }
            if(count>0)ApplyPull(amount/count);
        }
        void OnGUI()
        {
            if(Rig==null||Rig.XRActive||Busy)return;
            Event input=Event.current;
            if(input.button!=0)return;
            Vector3 screen=new Vector3(input.mousePosition.x,Screen.height-input.mousePosition.y,0);
            if(input.type==EventType.MouseDown)
            {
                if(Physics.Raycast(Rig.Head.ScreenPointToRay(screen),out var hit,70))
                {
                    var handle=hit.collider.GetComponentInParent<AdventureTarget>();
                    if(handle!=null&&State.Phase!=AdventurePhase.Climbing)
                    {Activate(handle.Action);input.Use();}
                    else if(handle!=null&&handle.Action==AdventureAction.Grip)
                    {desktopHeld=true;mousePrevious=screen;anchor[1]=handle.transform.position;Record("desktop_grip");input.Use();}
                }
            }
            else if(desktopHeld&&(input.type==EventType.MouseDrag||input.type==EventType.MouseUp))
            {
                bool released=input.type==EventType.MouseUp;
                // Events preserve short strokes that begin and end between rendered frames.
                float pull=Mathf.Clamp((mousePrevious.y-screen.y)/Mathf.Max(Screen.height,1)*1.8f,0,.8f);
                mousePrevious=screen;
                while(pull>0&&State.Phase==AdventurePhase.Climbing)
                {float sample=Mathf.Min(pull,.1f);ApplyPull(sample);pull-=sample;}
                if(released)desktopHeld=false;
                input.Use();
            }
        }
        void OnApplicationFocus(bool focused){if(!focused)ReleaseHands();}
        public bool ApplyPull(float metres)
        {
            int before=State.TotalLevels;
            if(Busy||!State.Pull(metres))return false;
            Vector3 position=World.Positions[State.Node];
            if(State.Phase==AdventurePhase.Climbing)position=Vector3.Lerp(position,World.Positions[State.Tree.Parent[State.Node]],State.EdgeProgress);
            Rig.TeleportFeet(World.CourseOrigin+position+AdventureWorld.ClimbOffset);
            World.SafetyDeck.localPosition=position+AdventureWorld.ClimbOffset-Vector3.up*.09f;
            if(State.TotalLevels>before){Rig.Pulse(true,.3f,.06f);Record("level_crossed_"+State.TotalLevels);}
            if(State.Phase!=AdventurePhase.Climbing)
            {ReleaseHands();World.Grips.gameObject.SetActive(false);World.Preview.enabled=false;Record("selection_finished");Refresh();}
            return true;
        }
        void UpdateGloves()
        {
            for(int i=0;i<2;i++)
            {
                bool visible=held[i] || (i==1&&desktopHeld);
                gloves[i].gameObject.SetActive(visible);arms[i].gameObject.SetActive(visible);
                if(!visible)continue;
                gloves[i].position=anchor[i];
                Vector3 elbow=Rig.Head.transform.position+Rig.Head.transform.right*(i==0?-.25f:.25f)-Vector3.up*.35f;
                Vector3 vector=anchor[i]-elbow;
                arms[i].position=(anchor[i]+elbow)*.5f;arms[i].rotation=Quaternion.FromToRotation(Vector3.up,vector);
                arms[i].localScale=new Vector3(.075f,Mathf.Min(vector.magnitude,.7f)*.5f,.075f);
            }
        }
        void ReleaseHands(){held[0]=held[1]=desktopHeld=false;}
        IEnumerator Restart()
        {
            Busy=true;ReleaseHands();walking=false;yield return Fade(0,1,.25f);
            State=new AdventureState();Checkpoint=0;flashUntil=0;
            World.Course.gameObject.SetActive(false);World.Intro.gameObject.SetActive(true);World.Notebook.gameObject.SetActive(true);
            World.Cone.gameObject.SetActive(true);World.EndCard.gameObject.SetActive(false);World.Marker.gameObject.SetActive(false);World.MapLine.enabled=false;
            World.Grips.gameObject.SetActive(false);World.Preview.enabled=false;World.Art.TrophyCones[0].gameObject.SetActive(false);
            World.Art.Horse.position=new Vector3(1.95f,0,3);World.Art.Horse.rotation=horseFacing;
            World.Art.CompanionLine.transform.position=new Vector3(1.95f,2.35f,3);World.Art.CompanionLine.text="YOUR CAMPUS COMPANION";
            World.Art.Monkey.position=new Vector3(-1,.4f,4.2f);if(!Rig.XRActive)Rig.transform.rotation=Quaternion.identity;Rig.SetDesktopView(0);Rig.TeleportFeet(new Vector3(0,0,-8));
            for(int i=1;i<State.Tree.Count;i++){var road=World.Roads[i-1];road.SetPosition(0,World.CampusNodes[i]);road.SetPosition(1,World.CampusNodes[State.Tree.Parent[i]]);road.startWidth=road.endWidth=.07f;}
            Refresh();
            World.Art.Monkey.Find("Stolen route notebook").gameObject.SetActive(false);
            yield return Fade(1,0,.35f);Busy=false;Record("story_restarted");
        }
        IEnumerator Snap(int direction)
        {
            Busy=true;yield return Fade(0,1,.08f);Rig.SnapTurn(direction);yield return Fade(1,0,.12f);Busy=false;
        }
        IEnumerator Fade(float from,float to,float duration)
        {
            fade.gameObject.SetActive(true);
            for(float t=0;t<duration;t+=Time.unscaledDeltaTime){fadeMaterial.color=new Color(0,0,0,Mathf.Lerp(from,to,t/duration));yield return null;}
            fadeMaterial.color=new Color(0,0,0,to);fade.gameObject.SetActive(to>0);
        }
        void Refresh()
        {
            bool choosing=State.Phase==AdventurePhase.Choosing;
            World.Controls.gameObject.SetActive(choosing);World.Controls.localPosition=World.Positions[State.Node]+AdventureWorld.ClimbOffset;
            World.SafetyDeck.localPosition=World.Positions[State.Node]+AdventureWorld.ClimbOffset-Vector3.up*.09f;
            for(int i=0;i<World.Platforms.Count;i++)World.Platforms[i].sharedMaterial.color=i==State.Node?new Color(.65f,.9f,.85f):i==State.Goal?new Color(.85f,.49f,.13f):new Color(.18f,.30f,.36f);
            for(int i=0;i<World.Choices.Count;i++)
            {
                var choice=World.Choices[i];bool available=State.CanChoose(1<<i);
                choice.GetComponent<Collider>().enabled=available;choice.BaseColor=available?World.Art.Dark.color:new Color(.12f,.13f,.14f);choice.Highlight(false);
                choice.transform.parent.GetComponentInChildren<TextMesh>().color=available?World.Art.White.color:new Color(.4f,.42f,.44f);
            }
            World.RouteTitle.text="CHOOSE YOUR NEXT CONNECTION\n"+State.Remaining+" LEVEL"+(State.Remaining==1?"":"S")+" TO THE GOLD NODE\nSelect, then grab and pull along the branches.";
            World.EndCard.gameObject.SetActive(State.Phase==AdventurePhase.Complete);
            World.Art.CompanionLine.gameObject.SetActive(State.Phase!=AdventurePhase.Complete&&State.Phase!=AdventurePhase.Clue&&State.Phase!=AdventurePhase.Climbing&&State.Phase!=AdventurePhase.Choosing);
            World.Instructions.gameObject.SetActive(!World.EndCard.gameObject.activeSelf&&!choosing);
            World.EndCard.localPosition=World.Positions[State.Goal]+AdventureWorld.ClimbOffset;
            World.Outcome.text="CLIMBED "+State.TotalLevels+" / "+State.RequiredLevels+" LEVELS\n"+State.Selections+" CONNECTIONS / 1 CONE RECOVERED\nLonger selections reduce stops, not the climb.";
            World.MarkerLabel.text="FOLLOW THE MONKEY / "+Mathf.Min(Checkpoint+1,route.Length)+" OF "+route.Length+"\nCLICK TO WALK HERE";
        }
        void UpdateInstructions()
        {
            World.Status.text=State.Phase==AdventurePhase.Arrival?"01 / AFTER CLASS":State.Phase==AdventurePhase.Courtyard?"02 / THE COURTYARD":State.Phase==AdventurePhase.Chase?"03 / FOLLOW THE MONKEY":State.Phase==AdventurePhase.Mapping?"04 / THE ROUTE REVEALED":"CLIMBED "+State.TotalLevels+" / "+State.RequiredLevels+" LEVELS";
            string hint;
            switch(State.Phase)
            {
                case AdventurePhase.Arrival:hint="WASD / left stick: explore. Exit through the lit door.";break;
                case AdventurePhase.Courtyard:hint="Walk towards your notebook in the courtyard.";break;
                case AdventurePhase.Chase:hint="Follow the orange marker. Walk or click to move.";break;
                case AdventurePhase.Mapping:hint="Look down: the monkey followed these ancestor paths.";break;
                case AdventurePhase.Choosing:hint="Choose a connection, then climb it yourself.";break;
                case AdventurePhase.Climbing:hint=(Rig.XRActive?"GRIP + PULL DOWN":"DRAG AN ORANGE HOLD DOWN")+" / THIS ROUTE "+State.CompletedInSelection+" OF "+State.Span;break;
                case AdventurePhase.Clue:hint="Clue reached! Grip nearby or click the orange cone.";break;
                default:hint="Journey complete. Replay from the classroom.";break;
            }
            World.Hint.text=Time.time<flashUntil?flash:hint;
        }
        void Say(string message,float seconds=3){flash=message;flashUntil=Time.time+seconds;}
        void Record(string action)
        {
            string line=System.DateTime.UtcNow.ToString("o")+" event="+action+" phase="+State.Phase+" node="+State.Node+" climbed="+State.TotalLevels+" selected="+State.Span+" crossed="+State.CompletedInSelection+" selections="+State.Selections;
            journal?.WriteLine(line);Debug.Log("Adventure | "+line);
        }
        void OnDestroy(){journal?.Dispose();if(World!=null)Destroy(World.gameObject);if(Rig!=null)Destroy(Rig.gameObject);}
    }
}
