using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BinaryLift
{
    /// <summary>Authored campus dressing. No colliders, progression or camera movement.</summary>
    public sealed class CampusDetail : MonoBehaviour
    {
        readonly List<Transform> banners=new List<Transform>();
        readonly List<LineRenderer> ripples=new List<LineRenderer>();
        Transform minuteHand,hourHand;
        CampusWorld art;

        public void Build(CampusWorld world)
        {
            art=world;
            var stone=art.Root("Detailed masonry",transform);
            for(int side=-1;side<=1;side+=2)
            {
                art.Box("Eaves stone lip",stone,new Vector3(side*10.2f,8.9f,24),new Vector3(.5f,.22f,54),art.Sand);
                art.Box("Rain gutter",stone,new Vector3(side*10.05f,9.02f,24),new Vector3(.14f,.13f,54),art.Bronze);
                for(int i=0;i<13;i++)
                {
                    float z=i*4+1.8f;
                    for(int floor=0;floor<2;floor++)
                    {
                        float y=2.6f+floor*3.6f;
                        var window=art.Root("Gothic window frame",stone);
                        window.localPosition=new Vector3(side*10.32f,y,z);
                        window.localRotation=Quaternion.Euler(0,side*90,0);
                        for(int edge=-1;edge<=1;edge+=2)
                            art.Box("Stone jamb",window,new Vector3(edge*.6f,-.13f,0),new Vector3(.14f,2.1f,.18f),art.Sand);
                        art.Box("Projecting sill",window,new Vector3(0,-1.2f,-.08f),new Vector3(1.5f,.16f,.38f),art.Sand);
                        art.Box("Central mullion",window,new Vector3(0,-.15f,-.04f),new Vector3(.055f,1.95f,.08f),art.White);
                        art.Box("Transom",window,new Vector3(0,.15f,-.04f),new Vector3(1.1f,.05f,.08f),art.White);
                        // Two connected slopes produce a pointed stone arch, not stacked blocks.
                        Beam(art,window,"Pointed arch",new Vector3(-.6f,.9f,0),new Vector3(0,1.5f,0),.14f,art.Sand);
                        Beam(art,window,"Pointed arch",new Vector3(0,1.5f,0),new Vector3(.6f,.9f,0),.14f,art.Sand);
                    }
                    if(i%3==0)
                        art.Primitive("Downpipe",PrimitiveType.Cylinder,stone,new Vector3(side*10.05f,4.45f,z-1),new Vector3(.09f,4.45f,.09f),art.Bronze);
                }
            }
            BuildClock(stone);
            BuildKiosk(stone);
            BuildEntrance(stone);
            Batch(stone);
            BuildWeather();
        }

        void BuildClock(Transform stone)
        {
            var clock=art.Root("Gilmorehill courtyard clock",transform);
            clock.localPosition=new Vector3(0,15.2f,49.91f);
            var rim=art.Primitive("Clock bronze rim",PrimitiveType.Cylinder,stone,clock.position,new Vector3(3.25f,.12f,3.25f),art.Bronze);
            rim.transform.localRotation=Quaternion.Euler(90,0,0);
            var face=art.Primitive("Clock ivory face",PrimitiveType.Cylinder,stone,clock.position+Vector3.back*.14f,new Vector3(2.96f,.025f,2.96f),art.White);
            face.transform.localRotation=Quaternion.Euler(90,0,0);
            for(int i=0;i<12;i++)
            {
                float a=i*Mathf.PI/6;
                var tick=art.Box("Clock hour marker",stone,clock.position+new Vector3(Mathf.Sin(a)*1.25f,Mathf.Cos(a)*1.25f,-.18f),new Vector3(.06f,.19f,.04f),art.Dark);
                tick.transform.localRotation=Quaternion.Euler(0,0,-i*30);
            }
            hourHand=art.Root("Hour hand",clock);minuteHand=art.Root("Minute hand",clock);
            art.Box("Hour hand blade",hourHand,new Vector3(0,.35f,-.22f),new Vector3(.10f,.8f,.04f),art.Dark);
            art.Box("Minute hand blade",minuteHand,new Vector3(0,.50f,-.24f),new Vector3(.065f,1.15f,.04f),art.Dark);
            art.Primitive("Clock spindle",PrimitiveType.Sphere,clock,new Vector3(0,0,-.27f),Vector3.one*.14f,art.Orange);
        }

        void BuildKiosk(Transform stone)
        {
            art.Box("Kiosk service opening",stone,new Vector3(7.5f,1.55f,7.72f),new Vector3(2.5f,1.15f,.06f),art.Bronze);
            art.Box("Kiosk counter",stone,new Vector3(7.5f,.98f,7.47f),new Vector3(3.0f,.12f,.6f),art.White);
            for(int side=-1;side<=1;side+=2)
                art.Box("Kiosk window jamb",stone,new Vector3(7.5f+side*1.3f,1.6f,7.6f),new Vector3(.1f,1.35f,.14f),art.Orange);
            art.Text("ESPRESSO YOURSELF",transform,new Vector3(7.5f,2.55f,7.66f),.12f,art.White.color);
            var menu=art.Root("Coffee pavement menu",transform);menu.localPosition=new Vector3(5.65f,.9f,7.2f);
            art.Box("Menu chalkboard",menu,Vector3.zero,new Vector3(.83f,1.13f,.10f),art.Dark);
            art.Text("LATE LAB\nCOFFEE\n\nONE SHOT\nTWO DEADLINES",menu,new Vector3(0,0,-.06f),.085f,art.White.color);
            for(int side=-1;side<=1;side+=2)
                art.Box("Menu timber leg",menu,new Vector3(side*.43f,-.3f,.05f),new Vector3(.07f,1.25f,.09f),art.Sand);
            CampusAssets.Place(art,"food","mug",transform,new Vector3(6.9f,1.05f,7.4f),new Vector3(.24f,.22f,.22f));
            CampusAssets.Place(art,"furniture","pottedPlant",transform,new Vector3(8.4f,1.05f,7.4f),new Vector3(.35f,.45f,.35f));
            for(int side=-1;side<=1;side+=2)
            {
                var banner=art.Root("Wind banner",transform);banner.localPosition=new Vector3(side*8.5f,3.6f,13);
                art.Box("Banner fabric",banner,new Vector3(0,-.6f,0),new Vector3(.65f,1.2f,.025f),art.Orange);
                art.Text("LATE\nLAB",banner,new Vector3(0,-.5f,-.025f),.15f,art.White.color);
                art.Box("Banner pole",stone,new Vector3(side*8.9f,1.8f,13),new Vector3(.07f,3.6f,.07f),art.Bronze);
                art.Box("Banner crossbar",stone,new Vector3(side*8.5f,3.61f,13),new Vector3(.85f,.05f,.05f),art.Bronze);
                banners.Add(banner);
            }
        }

        void BuildEntrance(Transform stone)
        {
            var wood=CampusAssets.Surface(art,"wood_planks",new Vector2(2,1));
            art.Box("Notice board timber surround",stone,new Vector3(-3.6f,1.65f,-.76f),new Vector3(1.8f,1.15f,.11f),wood);
            art.Box("Notice board felt",stone,new Vector3(-3.6f,1.65f,-.84f),new Vector3(1.62f,.97f,.025f),art.Bronze);
            art.Text("CAMPUS NOTICES\n\nLOST: ONE DEADLINE\nFOUND: THREE MORE\n\nTREE CLUB / BRANCH OUT",transform,new Vector3(-3.6f,1.65f,-.87f),.075f,art.White.color);
            for(int side=-1;side<=1;side+=2)
                art.Primitive("Notice pin",PrimitiveType.Sphere,stone,new Vector3(-3.6f+side*.7f,2.02f,-.88f),Vector3.one*.06f,art.Orange);
            art.Box("Entrance door lintel",stone,new Vector3(0,2.8f,-.85f),new Vector3(2.4f,.18f,.34f),art.Sand);
            art.Box("Entrance mat",stone,new Vector3(0,.025f,-.3f),new Vector3(2,.025f,.9f),art.Muted);
            for(int i=-5;i<=5;i++)
                art.Box("Mat rib",stone,new Vector3(i*.16f,.044f,-.3f),new Vector3(.035f,.008f,.8f),art.Dark);
            var umbrella=art.Root("Umbrella stand",transform);umbrella.localPosition=new Vector3(3.8f,0,-1.8f);
            Frustum(art,umbrella,"Stand basket",Vector3.up*.06f,.5f,.21f,.25f,art.Bronze);
            for(int i=0;i<3;i++)
            {
                var folded=art.Root("Folded umbrella",umbrella);folded.localPosition=new Vector3((i-1)*.12f,.1f,0);folded.localRotation=Quaternion.Euler(0,0,(i-1)*9);
                Frustum(art,folded,"Folded canopy",Vector3.up*.2f,.55f,.07f,.015f,i==1?art.Orange:art.Dark);
                art.Primitive("Umbrella handle",PrimitiveType.Capsule,folded,Vector3.up*.9f,new Vector3(.035f,.15f,.035f),art.Sand);
            }
        }

        void BuildWeather()
        {
            var wet=art.Mat(new Color(.16f,.25f,.28f));wet.SetFloat("_Glossiness",.92f);
            for(int i=0;i<6;i++)
            {
                var puddle=art.Root("Rain puddle",transform);puddle.localPosition=new Vector3((i%2==0?-1:1)*2.7f,.019f,6+i*6);
                art.Primitive("Shallow water",PrimitiveType.Cylinder,puddle,Vector3.zero,new Vector3(1.25f,.003f,.65f),wet);
                var ring=art.Line("Rain ripple",puddle,art.Muted,.008f);ring.useWorldSpace=false;ring.loop=true;ring.positionCount=24;
                for(int p=0;p<24;p++){float a=p*Mathf.PI/12;ring.SetPosition(p,new Vector3(Mathf.Cos(a)*.5f,.008f,Mathf.Sin(a)*.24f));}
                ripples.Add(ring);
            }
        }

        void Update()
        {
            // Fictional evening time; no system clock, flashing light or camera motion.
            float minutes=15+Time.time/60;
            if(minuteHand!=null)minuteHand.localRotation=Quaternion.Euler(0,0,-minutes*6);
            if(hourHand!=null)hourHand.localRotation=Quaternion.Euler(0,0,-(7+minutes/60)*30);
            for(int i=0;i<banners.Count;i++)banners[i].localRotation=Quaternion.Euler(0,0,Mathf.Sin(Time.time*.7f+i)*2.5f);
            for(int i=0;i<ripples.Count;i++){float t=Mathf.Repeat(Time.time*.35f+i*.17f,1);ripples[i].transform.localScale=new Vector3(t,1,t);ripples[i].enabled=t>.05f&&t<.9f;}
        }

        void Batch(Transform root)
        {
            var groups=new Dictionary<Material,List<CombineInstance>>();
            foreach(var filter in root.GetComponentsInChildren<MeshFilter>())
            {
                var renderer=filter.GetComponent<MeshRenderer>();
                if(renderer==null||filter.sharedMesh==null)continue;
                if(!groups.TryGetValue(renderer.sharedMaterial,out var list))groups[renderer.sharedMaterial]=list=new List<CombineInstance>();
                list.Add(new CombineInstance{mesh=filter.sharedMesh,transform=root.worldToLocalMatrix*filter.transform.localToWorldMatrix});renderer.enabled=false;
            }
            foreach(var pair in groups)
            {
                var mesh=new Mesh{name="Campus detail batch",indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(pair.Value.ToArray());
                art.MeshObject("Batched masonry and trim",root,mesh,pair.Key);
            }
        }

        public static void Beam(CampusWorld art,Transform parent,string name,Vector3 a,Vector3 b,float width,Material material)
        {
            var beam=art.Box(name,parent,(a+b)*.5f,new Vector3(width,(b-a).magnitude,width),material);
            beam.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);
        }

        public static GameObject Frustum(CampusWorld art,Transform parent,string name,Vector3 position,float height,float bottom,float top,Material material)
        {
            const int sides=24;
            var vertices=new Vector3[(sides+1)*2];var normals=new Vector3[vertices.Length];var uv=new Vector2[vertices.Length];var triangles=new int[sides*6];
            for(int i=0;i<=sides;i++)
            {
                float a=i*Mathf.PI*2/sides;var radial=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
                vertices[i*2]=radial*bottom;vertices[i*2+1]=radial*top+Vector3.up*height;
                normals[i*2]=normals[i*2+1]=(radial+Vector3.up*((bottom-top)/height)).normalized;
                uv[i*2]=new Vector2((float)i/sides,0);uv[i*2+1]=new Vector2((float)i/sides,1);
                if(i==sides)continue;int v=i*2,t=i*6;
                triangles[t]=v;triangles[t+1]=v+1;triangles[t+2]=v+2;
                triangles[t+3]=v+2;triangles[t+4]=v+1;triangles[t+5]=v+3;
            }
            var mesh=new Mesh{name=name,vertices=vertices,normals=normals,uv=uv,triangles=triangles};mesh.RecalculateBounds();
            var go=art.MeshObject(name,parent,mesh,material);go.transform.localPosition=position;return go;
        }
    }
}
