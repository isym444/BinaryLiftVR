using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BinaryLift.EditorTools
{
    [InitializeOnLoad]
    public static class TreeClimbValidation
    {
        const string Key="TreeClimbValidation.Pending";
        static TreeClimbValidation(){EditorApplication.update+=Tick;}
        [MenuItem("BinaryLift/Validate Tree Climb %#t")]
        public static void StartSmoke()
        {
            if(EditorApplication.isPlaying){Debug.LogWarning("Stop Play Mode first.");return;}
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;
        }
        static void Tick()
        {
            if(!SessionState.GetBool(Key,false)||!EditorApplication.isPlaying||EditorApplication.isCompiling)return;
            var climb=UnityEngine.Object.FindFirstObjectByType<TreeClimbingController>();
            if(climb==null)return;
            SessionState.SetBool(Key,false);
            string report="Tree climb desktop integration\n";int failures=0;
            Action<bool,string> check=(pass,label)=>{report+=(pass?"PASS ":"FAIL ")+label+"\n";if(!pass)failures++;};
            try
            {
                check(climb.Nodes.Count==13,"physical tree contains trunk and both side branches");
                check(climb.Bark.Count>50,"bark and root grip colliders constructed");
                check(climb.World.BeginButton==null,"no route quiz terminal in default experience");
                check(climb.TouchingBark(climb.Nodes[3],out _),"branch junction accepts nearby grip");
                check(!climb.TouchingBark(new Vector3(20,20,20),out _),"distant hand cannot grab bark");
                var move=typeof(TreeClimbingController).GetMethod("MoveBody",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
                Vector3 start=climb.Rig.transform.position;
                move.Invoke(climb,new object[]{Vector3.up*.06f});
                check(Mathf.Abs(climb.Rig.transform.position.y-start.y-.06f)<.001f,"pull displacement moves rig vertically");
                move.Invoke(climb,new object[]{Vector3.up*10});
                check(climb.Rig.transform.position.y-start.y<=.161f,"large displacement is bounded");
                climb.ResetClimb();
                var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
                Action<string,bool> input=(name,value)=>typeof(XRRig).GetField("<"+name+">k__BackingField",flags).SetValue(climb.Rig,value);
                var hands=typeof(TreeClimbingController).GetMethod("UpdateHands",flags);
                input("LeftTracked",true);input("RightTracked",true);
                input("LeftGrip",true);input("RightGrip",true);
                climb.Rig.LeftHand.position=climb.Nodes[1]+new Vector3(-.2f,0,-.50f);
                climb.Rig.RightHand.position=climb.Nodes[1]+new Vector3(.2f,0,-.50f);
                hands.Invoke(climb,null);
                float before=climb.Rig.transform.position.y;
                climb.Rig.LeftHand.position+=Vector3.down*.06f;
                climb.Rig.RightHand.position+=Vector3.down*.06f;
                hands.Invoke(climb,null);
                check(Mathf.Abs(climb.Rig.transform.position.y-before-.06f)<.003f,"two downward hands lift body once, not twice");
                input("LeftTracked",false);input("RightTracked",false);
                before=climb.Rig.transform.position.y;
                climb.Rig.LeftHand.position+=Vector3.down*2;
                hands.Invoke(climb,null);
                check(climb.Rig.transform.position.y==before,"tracking loss releases and holds belay");
                input("XRActive",true);input("RightTracked",true);input("RightGrip",true);
                var collect=typeof(TreeClimbingController).GetMethod("CollectNearby",flags);
                var cones=(System.Collections.Generic.Dictionary<int,Transform>)typeof(TreeClimbingController).GetField("cones",flags).GetValue(climb);
                climb.Rig.RightHand.position=cones[9].position+Vector3.up*.12f;
                collect.Invoke(climb,null);collect.Invoke(climb,null);
                check(climb.Progress.Cones==1&&!cones[9].gameObject.activeSelf&&climb.World.TrophyCones[0].gameObject.activeSelf,
                    "near grip collects once, hides cone and displays horse trophy");
                input("XRActive",false);input("RightTracked",false);input("RightGrip",false);input("LeftGrip",false);
                climb.Progress.Collect(9);climb.Progress.Collect(3);
                check(climb.Progress.Ancestor(11)==3,"upgrade uses four actual ancestor edges");
                Vector3 horse=climb.World.Horse.position;
                climb.ResetClimb();
                check(climb.Progress.Cones==0&&climb.Rig.transform.position.y==0,"restart resets collection and rig");
                check(climb.World.Horse.position==horse,"horse remains at ground base");
            }
            catch(Exception e){failures++;report+="FAIL "+e+"\n";}
            report+="Result: "+(failures==0?"PASS":"FAIL")+"\nHeadset/controller/comfort/performance: NOT RUN\n";
            Directory.CreateDirectory("Logs");File.WriteAllText("Logs/tree-climb-smoke.txt",report);Debug.Log(report);
        }
    }
}
