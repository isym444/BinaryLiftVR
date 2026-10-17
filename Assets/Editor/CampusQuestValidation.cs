using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BinaryLift.EditorTools
{
    /// <summary>Repeatable desktop Play Mode integration check. Does not simulate an XR device.</summary>
    [InitializeOnLoad]
    public static class CampusQuestValidation
    {
        const string Running = "CampusQuest.SmokeRunning";
        static double next;
        static int stage;
        static int failures;
        static readonly int[] jumps = { 1, 2, 4, 8, 4, 1 };
        static int jumpIndex;
        static string report;

        static CampusQuestValidation() { EditorApplication.update += Tick; }

        [MenuItem("BinaryLift/Validate Campus Quest (Play Mode)")]
        public static void StartSmoke()
        {
            if (EditorApplication.isPlaying) { Debug.LogWarning("Stop Play Mode before starting validation."); return; }
            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
            // The default bootstrap is now the climbing slice; explicitly opt into the legacy test.
            new GameObject("Legacy campus validation").AddComponent<CampusQuestController>();
            SessionState.SetBool(Running,true);
            EditorApplication.isPlaying = true;
        }

        static void Check(bool condition,string label)
        {
            report += (condition ? "PASS " : "FAIL ") + label + "\n";
            if (!condition) failures++;
        }

        static void Tick()
        {
            if(!SessionState.GetBool(Running,false) || !EditorApplication.isPlaying || EditorApplication.isCompiling) return;
            if(EditorApplication.timeSinceStartup<next) return;
            var quest=UnityEngine.Object.FindFirstObjectByType<CampusQuestController>();
            if(quest==null) return;
            next=EditorApplication.timeSinceStartup+.7;
            if(quest.Busy) return;
            try
            {
                if(stage==0)
                {
                    failures=0; jumpIndex=0; report="Campus Quest desktop integration checks\n";
                    Check(quest.State.Phase==QuestPhase.Briefing,"initial briefing");
                    Check(quest.World.Targets.Count==9,"nine interaction targets constructed");
                    Check(quest.World.MapNodes.Count==27,"route tree renders all nodes");
                    Check(quest.World.Horse!=null && quest.World.Monkey!=null,"placeholder characters constructed");
                    quest.Activate(quest.World.BeginButton); stage=1;
                }
                else if(stage==1)
                {
                    if(quest.State.Phase==QuestPhase.Complete){stage=2;return;}
                    if(quest.State.Remaining==0)
                    {
                        Check(quest.World.Cone.gameObject.activeSelf,"clue cone visible at goal");
                        quest.Activate(quest.World.ConeTarget);
                        Check(quest.State.Phase==QuestPhase.CarryingCone,"ray pickup enters carrying phase");
                        quest.Activate(quest.World.SaddleTarget);
                        Check(quest.State.Cones>0,"saddle deposit accepted");
                        Check(quest.World.TrophyCones[quest.State.Cones-1].gameObject.activeSelf,"horse displays recovered cone");
                    }
                    else
                    {
                        if(jumpIndex>=jumps.Length) throw new Exception("Expected route did not complete");
                        int jump=jumps[jumpIndex++];
                        var target=quest.World.JumpButtons.Find(t=>t.Value==jump);
                        Check(target.Available,"jump enabled: "+jump);
                        quest.Activate(target);
                    }
                }
                else if(stage==2)
                {
                    Check(quest.State.Moves==6 && quest.State.Cones==4,"full route completes in six jumps");
                    Check(quest.World.Objective.text.Contains("All four"),"completion presentation");
                    quest.RestartQuest();stage=3;
                }
                else
                {
                    Check(quest.State.Node==20 && quest.State.Cones==0 && quest.State.Phase==QuestPhase.Briefing,"restart returns to clean briefing");
                    Check(!quest.World.Cone.gameObject.activeSelf,"restart hides collectible");
                    foreach(var cone in quest.World.TrophyCones)Check(!cone.gameObject.activeSelf,"restart clears trophy");
                    Finish();
                }
            }
            catch(Exception e){failures++;report+="FAIL "+e+"\n";Finish();}
        }

        static void Finish()
        {
            SessionState.SetBool(Running,false);stage=0;
            Directory.CreateDirectory("Logs");
            report+="Result: "+(failures==0?"PASS":"FAIL")+"\nHeadset validation: NOT RUN\n";
            File.WriteAllText("Logs/campus-smoke.txt",report);
            Debug.Log(report);
            // Leave the editor open in the fresh briefing for manual inspection.
        }
    }
}
