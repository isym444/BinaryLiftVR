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
            SessionState.SetBool(Key+".Workshop",false);
            SessionState.SetBool(Key+".Results",false);
            deadline=EditorApplication.timeSinceStartup+100;SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;
        }
        [MenuItem("BinaryLift/Prepare Workshop Input Check %#i")]
        public static void PrepareWorkshop()
        {
            if(EditorApplication.isPlaying)return;
            StartSmoke();SessionState.SetBool(Key+".Workshop",true);
        }
        [MenuItem("BinaryLift/Prepare Results Input Check %#o")]
        public static void PrepareResults()
        {
            if(EditorApplication.isPlaying)return;
            StartSmoke();SessionState.SetBool(Key+".Results",true);
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
        static bool TextFits(Transform panel,Camera camera)
        {
            foreach(var label in panel.GetComponentsInChildren<TextMesh>())
            {
                if(string.IsNullOrEmpty(label.text))continue;
                var bounds=label.GetComponent<Renderer>().bounds;
                foreach(var point in new[]{bounds.min,bounds.max})
                {
                    var view=camera.WorldToViewportPoint(point);
                    if(view.z<=0||view.x<.02f||view.x>.98f||view.y<.02f||view.y>.98f)
                    {report+="Layout overflow: "+label.text.Replace('\n',' ')+" at "+view+"\n";return false;}
                }
            }
            return true;
        }
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
                    var surface=Resources.Load<Shader>("CampusSurface");
                    Check(surface!=null&&!ShaderUtil.ShaderHasError(surface),"campus surface shader compiles");
                    var pbr=Resources.Load<Shader>("CampusPBR");
                    Check(pbr!=null&&!ShaderUtil.ShaderHasError(pbr),"approved PBR shader compiles");
                    Check(game.Life!=null&&game.Life.Cup!=null&&game.Life.Stamp!=null&&game.Life.Drawer!=null,"hands-on classroom props instantiated");
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
                        Check(game.Activate(game.State.CanChoose(4)?AdventureAction.FourLevels:game.State.CanChoose(2)?AdventureAction.TwoLevels:AdventureAction.OneLevel),"select available connection");
                        Check(game.State.TotalLevels==before,"selection does not climb");
                    }
                    else if(game.State.Phase==AdventurePhase.Climbing)
                    {
                        float before=game.State.Progress;Check(!game.ApplyPull(0)&&game.State.Progress==before,"stationary grip cannot climb");game.ApplyPull(.1f);
                    }
                    else if(game.State.Phase==AdventurePhase.Clue)
                    {Check(game.Activate(AdventureAction.Cone),"physical cone collected once");if(game.State.Phase==AdventurePhase.Complete)stage=3;}
                    else if(game.State.Phase==AdventurePhase.Weaving)
                    {
                        if(SessionState.GetBool(Key+".Workshop",false))
                        {
                            SessionState.SetBool(Key,false);
                            Debug.Log("Workshop input fixture ready. Prior steps used direct methods; subsequent input must be tested manually.");return;
                        }
                        Check(!game.Activate(AdventureAction.LinkEnd),"socket requires a grabbed connector");
                        Check(game.Activate(AdventureAction.LinkStart)&&game.Activate(AdventureAction.LinkEnd),"join discovered route segments");
                    }
                    else if(game.State.Phase==AdventurePhase.Retargeting)
                    {
                        Check(game.State.Knows(game.State.StartNode,4),"four-edge memory built");
                        Check(TextFits(game.World.NewSearch,game.Rig.Head),"new-target text has viewport margins");
                        Check(game.Activate(AdventureAction.NextTarget),"change target in same tree");
                    }
                }
                else if(stage==3)
                {
                    if(SessionState.GetBool(Key+".Results",false))
                    {
                        SessionState.SetBool(Key,false);
                        Debug.Log("Results input fixture ready. Prior steps used direct methods; test optional quiz manually.");return;
                    }
                    Check(game.State.Phase==AdventurePhase.Complete,"single complete state");
                    Check(game.State.Cones==3&&game.State.ReusedConnections==1,"two discovery cones and one reused route recovery");
                    Check(game.State.MissionLevels==game.State.RequiredLevels,"all target levels physically climbed");
                    Check(Resources.Load<Texture2D>("Approved/Sandstone")!=null&&Resources.Load<Texture2D>("Approved/FundingPoster")!=null,"approved artwork available");
                    Check(game.World.Classroom.Find("Kenney desk")!=null&&game.World.Classroom.Find("Kenney chairDesk")!=null,"approved furniture instantiated");
                    Check(game.World.EndCard.gameObject.activeSelf&&game.World.Art.TrophyCones[0].gameObject.activeSelf,"results and collected cone visible");
                    Check(UnityEngine.Object.FindFirstObjectByType<RaceController>()==null,"no second game controller");
                    Check(game.Activate(AdventureAction.OpenQuiz)&&TextFits(game.World.QuizPanel,game.Rig.Head),"optional quiz opens with readable bounds");
                    Check(game.Activate(AdventureAction.QuizB)&&!game.State.QuizSolved,"wrong answer remains retryable");
                    Check(game.Activate(AdventureAction.QuizA)&&game.State.ResearchStamps==1,"correct answer earns one audit stamp");
                    Check(!game.Activate(AdventureAction.QuizA),"stamp cannot be collected twice");
                    Check(game.Activate(AdventureAction.CloseQuiz)&&game.State.Phase==AdventurePhase.Complete,"quiz returns to same results");
                    Check(game.State.BestRouteDecisions==3&&game.State.EfficientRecoveries==1,"route efficiency benchmark uses current memory");
                    Check(TextFits(game.World.EndCard,game.Rig.Head),"results text has viewport margins");
                    Check(game.Activate(AdventureAction.Research)&&game.State.PendingSpan==2,"newly explored branches support more research");
                    Check(game.Activate(AdventureAction.CancelResearch)&&game.State.Phase==AdventurePhase.Complete,"optional research can be cancelled");
                    Check(game.Activate(AdventureAction.Research)&&game.Activate(AdventureAction.LinkStart)&&game.Activate(AdventureAction.LinkEnd),"construct additional branch memory");stage=5;
                }
                else if(stage==5)
                {
                    Check(game.State.ResearchedConnections==1&&game.State.Phase==AdventurePhase.Complete,"research completes inside original results");
                    Check(game.State.MinimumDecisions(6)==2&&game.State.BestRouteDecisions==3,"new memory improves future routes without rewriting past score");
                    Check(game.Activate(AdventureAction.NextTarget),"continue to short target after research");stage=6;
                }
                else if(stage==6)
                {
                    Check(game.State.RequiredLevels==3&&!game.State.CanChoose(4)&&game.State.CanChoose(2),"short target offers valid remembered route only");
                    Check(game.World.Controls.gameObject.activeSelf&&game.State.Cones==3,"repeat search retains cones and controls");
                    stage=7;
                }
                else if(stage==7)
                {
                    if(game.State.Phase==AdventurePhase.Choosing){Check(game.Activate(game.State.CanChoose(2)?AdventureAction.TwoLevels:AdventureAction.OneLevel),"reuse short valid route");}
                    else if(game.State.Phase==AdventurePhase.Climbing)game.ApplyPull(.1f);
                    else if(game.State.Phase==AdventurePhase.Clue){Check(game.Activate(AdventureAction.Cone),"repeat search cone");}
                    else if(game.State.Phase==AdventurePhase.Complete){Check(game.Activate(AdventureAction.Replay),"replay accepted");stage=4;}
                }
                else if(stage==4)
                {
                    Check(game.State.Phase==AdventurePhase.Arrival&&game.State.TotalLevels==0&&game.State.Cones==0&&!game.State.Knows(game.State.StartNode,4)&&game.Rig.transform.position.z<0,"replay returns to clean classroom");
                    Check(UnityEngine.Object.FindObjectsByType<AdventureController>(FindObjectsSortMode.None).Length==1,"one adventure controller after replay");
                    Check(game.Life.State.Fill==0&&!game.Life.State.Stamped&&!game.Life.State.DrawerOpen&&!game.Life.Holding,"campus props reset with replay");
                    Finish();
                }
            }
            catch(Exception e){Check(false,e.ToString());Finish();}
        }
    }
}
