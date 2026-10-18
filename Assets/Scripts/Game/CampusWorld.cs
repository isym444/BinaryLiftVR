using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BinaryLift
{
    /// <summary>Approved primitive blockout. All dimensions are metres; no imported art.</summary>
    public sealed class CampusWorld : MonoBehaviour
    {
        public Transform Station, Horse, Monkey, Cone, Saddle, MapRoot;
        public TextMesh Objective, Status, Hint, Chapter, CompanionLine;
        public QuestTarget BeginButton, ConeTarget, SaddleTarget;
        public readonly List<QuestTarget> Targets = new List<QuestTarget>();
        public readonly List<QuestTarget> JumpButtons = new List<QuestTarget>();
        public readonly List<TextMesh> JumpLabels = new List<TextMesh>();
        public readonly List<Renderer> MapNodes = new List<Renderer>();
        public readonly List<Transform> TrophyCones = new List<Transform>();
        public LineRenderer RouteLine;
        public Material Orange, Cyan, Dark, Sand, Bronze, White, Muted;
        readonly List<Material> ownedMaterials = new List<Material>();
        readonly List<Mesh> ownedMeshes = new List<Mesh>();
        Font font;
        Material worldTextMaterial;
        Transform architecture;

        public void Build(CampusQuestState state, bool climbing = false)
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var textShader=Resources.Load<Shader>("WorldText");
            if(textShader!=null)
            {
                worldTextMaterial=new Material(textShader);
                worldTextMaterial.mainTexture=font.material.mainTexture;
                ownedMaterials.Add(worldTextMaterial);
            }
            Orange = Mat(new Color(1f, .33f, .07f));
            Cyan = Mat(new Color(.18f, .72f, .78f), true);
            Dark = Mat(new Color(.035f, .065f, .09f));
            Sand = Mat(new Color(.52f, .43f, .33f));
            Bronze = Mat(new Color(.18f, .26f, .27f));
            White = Mat(new Color(.92f, .88f, .76f));
            Muted = Mat(new Color(.16f, .23f, .27f));
            var masonry=Resources.Load<Texture2D>("Approved/Sandstone");
            var masonryShader=Resources.Load<Shader>("Sandstone");
            if(masonry!=null && masonryShader!=null){Sand.shader=masonryShader;Sand.mainTexture=masonry;Sand.color=Color.white;}
            BuildCampus();
            Station = Root("Player station", transform);
            if (!climbing) BuildTerminal();
            Horse = BuildHorse(Station, new Vector3(1.95f, 0, 3.0f));
            CompanionLine = Text("CONE COURIER\nYour campus companion", Station,
                new Vector3(1.95f, 2.35f, 3.0f), .095f, White.color);
            Saddle = Root("Cone dock", Horse);
            Saddle.localPosition = new Vector3(-.12f, 1.15f, -.27f);
            var dock = Box("Saddle dock", Saddle, Vector3.zero, new Vector3(.55f, .16f, .58f), Orange);
            SaddleTarget = Target(dock, QuestAction.Saddle, 0, "STORE CONE  /  unlock route memory");
            for (int i = 0; i < 4; i++)
            {
                Transform trophy = BuildCone(Horse, new Vector3(-.35f + i * .22f, 1.3f, -.27f), .27f);
                trophy.gameObject.SetActive(false);
                TrophyCones.Add(trophy);
            }
            if (climbing)
            {
                Monkey = BuildMonkey(transform, new Vector3(0, 9, 5));
                CombineArchitecture();
                return;
            }
            Box("Clue plinth", Station, new Vector3(-1.9f, .38f, 1.25f), new Vector3(.65f,.76f,.65f), Sand);
            Box("Plinth light", Station, new Vector3(-1.9f,.78f,1.25f), new Vector3(.7f,.04f,.7f), Cyan);
            Cone = BuildCone(Station, new Vector3(-1.9f, .81f, 1.25f), .55f);
            var coneHit = Cone.gameObject.AddComponent<BoxCollider>();
            coneHit.center = new Vector3(0,.27f,0); coneHit.size = new Vector3(.5f,.65f,.5f);
            ConeTarget = Cone.gameObject.AddComponent<QuestTarget>();
            ConeTarget.Action = QuestAction.Cone; ConeTarget.Caption = "RECOVER CONE  /  grip nearby or point + trigger";
            Targets.Add(ConeTarget);
            Text("CLUE COLLECTION", Station, new Vector3(-1.9f,.52f,.90f), .05f, White.color);
            BuildMap(state);
            Monkey = BuildMonkey(transform, new Vector3(-2.4f,.9f,6f));
            RouteLine = Line("Route preview", transform, Cyan, .035f);
            RouteLine.enabled = false;
            CombineArchitecture();
        }

        public Vector3 NodePosition(int node)
        {
            if (node <= 20) return new Vector3(Mathf.Sin((20-node)*.43f)*1.1f,0,(20-node)*2.2f);
            int parent = (node-21)*3+2;
            return NodePosition(parent) + new Vector3((node%2==0?1:-1)*5,0,0);
        }

        void BuildCampus()
        {
            architecture = Root("Campus architecture - replaceable blockout", transform);
            Material paving = Mat(new Color(.115f,.16f,.19f));
            paving.SetFloat("_Glossiness", .72f);
            Box("Courtyard", architecture, new Vector3(0,-.18f,22), new Vector3(36,.3f,66), paving);
            Material lawn = Mat(new Color(.09f,.21f,.17f));
            Box("West lawn", architecture, new Vector3(-7,-.005f,22), new Vector3(6,.05f,48), lawn);
            Box("East lawn", architecture, new Vector3(7,-.005f,22), new Vector3(6,.05f,48), lawn);
            for (int i=0;i<23;i++)
                Box("Paving seam", architecture, new Vector3(0,.002f,i*2.2f-1), new Vector3(7,.012f,.035f), Muted);
            for (int side=-1;side<=1;side+=2)
            {
                Box("Sandstone range", architecture, new Vector3(side*13,4.5f,24), new Vector3(5,9,54), Sand);
                Box("Roof", architecture, new Vector3(side*13,9.15f,24), new Vector3(5.6f,.5f,55), Dark);
                for (int i=0;i<13;i++)
                {
                    float z=i*4f;
                    Box("Buttress", architecture, new Vector3(side*10.35f,3.5f,z), new Vector3(.7f,7, .7f), Sand);
                    for (int floor=0;floor<2;floor++)
                    {
                        Box("Recessed window", architecture,new Vector3(side*10.45f,2.6f+floor*3.6f,z+1.8f),new Vector3(.06f,2.3f,1.05f),Dark);
                        Box("Window glow",architecture,new Vector3(side*10.40f,2.6f+floor*3.6f,z+1.8f),new Vector3(.07f,.035f,.85f),Cyan);
                    }
                }
                for (int i=0;i<5;i++)
                {
                    float z=4+i*9;
                    Box("Bench seat",architecture,new Vector3(side*4.6f,.48f,z),new Vector3(.75f,.13f,1.8f),Bronze);
                    Box("Bench support",architecture,new Vector3(side*4.6f,.23f,z),new Vector3(.45f,.46f,.8f),Dark);
                    Box("Path light",architecture,new Vector3(side*3.7f,.38f,z+2),new Vector3(.14f,.76f,.14f),Dark);
                    Box("Path light lens",architecture,new Vector3(side*3.7f,.78f,z+2),new Vector3(.16f,.08f,.16f),Cyan);
                }
            }
            // A readable Gothic tower silhouette, inspired by Gilmorehill rather than a surveyed replica.
            Box("Main building",architecture,new Vector3(0,5,52),new Vector3(27,10,5),Sand);
            Box("Tower",architecture,new Vector3(0,12,53),new Vector3(6,24,6),Sand);
            Box("Tower belfry",architecture,new Vector3(0,20,49.95f),new Vector3(2.6f,4,.08f),Dark);
            Box("Tower crown",architecture,new Vector3(0,24.1f,53),new Vector3(7,.65f,7),Sand);
            for(int x=-1;x<=1;x+=2) for(int z=-1;z<=1;z+=2)
            {
                Box("Tower pinnacle",architecture,new Vector3(x*2.8f,25.5f,53+z*2.8f),new Vector3(.65f,3.4f,.65f),Sand);
                Primitive("Pinnacle cap",PrimitiveType.Sphere,architecture,new Vector3(x*2.8f,27.2f,53+z*2.8f),Vector3.one*.7f,Sand);
            }
            for(int i=-2;i<=2;i++)
            {
                float x=i*4.5f;
                Box("Cloister shadow",architecture,new Vector3(x,1.7f,49.4f),new Vector3(2.8f,3.4f,.12f),Dark);
                for(int j=0;j<7;j++)
                {
                    float a=j*Mathf.PI/6;
                    var block=Box("Cloister arch",architecture,new Vector3(x+Mathf.Cos(a)*1.5f,3+Mathf.Sin(a)*1.5f,49.2f),new Vector3(.65f,.55f,.5f),Sand);
                    block.transform.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg);
                }
            }
            Box("Campus welcome sign",architecture,new Vector3(-5.4f,1.5f,4.5f),new Vector3(2.6f,1.2f,.18f),Dark);
            Text("UNIVERSITY OF\nGLASGOW",transform,new Vector3(-5.4f,1.65f,4.38f),.18f,White.color);
            Text("GILMOREHILL  /  AFTER HOURS",transform,new Vector3(-5.4f,1.18f,4.38f),.055f,Cyan.color);
            Box("Coffee kiosk",architecture,new Vector3(7.5f,1.4f,9),new Vector3(3,2.8f,2.5f),Dark);
            Text("LATE LAB\nCOFFEE",transform,new Vector3(7.5f,2,7.7f),.18f,White.color);
            Box("Kiosk orange canopy",architecture,new Vector3(7.5f,2.9f,8.6f),new Vector3(3.3f,.16f,3.4f),Orange);
            for(int i=0;i<=20;i++)
            {
                Vector3 p=NodePosition(i);
                Box("Route tile "+i,architecture,p+Vector3.up*.015f,new Vector3(1,.025f,.8f),Muted);
                Box("Route marker "+i,architecture,p+new Vector3(.52f,.035f,0),new Vector3(.06f,.025f,.8f),Cyan);
            }
        }

        void BuildTerminal()
        {
            var terminal=Root("Route terminal",Station);
            terminal.localPosition=new Vector3(-.25f,1.45f,2.65f);
            Box("Terminal housing",terminal,Vector3.zero,new Vector3(2.3f,1.65f,.13f),Dark);
            Box("Top accent",terminal,new Vector3(0,.83f,-.08f),new Vector3(2.3f,.035f,.03f),Orange);
            Box("Terminal stand",terminal,new Vector3(0,-1.05f,.12f),new Vector3(.15f,.8f,.15f),Bronze);
            Chapter=Text("CONE COURIER / 01",terminal,new Vector3(0,.64f,-.08f),.13f,White.color);
            Objective=Text("",terminal,new Vector3(0,.34f,-.08f),.095f,White.color);
            Status=Text("",terminal,new Vector3(0,.04f,-.08f),.075f,Cyan.color);
            Hint=Text("",terminal,new Vector3(0,-.68f,-.09f),.065f,White.color);
            BeginButton=Button("BEGIN THE TRAIL",terminal,new Vector3(0,-.26f,-.12f),new Vector3(1.55f,.24f,.08f),QuestAction.Begin,0);
            for(int j=0;j<4;j++)
            {
                int levels=1<<j;
                var button=Button(levels+" STEP"+(levels>1?"S":""),terminal,new Vector3(-.81f+j*.54f,-.26f,-.12f),new Vector3(.49f,.25f,.08f),QuestAction.Jump,levels);
                JumpButtons.Add(button); JumpLabels.Add(button.GetComponentInChildren<TextMesh>());
            }
            Button("SCAN TREE",terminal,new Vector3(-.54f,-.49f,-.12f),new Vector3(.9f,.17f,.08f),QuestAction.Scan,0);
            Button("RESTART",terminal,new Vector3(.54f,-.49f,-.12f),new Vector3(.9f,.17f,.08f),QuestAction.Restart,0);
        }

        void BuildMap(CampusQuestState state)
        {
            MapRoot=Root("World-space route map",Station);
            MapRoot.localPosition=new Vector3(-3.1f,1.4f,3.1f);
            Box("Map board",MapRoot,Vector3.zero,new Vector3(1.5f,2.1f,.1f),Dark);
            Text("CAMPUS / ROUTE TREE",MapRoot,new Vector3(0,.93f,-.07f),.07f,White.color);
            Text("ORANGE: YOU   CYAN: CLUE",MapRoot,new Vector3(0,-.94f,-.07f),.044f,Cyan.color);
            for(int i=0;i<state.Tree.Count;i++)
            {
                Vector3 p=MapPoint(state,i);
                var node=Primitive("Map node "+i,PrimitiveType.Sphere,MapRoot,p,Vector3.one*.043f,Mat(Muted.color,true));
                MapNodes.Add(node.GetComponent<Renderer>());
                if(i==0) continue;
                var line=Line("Parent edge",MapRoot,Muted,.012f);
                line.useWorldSpace=false; line.positionCount=2;
                line.SetPosition(0,p);line.SetPosition(1,MapPoint(state,state.Tree.Parent[i]));
            }
        }

        Vector3 MapPoint(CampusQuestState state,int node)
        {
            float x=node>20?(node%2==0?.35f:-.35f):0;
            return new Vector3(x,.78f-state.Tree.Depth[node]*.075f,-.095f);
        }

        public Transform BuildCone(Transform parent,Vector3 position,float height)
        {
            Transform root=Root("Cone placeholder",parent); root.localPosition=position;
            Box("Cone base",root,new Vector3(0,height*.035f,0),new Vector3(height*.85f,height*.07f,height*.85f),Dark);
            for(int i=0;i<7;i++)
            {
                float diameter=height*(.7f-i*.085f);
                Primitive("Cone band",PrimitiveType.Cylinder,root,new Vector3(0,height*(.13f+i*.12f),0),new Vector3(diameter,height*.065f,diameter),(i==2||i==3)?White:Orange);
            }
            return root;
        }

        Transform BuildHorse(Transform parent,Vector3 pos)
        {
            Transform horse=Root("Horse companion placeholder",parent);horse.localPosition=pos;
            Primitive("Body",PrimitiveType.Capsule,horse,new Vector3(0,1,0),new Vector3(.62f,.72f,.62f),Bronze).transform.localRotation=Quaternion.Euler(90,0,0);
            for(int x=-1;x<=1;x+=2) for(int z=-1;z<=1;z+=2)
            {
                Box("Leg",horse,new Vector3(x*.22f,.43f,z*.4f),new Vector3(.14f,.86f,.17f),Bronze);
                Box("Hoof",horse,new Vector3(x*.22f,.08f,z*.4f-.04f),new Vector3(.2f,.16f,.26f),Dark);
            }
            Primitive("Neck",PrimitiveType.Capsule,horse,new Vector3(0,1.4f,.48f),new Vector3(.35f,.45f,.38f),Bronze).transform.localRotation=Quaternion.Euler(-23,0,0);
            Box("Head",horse,new Vector3(0,1.7f,.72f),new Vector3(.35f,.32f,.62f),Bronze);
            Box("Muzzle",horse,new Vector3(0,1.56f,1),new Vector3(.36f,.25f,.25f),Muted);
            for(int side=-1;side<=1;side+=2)
            {
                Box("Ear",horse,new Vector3(side*.12f,1.96f,.55f),new Vector3(.09f,.25f,.12f),Bronze);
                Primitive("Eye",PrimitiveType.Sphere,horse,new Vector3(side*.18f,1.76f,.76f),Vector3.one*.065f,Cyan);
            }
            Box("Tail",horse,new Vector3(0,.85f,-.73f),new Vector3(.12f,.65f,.16f),Dark);
            BuildCone(horse,new Vector3(0,1.88f,.64f),.28f);
            horse.localRotation=Quaternion.Euler(0,-115,0);
            return horse;
        }

        Transform BuildMonkey(Transform parent,Vector3 pos)
        {
            Transform monkey=Root("Monkey placeholder",parent);monkey.localPosition=pos;
            Material plum=Mat(new Color(.36f,.22f,.31f));
            Primitive("Body",PrimitiveType.Capsule,monkey,new Vector3(0,.4f,0),new Vector3(.4f,.42f,.35f),plum);
            Primitive("Head",PrimitiveType.Sphere,monkey,new Vector3(0,.92f,0),Vector3.one*.45f,plum);
            Primitive("Face",PrimitiveType.Sphere,monkey,new Vector3(0,.89f,-.17f),new Vector3(.32f,.26f,.14f),Sand);
            for(int side=-1;side<=1;side+=2)
            {
                Primitive("Ear",PrimitiveType.Sphere,monkey,new Vector3(side*.25f,.94f,0),Vector3.one*.19f,plum);
                Box("Arm",monkey,new Vector3(side*.27f,.47f,-.03f),new Vector3(.12f,.6f,.13f),plum);
                Box("Foot",monkey,new Vector3(side*.13f,.05f,-.09f),new Vector3(.15f,.15f,.3f),plum);
                Primitive("Eye",PrimitiveType.Sphere,monkey,new Vector3(side*.08f,.95f,-.23f),Vector3.one*.04f,Dark);
            }
            Box("Stolen route notebook",monkey,new Vector3(0,.47f,-.3f),new Vector3(.38f,.28f,.07f),Orange);
            var tail=Line("Curled tail",monkey,plum,.07f);tail.useWorldSpace=false;tail.positionCount=12;
            for(int i=0;i<12;i++)tail.SetPosition(i,new Vector3(Mathf.Sin(i*.35f)*.24f,.2f+i*.035f,.15f+i*.045f));
            return monkey;
        }

        public QuestTarget Button(string label,Transform parent,Vector3 pos,Vector3 scale,QuestAction action,int value)
        {
            var go=Box(label,parent,pos,scale,Mat(Orange.color));
            QuestTarget target=Target(go,action,value,label);
            // Text must not inherit the nonuniform button scale.
            var text=Text(label,parent,pos+new Vector3(0,0,-scale.z*.6f),action==QuestAction.Jump?.085f:.08f,White.color);
            text.transform.SetParent(go.transform,true);
            return target;
        }

        QuestTarget Target(GameObject go,QuestAction action,int value,string caption)
        {
            var collider=go.AddComponent<BoxCollider>();collider.size=Vector3.one;
            var target=go.AddComponent<QuestTarget>();target.Action=action;target.Value=value;
            target.Caption=caption;target.Surface=go.GetComponent<Renderer>();target.BaseColor=target.Surface.sharedMaterial.color;
            Targets.Add(target);return target;
        }

        public Transform Root(string name,Transform parent)
        {
            var t=new GameObject(name).transform;t.SetParent(parent,false);return t;
        }

        public Material Mat(Color color,bool unlit=false)
        {
            var mat=new Material(Shader.Find(unlit?"Sprites/Default":"Standard")){color=color,enableInstancing=true};
            ownedMaterials.Add(mat);return mat;
        }

        public GameObject Box(string name,Transform parent,Vector3 pos,Vector3 scale,Material mat)
            => Primitive(name,PrimitiveType.Cube,parent,pos,scale,mat);

        public GameObject Primitive(string name,PrimitiveType type,Transform parent,Vector3 pos,Vector3 scale,Material mat)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);
            go.transform.localPosition=pos;go.transform.localScale=scale;
            var collider=go.GetComponent<Collider>();collider.enabled=false;Destroy(collider);
            var renderer=go.GetComponent<Renderer>();renderer.sharedMaterial=mat;
            renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            return go;
        }

        public TextMesh Text(string text,Transform parent,Vector3 pos,float size,Color color)
        {
            var t=Root("Text - "+(text.Length>30?text.Substring(0,30):text),parent);t.localPosition=pos;
            var tm=t.gameObject.AddComponent<TextMesh>();tm.font=font;tm.fontSize=64;
            tm.characterSize=size*.15f;tm.anchor=TextAnchor.MiddleCenter;tm.alignment=TextAlignment.Center;
            tm.color=color;tm.text=text;t.GetComponent<MeshRenderer>().sharedMaterial=worldTextMaterial!=null?worldTextMaterial:font.material;return tm;
        }

        public LineRenderer Line(string name,Transform parent,Material mat,float width)
        {
            var line=Root(name,parent).gameObject.AddComponent<LineRenderer>();line.sharedMaterial=mat;
            line.startWidth=line.endWidth=width;line.shadowCastingMode=ShadowCastingMode.Off;return line;
        }

        void CombineArchitecture()
        {
            // Batch only static scenery; targets, text, and animated characters remain independent.
            var groups=new Dictionary<Material,List<CombineInstance>>();
            foreach(var filter in architecture.GetComponentsInChildren<MeshFilter>())
            {
                var renderer=filter.GetComponent<MeshRenderer>();
                if(!groups.TryGetValue(renderer.sharedMaterial,out var list))
                    groups[renderer.sharedMaterial]=list=new List<CombineInstance>();
                list.Add(new CombineInstance{mesh=filter.sharedMesh,transform=filter.transform.localToWorldMatrix});
                renderer.enabled=false;
            }
            foreach(var pair in groups)
            {
                var go=Root("Batched campus geometry",architecture).gameObject;
                var mesh=new Mesh{indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(pair.Value.ToArray());ownedMeshes.Add(mesh);
                go.AddComponent<MeshFilter>().sharedMesh=mesh;
                var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=pair.Key;
                renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            }
        }

        void LateUpdate()
        {
            if(worldTextMaterial!=null && font!=null && worldTextMaterial.mainTexture!=font.material.mainTexture)
                worldTextMaterial.mainTexture=font.material.mainTexture;
        }

        void OnDestroy()
        {
            foreach(var material in ownedMaterials) if(material!=null) Destroy(material);
            foreach(var mesh in ownedMeshes) if(mesh!=null) Destroy(mesh);
        }
    }
}
