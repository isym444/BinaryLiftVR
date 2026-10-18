using System;
using System.Collections.Generic;
using BinaryLift;

// Plain console checks for the engine-independent game logic (Assets/Scripts/Logic).
static class Program
{
    static int failures;

    static void Check(bool ok, string what)
    {
        if (ok) return;
        failures++;
        Console.WriteLine("FAIL: " + what);
    }

    static int Main()
    {
        KthAncestorMatchesNaive();
        GeneratorProducesValidTrees();
        LayoutKeepsSameLevelNodesApart();
        PlayerWinsWithSetBitsOnly();
        NpcWinsIfPlayerIdles();
        RulesRejectIllegalMoves();
        CampusQuestProgression();
        PhysicalClimbProgression();
        AdventureFlow();
        CampusLifeFlow();

        Console.WriteLine(failures == 0 ? "ALL PASSED" : failures + " FAILURE(S)");
        return failures == 0 ? 0 : 1;
    }

    static void ClimbSelection(AdventureState s,int span)
    {
        int from=s.Node,levels=s.TotalLevels;
        Check(s.Choose(span)&&s.Node==from&&s.TotalLevels==levels,"selection never moves player");
        Check(!s.Pull(0)&&!s.Pull(-1)&&!s.Pull(float.NaN)&&!s.Pull(float.PositiveInfinity)&&!s.Pull(.21f),"invalid pulls rejected");
        s.Pull(.1f);Check(s.Node==from&&s.TotalLevels==levels&&s.EdgeProgress>0,"partial edge not counted");
        int guard=0;
        while(s.Phase==AdventurePhase.Climbing&&guard++<1000)
        {
            int old=s.Node,oldLevels=s.TotalLevels;s.Pull(.1f);
            if(s.Node!=old)Check(s.Node==s.Tree.Parent[old]&&s.TotalLevels==oldLevels+1,"each ancestor crossing counted once");
        }
        Check(guard<1000&&s.Node==s.Tree.KthAncestor(from,span),"physical ancestor chain completed");
    }
    static void AdventureFlow()
    {
        for(int seed=0;seed<40;seed++)
        {
            var s=new AdventureState(seed);
            Check(string.Join(",",s.Tree.Parent)==string.Join(",",TreeGenerator.Generate(seed,16,18).Parent),"original topology retained");
            Check(!s.BeginChase()&&!s.Choose(1)&&!s.EnterTree()&&!s.Weave()&&!s.NextTarget(),"phases cannot be skipped");
            Check(!s.OpenQuiz()&&!s.AnswerQuiz(0),"quiz cannot skip the adventure");
            Check(!s.BeginResearch()&&!s.CancelResearch()&&s.MinimumDecisions(4)==4,"no research or shortcut before discovery");
            Check(s.ExitClassroom()&&!s.ExitClassroom()&&s.BeginChase()&&!s.BeginChase(),"exit and theft once");
            Check(s.ReachGate()&&s.EnterTree(),"map before tree");
            Check(!s.CanChoose(2)&&!s.CanChoose(4)&&!s.Choose(3)&&!s.Choose(8),"no global upgrades before exploration");
            for(int pair=0;pair<2;pair++)
            {
                ClimbSelection(s,1);ClimbSelection(s,1);
                Check(s.Phase==AdventurePhase.Clue&&!s.Choose(1),"stop at a physical cone");
                Check(s.CollectCone()&&!s.CollectCone(),"unique cone collection");
                int from=s.PendingStart,mid=s.PendingMiddle,end=s.PendingEnd;
                Check(s.PendingSpan==2&&s.Knows(from,1)&&s.Knows(mid,1),"two observed adjacent edges required");
                Check(s.Tree.Parent[mid]==end&&s.Weave()&&s.Knows(from,2),"store a node-specific two-edge connection");
            }
            Check(s.PendingSpan==4&&s.PendingMiddle==s.Tree.KthAncestor(s.StartNode,2),"four-edge composition keeps middle ancestor");
            Check(s.Weave()&&!s.Weave()&&s.Phase==AdventurePhase.Retargeting,"compose once and offer new target");
            Check(!s.Knows(s.Tree.Parent[s.StartNode],2),"memory is not a universal two-level power");
            Check(s.Cones==2&&s.MissionSelections==4&&s.TotalLevels==4,"four physical discoveries and two cones");
            Check(s.NextTarget()&&!s.NextTarget()&&s.Node==s.StartNode&&s.Goal==s.OriginalGoal,"new target preserves original final goal");
            Check(s.TotalLevels==4&&s.MissionLevels==0&&s.CanChoose(4),"return preserves memory and lifetime effort");
            ClimbSelection(s,4);
            Check(s.MissionSelections==1&&s.MissionLevels==4,"one decision replaces four, same physical distance");
            while(s.Remaining>0)ClimbSelection(s,1);
            Check(s.CollectCone()&&!s.CollectCone()&&s.Phase==AdventurePhase.Complete,"single terminal results");
            Check(s.Cones==3&&s.ReusedConnections==1,"recovery and reuse recorded");
            Check(s.EfficientRecoveries==1&&s.BestRouteDecisions==s.MissionSelections,"efficiency is based on available memory, not ideal unseen routes");
            CheckQuiz(s);
            BuildAllResearch(s);
            Check(Math.Abs(s.TotalPull-s.TotalLevels*AdventureState.PullPerEdge)<.003f,"no upgrade bypasses physical effort");
            Check(s.NextTarget()&&s.Knows(s.StartNode,4)&&s.Cones==3,"another target keeps learned routes");
            Check(s.RequiredLevels==3&&!s.CanChoose(4),"short target rejects overshooting four");
            ClimbSelection(s,2);ClimbSelection(s,1);
            Check(s.CollectCone()&&s.Cones==4,"repeat search completes in same game");
            foreach(int distance in new[]{5,7,4,6,3})
            {
                Check(s.NextTarget()&&s.RequiredLevels==distance&&s.Goal>=0,"varied targets stay within original ancestry");
                while(s.Remaining>0)ClimbSelection(s,s.CanChoose(4)?4:s.CanChoose(2)?2:1);
                Check(s.CollectCone(),"varied funding target recoverable");
                CheckQuiz(s);
                BuildAllResearch(s);
            }
            Check(new AdventureState(seed).Cones==0&&!new AdventureState(seed).Knows(s.StartNode,4),"classroom restart resets memory");
        }
    }

    static void CampusLifeFlow()
    {
        var s=new CampusLifeState();
        Check(!s.Brew()&&!s.PlaceCup(CupPlace.Coaster)&&!s.Submit()&&!s.OpenDrawer()&&!s.TakeClue(),"campus tasks require real ordered actions");
        Check(!s.PlaceCup((CupPlace)99)&&s.PlaceCup(CupPlace.Brewer)&&s.Brew()&&!s.Brew(),"cup must dock before brewing once");
        Check(!s.PlaceCup(CupPlace.Tray)&&!s.Advance(float.NaN)&&!s.Advance(10),"brewing locks cup and rejects invalid time");
        for(int i=0;i<26;i++)s.Advance(.1f);
        Check(s.Fill==1&&!s.Brewing&&s.PlaceCup(CupPlace.Coaster)&&s.CoffeeServed&&!s.PlaceCup(CupPlace.Tray),"coffee fills and serving is persistent");
        Check(s.Stamp()&&!s.Stamp()&&s.Submit()&&!s.Submit()&&s.OpenDrawer()&&s.TakeClue()&&!s.TakeClue(),"stamp submit drawer clue cannot be farmed");
        var reset=new CampusLifeState();Check(!reset.CoffeeServed&&!reset.ClueFound&&!reset.Stamped&&reset.Fill==0,"campus replay starts clean");
        var optional=new AdventureState();Check(optional.ExitClassroom(),"campus chores never gate departure");
    }
    static void CheckQuiz(AdventureState s)
    {
        int cones=s.Cones,levels=s.TotalLevels,stamps=s.ResearchStamps;
        Check(s.OpenQuiz()&&s.CloseQuiz()&&s.ResearchStamps==stamps,"optional quiz can be skipped without penalty");
        Check(s.OpenQuiz()&&!s.NextTarget()&&!s.AnswerQuiz(-1)&&!s.AnswerQuiz(2),"quiz has bounded answers and safe phase guards");
        Check(s.AnswerQuiz(1-s.CorrectQuizAnswer)&&!s.QuizSolved&&s.ResearchStamps==stamps&&s.QuizFeedback.Length>30,"wrong answer explains and allows retry");
        Check(s.AnswerQuiz(s.CorrectQuizAnswer)&&s.QuizSolved&&s.ResearchStamps==stamps+1,"correct retry earns one stamp");
        Check(!s.AnswerQuiz(s.CorrectQuizAnswer)&&s.CloseQuiz()&&s.OpenQuiz()&&!s.AnswerQuiz(s.CorrectQuizAnswer),"reopening cannot farm stamps");
        Check(s.CloseQuiz()&&s.Cones==cones&&s.TotalLevels==levels,"quiz never changes climb or cones");
        if(s.QuizTopic==2)Check(s.QuizOption(0)=="NODE "+s.Tree.Parent[s.Tree.Parent[s.StartNode]],"ancestor quiz uses the actual tree");
    }
    static int ReferenceMinimum(AdventureState s,int node,int remaining)
    {
        if(remaining==0)return 0;
        int best=remaining;
        foreach(int span in new[]{1,2,4})if(span<=remaining&&(span==1||s.Knows(node,span)))best=Math.Min(best,1+ReferenceMinimum(s,s.Tree.KthAncestor(node,span),remaining-span));
        return best;
    }
    static void BuildAllResearch(AdventureState s)
    {
        int levels=s.TotalLevels,cones=s.Cones,guard=0,frozenBest=s.BestRouteDecisions;
        while(s.CanResearch&&guard++<s.Tree.Count*2)
        {
            int count=s.ConnectionCount;
            Check(s.BeginResearch()&&s.CancelResearch()&&s.ConnectionCount==count,"research can be cancelled without mutation");
            Check(s.BeginResearch()&&s.Knows(s.PendingStart,s.PendingSpan/2)&&s.Knows(s.PendingMiddle,s.PendingSpan/2),"research requires actual adjacent memories");
            int from=s.PendingStart,span=s.PendingSpan;
            Check(s.Weave()&&s.Knows(from,span)&&s.ConnectionCount==count+1&&s.Phase==AdventurePhase.Complete,"optional research returns to same results");
            Check(!s.Weave(),"no duplicate research reward");
        }
        Check(guard<s.Tree.Count*2&&!s.BeginResearch(),"research exhausts a finite set");
        Check(s.TotalLevels==levels&&s.Cones==cones&&s.BestRouteDecisions==frozenBest,"research preserves physical progress and historical score");
        for(int k=0;k<=s.Tree.Depth[s.StartNode];k++)Check(s.MinimumDecisions(k)==ReferenceMinimum(s,s.StartNode,k),"decision benchmark matches exhaustive routes");
        if(s.OriginalLevels>=6)Check(s.MinimumDecisions(6)==2,"new knowledge makes a six-level 4+2 route possible");
    }

    static void PhysicalClimbProgression()
    {
        var p = new ClimbProgress();
        Check(p.Ancestor(9)==8, "unassisted return follows one real branch");
        Check(!p.Collect(2) && !p.Collect(-1), "only physical clue nodes collect");
        Check(p.Collect(9) && !p.Collect(9), "free exploration and unique collection");
        Check(p.Reach==2 && p.Ancestor(9)==3, "two-edge return crosses branch junction");
        p.Collect(3);
        Check(p.Ancestor(11)==3, "four-edge return stays on ancestor route");
        p.Collect(7); p.Collect(11);
        Check(p.Cones==4 && p.Reach==8, "four cones cap reach at eight");
        Check(p.Ancestor(12)==0, "eight-edge rope reaches the root from the crown");
        Check(p.Ancestor(0)==-1 && p.Ancestor(100)==-1, "no root or invalid return");
        for(int i=1;i<p.Tree.Count;i++)
        {
            int a=p.Ancestor(i);
            Check(a>=0 && p.Tree.Depth[a]<p.Tree.Depth[i], "return always moves rootward");
        }
        Check(new ClimbProgress().Cones==0, "restart has no collected cones");
    }

    static void KthAncestorMatchesNaive()
    {
        for (int seed = 0; seed < 200; seed++)
        {
            var t = TreeGenerator.Generate(seed, 16, 18);
            for (int v = 0; v < t.Count; v++)
                for (int k = -1; k <= t.Depth[v] + 2; k++)
                    Check(t.KthAncestor(v, k) == t.KthAncestorNaive(v, k),
                        "kth ancestor seed=" + seed + " v=" + v + " k=" + k);
        }

        var chain = new TreeModel(new[] { -1, 0, 1, 2, 3, 4, 5, 6, 7 });
        Check(chain.Bits == 4, "chain of depth 8 needs 4 bits, got " + chain.Bits);
        Check(chain.Up[3][8] == 0, "2^3 jump from depth 8 reaches root");
        Check(chain.Up[3][7] == -1, "2^3 jump from depth 7 runs off the root");
        Check(string.Join(",", TreeModel.SetBits(13)) == "0,2,3", "set bits of 13");
    }

    static void GeneratorProducesValidTrees()
    {
        for (int seed = 0; seed < 2000; seed++)
        {
            var t = TreeGenerator.Generate(seed, 16, 18);
            Check(t.Count == 16 + 1 + 18, "node count");
            Check(t.MaxDepth == 16, "max depth is the trunk tip");

            var race = RaceState.Create(t, seed);
            Check(race.K >= 1 && race.K <= t.Depth[race.Start], "k in range, seed " + seed);
            Check(race.Target >= 0, "target exists, seed " + seed);
            Check(race.Target == t.KthAncestor(race.Start, race.K), "target is kth ancestor");
        }
    }

    static void LayoutKeepsSameLevelNodesApart()
    {
        const float spacing = 1.3f;
        for (int seed = 0; seed < 300; seed++)
        {
            var t = TreeGenerator.Generate(seed, 16, 18);
            float[] x = TreeLayout.ComputeX(t, spacing);
            for (int a = 0; a < t.Count; a++)
                for (int b = a + 1; b < t.Count; b++)
                    if (t.Depth[a] == t.Depth[b])
                        Check(Math.Abs(x[a] - x[b]) >= spacing - 1e-3f, "overlap seed=" + seed + " " + a + "," + b);
        }
    }

    static void PlayerWinsWithSetBitsOnly()
    {
        for (int seed = 0; seed < 500; seed++)
        {
            var t = TreeGenerator.Generate(seed, 16, 18);
            var race = RaceState.Create(t, seed);
            race.Begin();

            // Take the bits in reverse order to prove ordering doesn't matter.
            var bits = TreeModel.SetBits(race.K);
            bits.Reverse();
            foreach (int j in bits) Check(race.TryPortal(j), "portal " + j + " usable, seed " + seed);

            Check(race.Phase == RacePhase.Won, "player won, seed " + seed);
            Check(race.Moves == TreeModel.PopCount(race.K), "moves == popcount(k), seed " + seed);
            Check(race.Player == race.Target, "player at target");
        }
    }

    static void NpcWinsIfPlayerIdles()
    {
        var t = TreeGenerator.Generate(7, 16, 18);
        var race = RaceState.Create(t, 7);
        race.Begin();
        for (int i = 0; i < race.K; i++) Check(race.StepNpc(), "npc step " + i);
        Check(race.Phase == RacePhase.Lost, "npc reaches target after k steps");
        Check(race.NpcSteps == race.K, "npc used exactly k steps");
        Check(!race.TryPortal(0), "no moves after the race is over");
    }

    static void RulesRejectIllegalMoves()
    {
        var t = TreeGenerator.Generate(3, 16, 18);
        var race = RaceState.Create(t, 3);

        Check(!race.TryStepTo(t.Parent[race.Player]), "no moves during countdown");
        race.Begin();

        int far = (race.Player + 5) % t.Count;
        if (t.Parent[race.Player] != far && t.Parent[far] != race.Player && far != race.Player)
            Check(!race.TryStepTo(far), "cannot step to a non-adjacent node");
        Check(!race.TryStepTo(race.Player), "cannot step to the current node");
        Check(!race.TryPortal(t.Bits), "portal index out of range");
        Check(!race.TryPortal(-1), "negative portal index");

        // A portal that overshoots the root is refused and costs nothing.
        var shallow = new TreeModel(new[] { -1, 0, 1, 2, 3, 4, 5, 6, 7, 8 });
        var r2 = RaceState.Create(shallow, 1);
        r2.Begin();
        int movesBefore = r2.Moves;
        for (int j = 0; j < shallow.Bits; j++)
            if (shallow.Up[j][r2.Player] < 0) Check(!r2.TryPortal(j), "overshooting portal refused");
        Check(r2.Moves == movesBefore, "refused portals do not count as moves");
    }

    static void CampusQuestProgression()
    {
        var q = new CampusQuestState();
        Check(!q.TryJump(1) && !q.PickUpCone(), "quest blocks actions before briefing ends");
        Check(q.Begin() && !q.Begin(), "quest starts exactly once");
        Check(!q.TryJump(2) && !q.TryJump(0) && !q.TryJump(-1), "locked and invalid jumps rejected");
        Check(!q.PickUpCone() && !q.DepositCone(), "cannot collect remotely or deposit empty hands");
        int[] legs = { 1, 2, 4 };
        foreach (int leg in legs)
        {
            Check(q.CanJump(leg), "new jump available: " + leg);
            var preview = q.Preview(leg);
            Check(preview.Count == leg + 1 && preview[0] == q.Node && preview[leg] == q.Goal, "preview follows each parent edge");
            Check(q.TryJump(leg) && q.Node == q.Goal, "jump reaches clue");
            Check(!q.TryJump(1), "cannot overshoot a clue");
            Check(q.PickUpCone() && !q.PickUpCone(), "cone cannot be duplicated");
            Check(!q.TryJump(1), "cannot travel while carrying cone");
            Check(q.ReturnCone() && q.PickUpCone(), "dropped cone can be recovered");
            Check(q.DepositCone() && !q.DepositCone(), "deposit unlocks exactly once");
        }
        Check(q.Remaining == 13 && q.MaxJump == 8, "final challenge teaches 13 = 8 + 4 + 1");
        Check(!q.TryJump(3) && !q.TryJump(16), "non-powers and unavailable powers rejected");
        Check(q.TryJump(8) && !q.TryJump(8) && q.TryJump(4) && q.TryJump(1), "final bit decomposition");
        Check(q.PickUpCone() && q.DepositCone(), "last clue stored");
        Check(q.Phase == QuestPhase.Complete && q.Cones == 4 && q.Moves == 6, "complete in six jumps");
        Check(!q.Begin() && !q.TryJump(1) && !q.PickUpCone() && !q.ReturnCone(), "completion is terminal");
        var visited = q.BreadthFirstOrder();
        var unique = new HashSet<int>(visited);
        Check(visited.Count == q.Tree.Count && unique.Count == q.Tree.Count, "BFS visits every node once");
        for (int i = 1; i < visited.Count; i++)
            Check(q.Tree.Depth[visited[i]] >= q.Tree.Depth[visited[i-1]], "BFS visits nondecreasing depth");
        var reset = new CampusQuestState();
        Check(reset.Node == 20 && reset.Cones == 0 && reset.Moves == 0 && reset.Phase == QuestPhase.Briefing, "fresh session resets progression");
    }
}
