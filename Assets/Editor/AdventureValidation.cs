using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BinaryLift.EditorTools
{
    [InitializeOnLoad]
    public static class AdventureValidation
    {
        const string Key="AdventureValidation.Running";
        static double deadline;
        static int failures,stage,lastCheckpoint;
        static string report;
        static AdventureValidation(){EditorApplication.update+=Tick;}
        [MenuItem("BinaryLift/Validate Adventure %#j")]
        public static void StartSmoke()
        {
            if(EditorApplication.isPlaying){Debug.LogWarning("Stop Play before validation.");return;}
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            stage=failures=0;lastCheckpoint=-1;report="Adventure integration checks\n";
            deadline=EditorApplication.timeSinceStartup+100;SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;
        }
        [MenuItem("BinaryLift/Capture Adventure Screenshot %#k")]
        public static void Capture()
        {
            var game=UnityEngine.Object.FindFirstObjectByType<AdventureController>();
            if(game==null||!EditorApplication.isPlaying)return;
            Directory.CreateDirectory("Docs/Evidence");
            ScreenCapture.CaptureScreenshot("Docs/Evidence/Adventure-"+game.State.Phase+"-"+DateTime.UtcNow.ToString("HHmmss")+".png");
        }
        static void Check(bool condition,string label){report+=(condition?"PASS ":"FAIL ")+label+"\n";if(!condition)failures++;}
        static void Finish()
        {
            SessionState.SetBool(Key,false);report+="Result: "+(failures==0?"PASS":"FAIL")+"\nHeadset validation: NOT RUN\n";
            Directory.CreateDirectory("Logs");File.WriteAllText("Logs/adventure-smoke.txt",report);Debug.Log(report);
        }
        static void Tick()
        {
            if(!SessionState.GetBool(Key,false)||EditorApplication.isCompiling)return;
            if(deadline<=0){deadline=EditorApplication.timeSinceStartup+100;report="Adventure integration checks\n";lastCheckpoint=-1;}
            if(EditorApplication.timeSinceStartup>deadline){Check(false,"100 second timeout");Finish();return;}
            var game=UnityEngine.Object.FindFirstObjectByType<AdventureController>();if(game==null||!EditorApplication.isPlaying||game.Busy)return;
            try
            {
                if(stage==0)
                {
                    Check(game.State.Phase==AdventurePhase.Arrival,"arrival waits in classroom");
                    Check(!game.Activate(AdventureAction.TwoLevels),"selection cannot skip classroom");
                    Check(game.Activate(AdventureAction.Exit),"walk through classroom exit");stage=1;
                }
                else if(stage==1)
                {
                    if(game.State.Phase==AdventurePhase.Chase&&lastCheckpoint!=game.Checkpoint)
                    {lastCheckpoint=game.Checkpoint;Check(game.Activate(AdventureAction.Waypoint),"walk checkpoint "+lastCheckpoint);}
                    if(game.State.Phase==AdventurePhase.Choosing)
                    {Check(game.Checkpoint==game.CheckpointCount,"all checkpoints precede tree reveal");Check(game.World.Platforms.Count==35,"original 35-node structure");stage=2;}
                }
                else if(stage==2)
                {
                    if(game.State.Phase==AdventurePhase.Choosing)
                    {
                        int before=game.State.TotalLevels;
                        Check(game.Activate(game.State.Remaining>=4?AdventureAction.FourLevels:game.State.Remaining>=2?AdventureAction.TwoLevels:AdventureAction.OneLevel),"select available connection");
                        Check(game.State.TotalLevels==before,"selection does not climb");
                    }
                    else if(game.State.Phase==AdventurePhase.Climbing)
                    {
                        float before=game.State.Progress;Check(!game.ApplyPull(0)&&game.State.Progress==before,"stationary grip cannot climb");game.ApplyPull(.1f);
                    }
                    else if(game.State.Phase==AdventurePhase.Clue)
                    {Check(game.State.TotalLevels==game.State.RequiredLevels,"all physical levels counted");Check(game.Activate(AdventureAction.Cone),"cone ends journey");stage=3;}
                }
                else if(stage==3)
                {
                    Check(game.State.Phase==AdventurePhase.Complete,"single complete state");
                    Check(game.World.EndCard.gameObject.activeSelf&&game.World.Art.TrophyCones[0].gameObject.activeSelf,"results and collected cone visible");
                    Check(UnityEngine.Object.FindFirstObjectByType<RaceController>()==null,"no second game controller");
                    Check(game.Activate(AdventureAction.Replay),"replay accepted");stage=4;
                }
                else if(stage==4)
                {
                    Check(game.State.Phase==AdventurePhase.Arrival&&game.State.TotalLevels==0&&game.Rig.transform.position.z<0,"replay returns to clean classroom");
                    Check(UnityEngine.Object.FindObjectsByType<AdventureController>(FindObjectsSortMode.None).Length==1,"one adventure controller after replay");
                    Finish();
                }
            }
            catch(Exception e){Check(false,e.ToString());Finish();}
        }
    }
}
