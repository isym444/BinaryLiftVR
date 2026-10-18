using System;
using System.Collections.Generic;
using UnityEngine;
namespace BinaryLift
{
    public enum CampusPropKind { Cup, Stamp, Form, Drawer, Brew, Clue }
    public sealed class CampusProp : MonoBehaviour { public CampusPropKind Kind; }
    /// <summary>Optional, spatial campus tasks. Props return safely after invalid drops.</summary>
    public sealed class CampusLife : MonoBehaviour
    {
        public CampusLifeState State { get; private set; }=new CampusLifeState();
        public Transform Cup,Stamp,Form,Drawer,Clue;
        public TextMesh CoffeeHint,PaperHint;
        public bool Holding => grabbed!=null;
        readonly List<CampusProp> props=new List<CampusProp>();
        readonly bool[] previousGrip=new bool[2];
        readonly Vector3 cupHome=new Vector3(-2.85f,1.08f,-5.35f),brewSpot=new Vector3(-1.95f,1.08f,-5.35f),coaster=new Vector3(-1.05f,1.08f,-5.35f);
        readonly Vector3 stampHome=new Vector3(.8f,1.08f,-5.35f),formHome=new Vector3(1.55f,1.09f,-5.35f),inbox=new Vector3(2.7f,1.10f,-5.35f),drawerHome=new Vector3(1.6f,.65f,-5.3f);
        CampusWorld art;XRRig rig;Action<string> record;
        Transform liquid,seal,stream;readonly List<Transform> steam=new List<Transform>();
        CampusProp grabbed;Vector3 grabStart,handStart;Plane dragPlane;int hand=-1;float mouseY;
        bool active,wasBrewing;float stampUntil;Vector3 stampRest;string routeHint;
        public void Build(AdventureWorld world,XRRig player,AdventureState quest,Action<string> log)
        {
            art=world.Art;rig=player;record=log;
            routeHint="ROUTE CARD: "+quest.StartNode+" > "+quest.Tree.KthAncestor(quest.StartNode,1)+" > "+quest.Tree.KthAncestor(quest.StartNode,2)+" / TWO PARENT EDGES";
            var wood=CampusAssets.Surface(art,"wood_planks",new Vector2(2,1));
            for(int side=-1;side<=1;side+=2)
            {
                art.Box("Campus worktop",transform,new Vector3(side*1.95f,1,-5.05f),new Vector3(2.7f,.14f,1.05f),wood);
                for(int leg=-1;leg<=1;leg+=2)art.Box("Worktop leg",transform,new Vector3(side*1.95f+leg*1.1f,.48f,-4.95f),new Vector3(.12f,.96f,.55f),art.Dark);
            }
            art.Text("THE DAILY GRIND",transform,new Vector3(-1.95f,2.25f,-5.05f),.16f,art.White.color);
            art.Text("EXPENSES / NO HAY CLAIMS",transform,new Vector3(1.8f,2.25f,-5.05f),.14f,art.White.color);
            CoffeeHint=art.Text("DRAG CUP UNDER SPOUT, THEN PRESS BREW",transform,new Vector3(-1.95f,1.95f,-5.4f),.065f,art.Cyan.color);
            PaperHint=art.Text("DRAG STAMP ONTO FORM",transform,new Vector3(1.8f,1.95f,-5.4f),.075f,art.Cyan.color);
            art.Box("Coffee machine",transform,new Vector3(-1.95f,1.4f,-4.85f),new Vector3(.62f,.7f,.42f),art.Dark);
            art.Box("Copper machine trim",transform,new Vector3(-1.95f,1.7f,-5.08f),new Vector3(.55f,.10f,.05f),art.Orange);
            art.Box("Coffee spout",transform,new Vector3(-1.95f,1.52f,-5.25f),new Vector3(.10f,.07f,.3f),art.Bronze);
            art.Box("Cup docking tray",transform,brewSpot-Vector3.up*.015f,new Vector3(.42f,.02f,.34f),art.Bronze);
            for(int i=-2;i<=2;i++)art.Box("Drip tray groove",transform,brewSpot+new Vector3(i*.065f,0,0),new Vector3(.02f,.005f,.29f),art.Dark);
            var brew=art.Box("BREW",transform,new Vector3(-1.95f,1.78f,-5.36f),new Vector3(.48f,.22f,.07f),art.Cyan);Add(brew.transform,CampusPropKind.Brew,new Vector3(1,1,1));
            art.Text("BREW",transform,new Vector3(-1.95f,1.78f,-5.405f),.085f,art.Dark.color);
            Cup=CampusAssets.Place(art,"food","mug",transform,cupHome,new Vector3(.30f,.27f,.25f));Add(Cup,CampusPropKind.Cup,new Vector3(.34f,.32f,.32f),Vector3.up*.14f);
            CampusAssets.Place(art,"food","cup-saucer",transform,coaster-Vector3.up*.02f,new Vector3(.45f,.035f,.40f));
            art.Text("SERVE HERE",transform,coaster+new Vector3(0,-.18f,-.23f),.07f,art.White.color);
            var coffee=art.Mat(new Color(.12f,.055f,.025f));coffee.SetFloat("_Glossiness",.7f);
            liquid=art.Primitive("Coffee liquid",PrimitiveType.Cylinder,Cup,new Vector3(0,.04f,0),new Vector3(.16f,.004f,.16f),coffee).transform;
            stream=art.Primitive("Coffee stream",PrimitiveType.Cylinder,transform,new Vector3(-1.95f,1.38f,-5.35f),new Vector3(.018f,.11f,.018f),coffee).transform;
            for(int i=0;i<4;i++)steam.Add(art.Primitive("Warm steam",PrimitiveType.Sphere,transform,Vector3.zero,Vector3.one*.02f,art.White).transform);
            Stamp=art.Root("Physical expense stamp",transform);Stamp.localPosition=stampHome;
            art.Box("Stamp rubber",Stamp,Vector3.zero,new Vector3(.19f,.04f,.13f),art.Dark);
            art.Primitive("Stamp handle",PrimitiveType.Capsule,Stamp,new Vector3(0,.11f,0),new Vector3(.1f,.10f,.1f),art.Orange);Add(Stamp,CampusPropKind.Stamp,new Vector3(.25f,.27f,.22f),Vector3.up*.1f);
            Form=art.Root("Expense form",transform);Form.localPosition=formHome;
            art.Box("Paper sheet",Form,Vector3.zero,new Vector3(.47f,.025f,.33f),art.White);
            for(int i=0;i<4;i++)art.Box("Printed form line",Form,new Vector3(-.04f,.015f,-.08f+i*.055f),new Vector3(.26f,.003f,.008f),art.Muted);
            seal=art.Box("Approved ink impression",Form,new Vector3(.13f,.018f,.02f),new Vector3(.14f,.004f,.12f),art.Orange).transform;
            Add(Form,CampusPropKind.Form,new Vector3(.5f,.12f,.38f));
            art.Box("In tray base",transform,inbox-Vector3.up*.02f,new Vector3(.65f,.05f,.50f),art.Bronze);
            art.Text("IN TRAY",transform,inbox+new Vector3(0,-.18f,-.23f),.075f,art.White.color);
            Drawer=art.Root("Sliding archive drawer",transform);Drawer.localPosition=drawerHome;
            art.Box("Drawer base",Drawer,new Vector3(0,0,.2f),new Vector3(.75f,.04f,.65f),wood);
            var drawerFace=art.Box("Drawer front",Drawer,Vector3.zero,new Vector3(.8f,.28f,.07f),wood);
            art.Box("Drawer handle",Drawer,new Vector3(0,0,-.08f),new Vector3(.32f,.045f,.07f),art.Bronze);Add(Drawer,CampusPropKind.Drawer,new Vector3(.8f,.32f,.15f));
            art.Text("PULL",Drawer,new Vector3(0,-.08f,-.09f),.075f,art.White.color);
            Clue=art.Root("Archive route card",Drawer);Clue.localPosition=new Vector3(0,.20f,.16f);
            art.Box("Route card",Clue,Vector3.zero,new Vector3(.64f,.3f,.025f),art.White);
            art.Text("ROUTE CARD",Clue,new Vector3(0,.05f,-.02f),.07f,art.Dark.color);
            art.Text("ONE PARENT AT A TIME",Clue,new Vector3(0,-.05f,-.02f),.04f,art.Dark.color);Add(Clue,CampusPropKind.Clue,new Vector3(.68f,.34f,.12f));
            CampusAssets.Place(art,"food","cookie",transform,new Vector3(-3.15f,1.08f,-4.7f),new Vector3(.18f,.04f,.18f));
            CampusAssets.Place(art,"food","donut",transform,new Vector3(-2.85f,1.08f,-4.7f),new Vector3(.20f,.07f,.20f));
            Reset();
        }
        void Add(Transform t,CampusPropKind kind,Vector3 size,Vector3 center=default(Vector3))
        {if(t==null)throw new InvalidOperationException("Approved prop failed to load: "+kind);var c=t.gameObject.AddComponent<BoxCollider>();c.size=size;c.center=center;var p=t.gameObject.AddComponent<CampusProp>();p.Kind=kind;props.Add(p);}
        CampusProp Pick(Ray ray){return Physics.Raycast(ray,out var hit,6)?hit.collider.GetComponentInParent<CampusProp>():null;}
        public void Tick(bool enabled)
        {
            active=enabled;if(!enabled&&Holding)CancelHold();
            if(enabled)State.Advance(Mathf.Min(Time.deltaTime,.1f));
            if(wasBrewing&&!State.Brewing)record("coffee_brewed");wasBrewing=State.Brewing;
            liquid.localPosition=new Vector3(0,.04f+State.Fill*.16f,0);liquid.gameObject.SetActive(State.Fill>0);stream.gameObject.SetActive(State.Brewing);
            seal.gameObject.SetActive(State.Stamped);Clue.gameObject.SetActive(State.DrawerOpen&&!State.ClueFound);
            for(int i=0;i<steam.Count;i++){steam[i].gameObject.SetActive(enabled&&State.Fill>=1);float k=Mathf.Repeat(Time.time*.5f+i*.25f,1);steam[i].position=Cup.position+new Vector3(Mathf.Sin(i+Time.time)*.025f,.28f+k*.22f,0);steam[i].localScale=Vector3.one*(.012f+k*.018f);}
            if(stampUntil>Time.time)Stamp.position=stampRest+Vector3.up*Mathf.Abs(Mathf.Sin((stampUntil-Time.time)*Mathf.PI/.45f))*.1f;
            else if(grabbed==null||grabbed.Kind!=CampusPropKind.Stamp)Stamp.localPosition=stampHome;
            CoffeeHint.text=State.CoffeeServed?"COFFEE SECURED. THE DEADLINE REMAINS.":State.Brewing?"BREWING / PLEASE DO NOT BOIL THE BUDGET":State.Fill>=1?"DRAG FULL CUP TO THE SAUCER":State.Cup==CupPlace.Brewer?"PRESS BREW":"DRAG CUP UNDER SPOUT, THEN PRESS BREW";
            PaperHint.text=State.ClueFound?routeHint:State.DrawerOpen?"TAKE THE ROUTE CARD":State.Submitted?"PULL THE DRAWER OPEN":State.Stamped?"DRAG STAMPED FORM INTO THE IN TRAY":"DRAG STAMP ONTO FORM";
            if(!rig.XRActive)return;
            for(int i=0;i<2;i++)
            {
                bool tracked=i==0?rig.LeftTracked:rig.RightTracked,grip=tracked&&(i==0?rig.LeftGrip:rig.RightGrip);var h=i==0?rig.LeftHand:rig.RightHand;
                if(enabled&&grabbed!=null&&hand==i){if(!tracked){CancelHold();}else if(!grip){Drop();}else Move(h.position-handStart+grabStart);}
                if(enabled&&grip&&!previousGrip[i]&&!Holding)
                {foreach(var p in props)if(p.gameObject.activeInHierarchy&&Vector3.Distance(p.GetComponent<Collider>().ClosestPoint(h.position),h.position)<.12f){Begin(p,i);handStart=h.position;break;}}
                previousGrip[i]=grip;
                if(enabled&&(i==0?rig.LeftSelectPressed:rig.RightSelectPressed)&&rig.TryGetHandRay(i==1,out var ray))
                {var p=Pick(ray);if(p!=null&&(p.Kind==CampusPropKind.Brew||p.Kind==CampusPropKind.Clue))Click(p);}
            }
        }
        bool Click(CampusProp p)
        {if(p.Kind==CampusPropKind.Brew&&State.Brew()){record("coffee_brewing_started");return true;}if(p.Kind==CampusPropKind.Clue&&State.TakeClue()){record("archive_route_card_found");return true;}return false;}
        void Begin(CampusProp p,int selectedHand)
        {
            if(p.Kind==CampusPropKind.Brew||p.Kind==CampusPropKind.Clue){Click(p);return;}
            if(p.Kind==CampusPropKind.Stamp&&stampUntil>Time.time)return;
            if((p.Kind==CampusPropKind.Cup&&(State.Brewing||State.CoffeeServed))||(p.Kind==CampusPropKind.Form&&(!State.Stamped||State.Submitted))||(p.Kind==CampusPropKind.Drawer&&(!State.Submitted||State.DrawerOpen)))return;
            // Mouse dragging follows the tabletop so docking remains accurate after turning.
            grabbed=p;hand=selectedHand;grabStart=p.transform.position;dragPlane=new Plane(Vector3.up,grabStart);record("campus_prop_grabbed_"+p.Kind);
        }
        void Move(Vector3 position)
        {
            if(grabbed==null)return;
            if(grabbed.Kind==CampusPropKind.Drawer){float pull=Mathf.Clamp(grabStart.z-position.z,0,.45f);Drawer.localPosition=drawerHome+Vector3.back*pull;return;}
            grabbed.transform.position=Vector3.ClampMagnitude(position-grabStart,2)+grabStart;
        }
        void Drop()
        {
            if(grabbed==null)return;var kind=grabbed.Kind;var point=grabbed.transform.position;bool accepted=false;
            if(kind==CampusPropKind.Cup){if(Vector3.Distance(point,transform.TransformPoint(brewSpot))<.40f)accepted=State.PlaceCup(CupPlace.Brewer);else if(Vector3.Distance(point,transform.TransformPoint(coaster))<.40f)accepted=State.PlaceCup(CupPlace.Coaster);}
            if(kind==CampusPropKind.Stamp&&Vector3.Distance(point,Form.position)<.38f){accepted=State.Stamp();stampRest=Form.position+Vector3.up*.04f;stampUntil=Time.time+.45f;}
            if(kind==CampusPropKind.Form&&Vector3.Distance(point,transform.TransformPoint(inbox))<.42f)accepted=State.Submit();
            if(kind==CampusPropKind.Drawer&&drawerHome.z-Drawer.localPosition.z>.24f)accepted=State.OpenDrawer();
            record("campus_prop_"+(accepted?"placed_":"returned_")+kind);grabbed=null;hand=-1;RestorePositions();
        }
        void RestorePositions(){Cup.localPosition=State.Cup==CupPlace.Brewer?brewSpot:State.Cup==CupPlace.Coaster?coaster:cupHome;Form.localPosition=State.Submitted?inbox:formHome;Drawer.localPosition=drawerHome+(State.DrawerOpen?Vector3.back*.45f:Vector3.zero);if(stampUntil<=Time.time)Stamp.localPosition=stampHome;}
        public void CancelHold(){grabbed=null;hand=-1;RestorePositions();}
        public void Reset(){State=new CampusLifeState();stampUntil=0;wasBrewing=false;CancelHold();}
        public bool Desktop(Event e)
        {
            if(!active||rig.XRActive||e.button!=0)return false;
            var ray=rig.Head.ScreenPointToRay(new Vector3(e.mousePosition.x,Screen.height-e.mousePosition.y,0));
            if(e.type==EventType.MouseDown){var p=Pick(ray);if(p==null)return false;mouseY=e.mousePosition.y;Begin(p,-1);return true;}
            if(Holding&&(e.type==EventType.MouseDrag||e.type==EventType.MouseUp))
            {
                if(grabbed.Kind==CampusPropKind.Drawer)Move(grabStart+Vector3.back*Mathf.Clamp((e.mousePosition.y-mouseY)/Screen.height*2,0,.45f));
                else if(dragPlane.Raycast(ray,out float distance))Move(ray.GetPoint(distance));
                if(e.type==EventType.MouseUp)Drop();return true;
            }
            return false;
        }
    }
}
