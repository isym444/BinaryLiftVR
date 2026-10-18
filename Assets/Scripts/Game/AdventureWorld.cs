using System.Collections.Generic;
using UnityEngine;

namespace BinaryLift
{
    public enum AdventureAction { Exit, Waypoint, OneLevel, TwoLevels, FourLevels, EightLevels, Grip, Cone, Replay, LinkStart, LinkEnd, NextTarget }
    public sealed class AdventureTarget : MonoBehaviour
    {
        public AdventureAction Action;
        public Renderer Surface;
        public Color BaseColor;
        MaterialPropertyBlock properties;
        public void Highlight(bool active)
        {
            if(Surface==null)return;
            if(properties==null)properties=new MaterialPropertyBlock();
            properties.SetColor("_Color",active?new Color(1,.68f,.23f):BaseColor);
            Surface.SetPropertyBlock(properties);
        }
    }

    public sealed class AdventureWorld : MonoBehaviour
    {
        public CampusWorld Art { get; private set; }
        public Transform Course, Controls, Grips, Notebook, Cone, Marker, Intro, EndCard;
        public Transform SafetyDeck, Instructions, Classroom, Door, Loom, NewSearch, RecoveryBalcony;
        public TextMesh LoomTitle, LoomNodes, LoomHint, SearchTitle, MemoryLabel;
        public AdventureTarget LinkStart, LinkEnd;
        public LineRenderer Thread;
        readonly List<LineRenderer> remembered = new List<LineRenderer>();
        public Vector3[] CampusNodes;
        public readonly List<AdventureTarget> Choices=new List<AdventureTarget>();
        public readonly List<LineRenderer> Roads=new List<LineRenderer>();
        AdventureState state;
        public static readonly Vector3 ClimbOffset=Vector3.back*.85f;
        public AdventureTarget ConeTarget;
        public TextMesh Status, Hint, RouteTitle, MarkerLabel, Outcome;
        public readonly List<AdventureTarget> GripTargets=new List<AdventureTarget>();
        public readonly List<Renderer> Platforms=new List<Renderer>();
        public Vector3[] Positions;
        public readonly Vector3 CourseOrigin=new Vector3(0,0,100);
        public LineRenderer Preview, MapLine;
        Material silver, bark;

        public void Build(XRRig rig, AdventureState model)
        {
            state=model;
            Art=new GameObject("Gilmorehill - evening courtyard").AddComponent<CampusWorld>();
            Art.transform.SetParent(transform,false);Art.Build(new CampusQuestState(),true);
            Art.SaddleTarget.enabled=false;Art.SaddleTarget.GetComponent<Collider>().enabled=false;
            Art.CompanionLine.text="YOUR CAMPUS COMPANION";
            Art.Monkey.position=new Vector3(-1,.4f,4.2f);
            Art.Monkey.Find("Stolen route notebook").gameObject.SetActive(false);
            silver=Art.Mat(new Color(.35f,.48f,.52f));bark=Art.Mat(new Color(.20f,.15f,.10f));bark.SetFloat("_Glossiness",.05f);
            BuildCourtyard();BuildClassroom();
            Intro=Art.Root("Classroom welcome",Classroom);
            Panel("CONE COURIER", "UNIVERSITY OF GLASGOW / AFTER HOURS\nExplore the classroom, then head outside.",Intro,new Vector3(0,2.85f,-1.18f),new Vector2(3.7f,1.1f));
            Button("EXIT TO COURTYARD",Intro,new Vector3(0,1.4f,-1.17f),AdventureAction.Exit,2.1f);
            Notebook=Art.Root("Research funding envelope",transform);Notebook.position=new Vector3(0,1.1f,3.8f);
            Art.Box("Grant paperwork",Notebook,Vector3.zero,new Vector3(.55f,.08f,.36f),Art.White);
            Art.Box("Grant envelope",Notebook,new Vector3(0,.06f,0),new Vector3(.58f,.045f,.39f),Art.Orange);
            Art.Box("Reading pedestal",transform,new Vector3(0,.52f,3.8f),new Vector3(.7f,1.04f,.5f),Art.Dark);
            Art.Text("RESEARCH FUND\nNOT FOR BANANAS",Notebook,new Vector3(0,.35f,-.05f),.10f,Art.White.color);
            Marker=Art.Root("Chase destination",transform);
            Art.Primitive("Waypoint ring",PrimitiveType.Cylinder,Marker,new Vector3(0,.035f,0),new Vector3(1.4f,.02f,1.4f),Art.Orange);
            var beacon=Art.Box("Waypoint beacon",Marker,new Vector3(0,.7f,0),new Vector3(.12f,1.4f,.12f),Art.Cyan);
            Target(beacon,AdventureAction.Waypoint,new Vector3(8,1.4f,8));
            MarkerLabel=Art.Text("FOLLOW THE MONKEY",Marker,new Vector3(0,1.6f,0),.14f,Art.White.color);
            Marker.gameObject.SetActive(false);
            MapLine=Art.Line("Roads becoming a tree",transform,Art.Cyan,.07f);MapLine.enabled=false;
            BuildCourse();
            var hud=Art.Root("Adventure instructions",rig.Head.transform);
            Instructions=hud;
            // Keep guidance in front of reachable handles while preserving its angular size.
            hud.localPosition=new Vector3(0,-.137143f,.4f);
            hud.localScale=Vector3.one*(.4f/1.4f);
            Art.Box("Instruction backing",hud,new Vector3(0,0,.02f),new Vector3(1.32f,.22f,.01f),Art.Dark);
            Status=Art.Text("",hud,new Vector3(0,.047f,0),.065f,Art.Cyan.color);
            Hint=Art.Text("",hud,new Vector3(0,-.045f,0),.06f,Art.White.color);
        }
        void BuildCourtyard()
        {
            var details=Art.Root("Courtyard detail and chase landmarks",transform);
            // Repeated architectural rhythm gives scale and route continuity without imported textures.
            for(int side=-1;side<=1;side+=2)
            {
                for(int z=2;z<46;z+=6)
                {
                    Art.Box("Window sill",details,new Vector3(side*10.25f,1.3f,z),new Vector3(.3f,.12f,1.7f),Art.White);
                    Art.Box("Stone course",details,new Vector3(side*10.28f,5.8f,z),new Vector3(.15f,.13f,5.8f),Art.White);
                    Art.Primitive("Planter rim",PrimitiveType.Cylinder,details,new Vector3(side*5.3f,.35f,z),new Vector3(1,.35f,1),Art.Sand);
                    Art.Primitive("Trimmed planting",PrimitiveType.Sphere,details,new Vector3(side*5.3f,.85f,z),new Vector3(1.2f,.8f,1.2f),Art.Mat(new Color(.10f,.25f,.18f)));
                }
            }
            for(int z=13;z<=37;z+=12)
            {
                for(int side=-1;side<=1;side+=2)
                    Art.Box("Arcade pier",details,new Vector3(side*3.5f,2.2f,z),new Vector3(.6f,4.4f,.7f),Art.Sand);
                for(int i=0;i<13;i++)
                {
                    float a=i*Mathf.PI/12;
                    var block=Art.Box("Arcade voussoir",details,new Vector3(Mathf.Cos(a)*3.5f,4.1f+Mathf.Sin(a)*1.5f,z),new Vector3(.92f,.5f,.7f),Art.Sand);
                    block.transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(1.5f*Mathf.Cos(a),-3.5f*Mathf.Sin(a))*Mathf.Rad2Deg);
                }
                Art.Text(z==13?"THE CLOISTERS":z==25?"RESEARCH WALK":"ROUTE ARCHIVE",details,new Vector3(0,4.2f,z-.4f),.19f,Art.White.color);
            }
            for(int i=0;i<38;i++)
            {
                float z=3+i;
                Art.Box("Wet paving inlay",details,new Vector3((i%2==0?-.8f:.8f),.025f,z),new Vector3(.32f,.015f,.65f),i%4==0?Art.Orange:Art.Muted);
            }
            Panel("AFTER HOURS", "STUDY SPACES OPEN\nCOFFEE / CLOISTERS / ARCHIVE",details,new Vector3(4,1.7f,11),new Vector2(2.2f,1.2f));
        }
        void BuildClassroom()
        {
            Classroom=Art.Root("Computing classroom",transform);
            Art.Box("Timber floor",Classroom,new Vector3(0,-.1f,-6),new Vector3(10,.2f,12),Art.Bronze);
            Art.Box("Ceiling",Classroom,new Vector3(0,3.5f,-6),new Vector3(10,.18f,12),Art.Dark);
            Art.Box("Rear wall",Classroom,new Vector3(0,1.7f,-12),new Vector3(10,3.4f,.2f),Art.Sand);
            for(int side=-1;side<=1;side+=2)
            {
                Art.Box("Exit wall",Classroom,new Vector3(side*3.15f,1.7f,-1),new Vector3(3.7f,3.4f,.2f),Art.Sand);
                Art.Box("Window wall",Classroom,new Vector3(side*5,1.7f,-6.5f),new Vector3(.2f,3.4f,11),Art.Sand);
                for(int z=-9;z<=-3;z+=3)
                {
                    Art.Box("Window glass",Classroom,new Vector3(side*4.88f,2.1f,z),new Vector3(.02f,1.6f,2),Art.Muted);
                    Art.Box("Window mullion",Classroom,new Vector3(side*4.84f,2.1f,z),new Vector3(.06f,1.6f,.06f),Art.White);
                    PlaceFurniture("desk",new Vector3(side*2.8f,0,z),new Vector3(2.3f,.80f,.85f));
                    Art.Box("Computer screen",Classroom,new Vector3(side*2.8f,1.12f,z+.15f),new Vector3(.8f,.45f,.05f),Art.Dark);
                    Art.Box("Screen glow",Classroom,new Vector3(side*2.8f,1.12f,z+.117f),new Vector3(.7f,.35f,.01f),Art.Cyan);
                    Art.Box("Keyboard",Classroom,new Vector3(side*2.8f,.82f,z-.18f),new Vector3(.55f,.03f,.19f),Art.Dark);
                    for(int line=0;line<4;line++)Art.Box("Code line",Classroom,new Vector3(side*2.8f-.07f,1.23f-line*.065f,z+.105f),new Vector3(.35f-line*.055f,.018f,.006f),Art.White);
                    Art.Box("Lecture notes",Classroom,new Vector3(side*2.8f+.8f,.82f,z-.12f),new Vector3(.36f,.012f,.29f),Art.White);
                    Art.Primitive("Coffee cup",PrimitiveType.Cylinder,Classroom,new Vector3(side*2.8f-.78f,.89f,z),new Vector3(.13f,.1f,.13f),Art.Orange);
                    PlaceFurniture("chairDesk",new Vector3(side*2.8f,0,z-.85f),new Vector3(.60f,1.05f,.60f));
                }
            }
            for(int z=-10;z<-1;z+=3)Art.Box("Ceiling strip",Classroom,new Vector3(0,3.38f,z),new Vector3(3,.03f,.08f),Art.White);
            for(int z=-11;z<-1;z++)Art.Box("Floor joint",Classroom,new Vector3(0,.005f,z),new Vector3(9.8f,.008f,.025f),Art.Dark);
            var posterMaterial=Art.Mat(Color.white,true);
            posterMaterial.mainTexture=Resources.Load<Texture2D>("Approved/FundingPoster");
            Art.Box("Noticeboard frame",Classroom,new Vector3(-3,1.82f,-1.16f),new Vector3(1.18f,1.75f,.06f),Art.Dark);
            Art.Primitive("Funding notice - approved AI illustration",PrimitiveType.Quad,Classroom,new Vector3(-3,1.82f,-1.20f),new Vector3(1.08f,1.62f,1),posterMaterial);
            Door=Art.Root("Automatic exit door",Classroom);Door.position=new Vector3(0,0,-1);
            Art.Box("Door leaf",Door,new Vector3(0,1.25f,0),new Vector3(2.4f,2.5f,.08f),Art.Muted);
            Art.Box("Door light",Door,new Vector3(0,2.55f,-.06f),new Vector3(2.4f,.04f,.03f),Art.Cyan);
            Panel("COMPUTING SCIENCE", "CS / ROUTES AND ANCESTORS\nOffice hours: 09:00 - 17:00",Classroom,new Vector3(3,2,-1.15f),new Vector2(2.7f,.9f));
        }
        void BuildCourse()
        {
            var tree=state.Tree;
            float[] x=TreeLayout.ComputeX(tree,1.3f);
            Positions=new Vector3[tree.Count];CampusNodes=new Vector3[tree.Count];
            float extent=1;
            for(int i=0;i<tree.Count;i++)extent=Mathf.Max(extent,Mathf.Abs(x[i]-x[state.StartNode]));
            for(int i=0;i<tree.Count;i++)
            {
                Positions[i]=new Vector3(x[i]-x[state.StartNode],tree.MaxDepth-tree.Depth[i],0);
                CampusNodes[i]=new Vector3(Positions[i].x/extent*2.5f,.08f,8+Positions[i].y*2);
            }
            for(int i=1;i<tree.Count;i++)
            {
                var road=Art.Line("Campus ancestor path "+i,transform,Art.Cyan,.07f);
                road.positionCount=2;road.SetPosition(0,CampusNodes[i]);road.SetPosition(1,CampusNodes[tree.Parent[i]]);Roads.Add(road);
            }
            Course=Art.Root("Original tree / manual ascent",transform);Course.position=CourseOrigin;
            SafetyDeck=Art.Primitive("Resting belay deck",PrimitiveType.Cylinder,Course,ClimbOffset-Vector3.up*.09f,new Vector3(1.05f,.06f,1.05f),Art.Bronze).transform;
            for(int i=0;i<Positions.Length;i++)
            {
                Vector3 p=Positions[i];
                var deck=Art.Primitive("Original node "+i,PrimitiveType.Cylinder,Course,p-Vector3.up*.05f,new Vector3(.9f,.05f,.9f),Art.Mat(i==state.Goal?new Color(.85f,.49f,.13f):new Color(.18f,.30f,.36f)));
                Platforms.Add(deck.GetComponent<Renderer>());
                Art.Primitive("Node rim",PrimitiveType.Cylinder,Course,p,new Vector3(.94f,.012f,.94f),i==state.Goal?Art.Orange:Art.Cyan);
                Art.Text("DEPTH "+tree.Depth[i],Course,p+new Vector3(.65f,.2f,0),.075f,Art.White.color);
                if(i>0)
                {
                    Beam(Course,p,Positions[tree.Parent[i]],.16f,bark);
                    Beam(Course,p+Vector3.forward*.10f,Positions[tree.Parent[i]]+Vector3.forward*.10f,.035f,Art.Cyan);
                }
            }
            // Original coordinates and edges are retained; the larger frame is purely scenic.
            for(int side=-1;side<=1;side+=2)
                Art.Box("Archive frame",Course,new Vector3(side*13,8,5),new Vector3(.5f,21,.5f),Art.Sand);
            Art.Box("Archive rear wall",Course,new Vector3(0,8,7),new Vector3(32,22,.4f),Art.Muted);
            for(int xColumn=-12;xColumn<=12;xColumn+=4)
            {
                Art.Box("Archive stone pier",Course,new Vector3(xColumn,8,6.7f),new Vector3(.35f,22,.4f),Art.Sand);
                for(int y=1;y<18;y+=4)
                {
                    Art.Box("Archive window recess",Course,new Vector3(xColumn+1.8f,y,6.75f),new Vector3(2.4f,2.9f,.3f),Art.Dark);
                    Art.Box("Archive window light",Course,new Vector3(xColumn+1.8f,y-1.3f,6.5f),new Vector3(2.4f,.045f,.06f),Art.Cyan);
                }
            }
            RecoveryBalcony=Art.Root("Recovery balcony assembly",Course);
            Vector3 balcony=new Vector3(0,-.12f,2.5f);
            RecoveryBalcony.localPosition=Positions[state.Goal];
            Art.Box("Recovery balcony",RecoveryBalcony,balcony,new Vector3(6,.2f,4),bark);
            for(int side=-1;side<=1;side+=2)
            {
                Beam(RecoveryBalcony,balcony+new Vector3(side*2.9f,1,-1.8f),balcony+new Vector3(side*2.9f,1,1.8f),.07f,silver);
                for(int z=-1;z<=1;z++)Beam(RecoveryBalcony,balcony+new Vector3(side*2.9f,0,z*1.7f),balcony+new Vector3(side*2.9f,1,z*1.7f),.045f,silver);
            }
            Panel("THE ROUTE TREE", "UNIVERSITY OF GLASGOW / ROUTE ARCHIVE",Course,new Vector3(0,18,5),new Vector2(9,1.4f));
            Controls=Art.Root("Ancestor choices",Course);
            var actions=new[]{AdventureAction.OneLevel,AdventureAction.TwoLevels,AdventureAction.FourLevels};
            for(int i=0;i<3;i++)Choices.Add(Button((1<<i)+" LEVEL"+(i==0?"":"S"),Controls,new Vector3((i-1)*.9f,1.15f,1.5f),actions[i],.82f));
            RouteTitle=Art.Text("",Controls,new Vector3(0,1.88f,1.65f),.095f,Art.White.color);
            MemoryLabel=Art.Text("",Controls,new Vector3(0,.72f,1.45f),.08f,Art.Cyan.color);
            Loom=Art.Root("Connection workshop",Course);
            Panel("REMEMBER THE BRANCHES", "",Loom,new Vector3(0,1.35f,1.65f),new Vector2(3.1f,1.85f));
            LoomTitle=Art.Text("",Loom,new Vector3(0,1.71f,1.52f),.105f,Art.Cyan.color);
            LoomNodes=Art.Text("",Loom,new Vector3(0,1.4f,1.50f),.09f,Art.White.color);
            LoomHint=Art.Text("",Loom,new Vector3(0,.68f,1.50f),.085f,Art.White.color);
            var plug=Art.Primitive("Memory connector",PrimitiveType.Sphere,Loom,new Vector3(-.85f,1.05f,1.40f),Vector3.one*.24f,Art.Orange);
            LinkStart=Target(plug,AdventureAction.LinkStart,Vector3.one*1.6f);
            var socket=Art.Primitive("Memory socket",PrimitiveType.Cube,Loom,new Vector3(.85f,1.05f,1.40f),Vector3.one*.24f,Art.Cyan);
            LinkEnd=Target(socket,AdventureAction.LinkEnd,Vector3.one*1.6f);
            Thread=Art.Line("Memory thread",transform,Art.Orange,.025f);Thread.positionCount=2;Thread.enabled=false;
            NewSearch=Art.Root("New funding target",Course);
            Panel("CONNECTIONS READY", "",NewSearch,new Vector3(0,1.8f,1.65f),new Vector2(3.5f,1.5f));
            SearchTitle=Art.Text("",NewSearch,new Vector3(0,1.65f,1.52f),.10f,Art.White.color);
            Button("FOLLOW THE FUNDING",NewSearch,new Vector3(0,.92f,1.5f),AdventureAction.NextTarget,2.65f);
            Loom.gameObject.SetActive(false);NewSearch.gameObject.SetActive(false);
            Grips=Art.Root("Branch climbing holds",Course);Grips.localPosition=ClimbOffset;
            Preview=Art.Line("Selected ancestor path",transform,Art.Orange,.055f);Preview.enabled=false;
            Cone=Art.BuildCone(Course,Positions[state.Goal]+ClimbOffset+new Vector3(0,1.15f,.85f),.40f);
            ConeTarget=Target(Cone.gameObject,AdventureAction.Cone,new Vector3(.65f,.75f,.65f));ConeTarget.GetComponent<BoxCollider>().center=Vector3.up*.15f;
            Art.Text("RECOVER CONE",Cone,new Vector3(0,.65f,0),.10f,Art.White.color);
            EndCard=Art.Root("Journey results",Course);
            Panel("ROUTE MEMORY RESTORED", "",EndCard,new Vector3(0,2,2),new Vector2(3.8f,1.3f));
            Outcome=Art.Text("",EndCard,new Vector3(0,1.9f,1.88f),.11f,Art.White.color);
            Button("NEW TARGET / KEEP MEMORY",EndCard,new Vector3(0,1.08f,1.6f),AdventureAction.NextTarget,2.9f);
            Button("REPLAY FROM CLASSROOM",EndCard,new Vector3(0,.62f,1.6f),AdventureAction.Replay,2.9f);
            EndCard.gameObject.SetActive(false);Grips.gameObject.SetActive(false);Course.gameObject.SetActive(false);
        }
        void PlaceFurniture(string resource,Vector3 feet,Vector3 size)
        {
            var prefab=Resources.Load<GameObject>("Approved/"+resource);
            if(prefab==null){Debug.LogError("Approved furniture missing: "+resource);return;}
            var item=Instantiate(prefab,Classroom);item.name="Kenney "+resource;item.transform.localPosition=Vector3.zero;
            var renderers=item.GetComponentsInChildren<Renderer>();
            if(renderers.Length==0)return;
            Bounds bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            item.transform.localScale=Vector3.Scale(item.transform.localScale,new Vector3(size.x/bounds.size.x,size.y/bounds.size.y,size.z/bounds.size.z));
            bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            item.transform.position+=Classroom.TransformPoint(feet)-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
            var finish=Art.Mat(resource=="desk"?new Color(.43f,.30f,.20f):new Color(.13f,.23f,.26f));
            foreach(var renderer in renderers){var materials=new Material[renderer.sharedMaterials.Length];for(int i=0;i<materials.Length;i++)materials[i]=finish;renderer.sharedMaterials=materials;}
        }
        public void RefreshMemory(AdventureState model)
        {
            foreach(var line in remembered)Destroy(line.gameObject);remembered.Clear();
            for(int n=0;n<model.Tree.Count;n++)foreach(int span in new[]{2,4})
            {
                if(!model.Knows(n,span))continue;
                var line=Art.Line("Remembered "+span+" from node "+n,transform,span==4?Art.Orange:Art.Cyan,.035f);
                line.positionCount=span+1;
                for(int j=0;j<=span;j++)line.SetPosition(j,CourseOrigin+Positions[model.Tree.KthAncestor(n,j)]+new Vector3(span==4?-.24f:.24f,.12f,-.15f));
                line.gameObject.SetActive(Course.gameObject.activeSelf);remembered.Add(line);
            }
        }
        public void BuildGrips(AdventureState model,float headHeight)
        {
            foreach(Transform child in Grips){child.gameObject.SetActive(false);Destroy(child.gameObject);}
            GripTargets.Clear();Grips.gameObject.SetActive(true);
            for(int n=model.Node;n!=model.Destination;n=model.Tree.Parent[n])
            {
                Vector3 a=Positions[n],b=Positions[model.Tree.Parent[n]];
                int steps=Mathf.Max(4,Mathf.CeilToInt(Vector3.Distance(a,b)/.22f));
                for(int j=0;j<=steps;j++)
                {
                    Vector3 p=Vector3.Lerp(a,b,(float)j/steps)+new Vector3(0,headHeight-.18f,.52f);
                    for(int side=-1;side<=1;side+=2)
                    {
                        var mount=Art.Root("Branch hold",Grips);mount.localPosition=p+Vector3.right*side*.23f;
                        var grip=Art.Primitive("Rounded grip",PrimitiveType.Capsule,mount,Vector3.zero,new Vector3(.055f,.12f,.055f),Art.Orange);
                        grip.transform.localRotation=Quaternion.Euler(0,0,90);
                        Art.Box("Grip bracket",mount,new Vector3(0,0,.06f),new Vector3(.07f,.05f,.12f),silver);
                        var target=Target(mount.gameObject,AdventureAction.Grip,new Vector3(.29f,.14f,.16f));
                        target.Surface=grip.GetComponent<Renderer>();target.BaseColor=Art.Orange.color;GripTargets.Add(target);
                    }
                }
                Beam(Grips,a+new Vector3(0,headHeight-.18f,.62f),b+new Vector3(0,headHeight-.18f,.62f),.065f,bark);
                Beam(Grips,a+new Vector3(0,0,.85f),a+new Vector3(0,headHeight-.18f,.62f),.04f,silver);
            }
        }
        public void PreviewRoute(AdventureState model,int span)
        {
            Preview.enabled=span<=model.Remaining && (model.Phase==AdventurePhase.Climbing || model.CanChoose(span));if(!Preview.enabled)return;
            int target=model.Tree.KthAncestor(model.Node,span);
            var points=new List<Vector3>();
            for(int n=model.Node;n>=0;n=model.Tree.Parent[n]){points.Add(CourseOrigin+Positions[n]+new Vector3(0,.08f,-.1f));if(n==target)break;}
            Preview.positionCount=points.Count;Preview.SetPositions(points.ToArray());
        }
        public AdventureTarget Button(string label,Transform parent,Vector3 position,AdventureAction action,float width)
        {
            var root=Art.Root(label,parent);root.localPosition=position;
            var face=Art.Box("Button face",root,Vector3.zero,new Vector3(width,.32f,.09f),Art.Dark);
            Art.Box("Button accent",root,new Vector3(0,-.16f,-.05f),new Vector3(width,.025f,.02f),Art.Orange);
            Art.Text(label,root,new Vector3(0,0,-.055f),.095f,Art.White.color);
            return Target(face,action,Vector3.one);
        }
        public AdventureTarget Target(GameObject go,AdventureAction action,Vector3 size)
        {
            var collider=go.AddComponent<BoxCollider>();collider.size=size;
            var target=go.AddComponent<AdventureTarget>();target.Action=action;target.Surface=go.GetComponent<Renderer>();
            if(target.Surface!=null)target.BaseColor=target.Surface.sharedMaterial.color;
            return target;
        }
        void Panel(string title,string body,Transform parent,Vector3 position,Vector2 size)
        {
            Art.Box(title,parent,position,new Vector3(size.x,size.y,.09f),Art.Dark);
            Art.Box("Panel orange rule",parent,position+new Vector3(-size.x*.49f,0,-.06f),new Vector3(.035f,size.y,.02f),Art.Orange);
            Art.Text(title,parent,position+new Vector3(0,size.y*.29f,-.07f),.15f,Art.White.color);
            Art.Text(body,parent,position+new Vector3(0,-size.y*.12f,-.07f),.10f,Art.White.color);
        }
        public void Beam(Transform parent,Vector3 a,Vector3 b,float radius,Material material)
        {
            var beam=Art.Primitive("Connection",PrimitiveType.Cylinder,parent,(a+b)*.5f,new Vector3(radius,Vector3.Distance(a,b)*.5f,radius),material);
            beam.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);
        }
    }
}
