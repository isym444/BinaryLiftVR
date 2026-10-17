using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BinaryLift.EditorTools
{
    [InitializeOnLoad]
    public static class RaceValidation
    {
        const string Key="RaceValidation.Running";
        static int stage, jumpIndex, failures;
        static double deadline;
        static string report;
        static System.Collections.Generic.List<int> jumps;
        static RaceValidation(){EditorApplication.update+=Tick;}
        [MenuItem("BinaryLift/Validate Original Race %#v")]
        public static void StartSmoke()
        {
            if(EditorApplication.isPlaying){Debug.LogWarning("Stop Play Mode before validation.");return;}
            stage=0;failures=0;report="Original race regression checks\n";
            deadline=EditorApplication.timeSinceStartup+45;
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            new GameObject("Original race validation").AddComponent<RaceController>();
            SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;
        }
        static void Check(bool pass,string label){report+=(pass?"PASS ":"FAIL ")+label+"\n";if(!pass)failures++;}
        static void Finish()
        {
            SessionState.SetBool(Key,false);
            report+="Result: "+(failures==0?"PASS":"FAIL")+"\nInput: direct actions + physics ray checks; headset NOT RUN\n";
            Directory.CreateDirectory("Logs");File.WriteAllText("Logs/original-race-smoke.txt",report);Debug.Log(report);
        }
        static void Tick()
        {
            if(!SessionState.GetBool(Key,false)||EditorApplication.isCompiling)return;
            if(deadline<=0){deadline=EditorApplication.timeSinceStartup+45;report="Original race regression checks\n";}
            if(EditorApplication.timeSinceStartup>deadline){Check(false,"45-second timeout");Finish();return;}
            if(!EditorApplication.isPlaying)return;
            var game=UnityEngine.Object.FindFirstObjectByType<RaceController>();if(game==null)return;
            try
            {
                if(stage==0)
                {
                    Check(game.WaitingForStart&&game.State.NpcSteps==0,"race waits for explicit start");
                    Check(UnityEngine.Object.FindFirstObjectByType<TreeClimbingController>()==null,"rejected tree not bootstrapped");
                    Check(!game.SelectTarget(TargetKind.Portal,0),"cannot jump before start");
                    Check(game.SelectTarget(TargetKind.Start),"start action begins countdown");
                    jumps=TreeModel.SetBits(game.State.K);jumps.Reverse();jumpIndex=0;stage=1;
                }
                else if(stage==1 && game.State.Phase==RacePhase.Running && !game.IsBusy)
                {
                    Check(!game.SelectTarget(TargetKind.Portal,99),"invalid portal does not move player");
                    stage=2;
                }
                else if(stage==2 && !game.IsBusy)
                {
                    if(jumpIndex<jumps.Count)
                    {
                        int bit=jumps[jumpIndex++];
                        var portal=Array.Find(UnityEngine.Object.FindObjectsByType<HoverTarget>(FindObjectsSortMode.None),t=>t.Kind==TargetKind.Portal&&t.Index==bit);
                        var camera=Camera.main;
                        Vector3 direction=portal.transform.position-camera.transform.position;
                        Check(Physics.Raycast(camera.transform.position,direction.normalized,out RaycastHit hit,direction.magnitude+.1f)
                            &&hit.collider.GetComponent<HoverTarget>()==portal,"portal "+(1<<bit)+" has unobstructed clickable collider");
                        Check(game.SelectTarget(TargetKind.Portal,bit),"portal "+(1<<bit)+" accepted");
                        Check(!game.SelectTarget(TargetKind.Portal,0),"duplicate action blocked during teleport");
                    }
                    else
                    {
                        Check(game.State.Phase==RacePhase.Won&&game.State.Moves==TreeModel.PopCount(game.State.K),"full race wins with optimal jumps");
                        int originalK=game.State.K,originalStart=game.State.Start;
                        Check(game.SelectTarget(TargetKind.Retry),"visible retry action accepted");
                        Check(game.State.K==originalK&&game.State.Start==originalStart&&game.WaitingForStart,"retry preserves tree and resets readiness");
                        game.SelectTarget(TargetKind.Start);stage=3;
                    }
                }
                else if(stage==3 && game.State.Phase==RacePhase.Running)
                {
                    for(int i=0;i<game.State.K;i++)game.State.StepNpc();
                    Check(game.State.Phase==RacePhase.Lost,"runner can win when player does not move");
                    stage=4;
                }
                else if(stage==4)
                {
                    Check(game.SelectTarget(TargetKind.NewRound)&&game.WaitingForStart,"new tree available after losing");
                    game.StartRound(13);Finish();
                }
            }
            catch(Exception e){Check(false,e.ToString());Finish();}
        }
    }
}
