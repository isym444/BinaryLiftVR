using System;
using System.Collections.Generic;
namespace BinaryLift
{
    public enum AdventurePhase { Arrival, Courtyard, Chase, Mapping, Choosing, Climbing, Clue, Weaving, Retargeting, Complete, Quiz }
    /// <summary>Physical exploration teaches node-specific binary-lifting connections.</summary>
    public sealed class AdventureState
    {
        public readonly TreeModel Tree;
        public readonly int StartNode, OriginalGoal, OriginalLevels;
        readonly HashSet<int> traversed = new HashSet<int>();
        readonly HashSet<int> connections = new HashSet<int>();
        public AdventurePhase Phase { get; private set; } = AdventurePhase.Arrival;
        public int Goal { get; private set; }
        public int RequiredLevels { get; private set; } = 4;
        public int Node { get; private set; }
        public int Destination { get; private set; } = -1;
        public int Span { get; private set; }
        public int CompletedInSelection { get; private set; }
        public int TotalLevels { get; private set; }
        public int MissionLevels { get; private set; }
        public int Selections { get; private set; }
        public int MissionSelections { get; private set; }
        public int Cones { get; private set; }
        public int Searches { get; private set; }
        public int ReusedConnections { get; private set; }
        public int ResearchStamps { get; private set; }
        public int BestRouteDecisions { get; private set; } = 4;
        public int EfficientRecoveries { get; private set; }
        public int ResearchedConnections { get; private set; }
        public bool Researching { get; private set; }
        public int ConnectionCount => connections.Count;
        public bool CanResearch { get { int from,span;return Phase==AdventurePhase.Complete&&FindResearch(out from,out span); } }
        public bool QuizSolved { get; private set; }
        public string QuizFeedback { get; private set; } = "Optional. No grades, no banana-based penalties.";
        public int QuizTopic => Math.Max(0,Searches-1)%3;
        public int CorrectQuizAnswer => QuizTopic==1?1:0;
        public string QuizQuestion => QuizTopic==0?"The monkey claims 2 + 2 makes eight levels.\nWhat do two adjacent two-level routes make?":QuizTopic==1?"Three levels remain. You know routes of 1, 2 and 4.\nWhich longest route can fit without passing the goal?":"Start at node "+StartNode+". Follow two parent edges.\nWhich node receives the funding envelope?";
        public string QuizOption(int index)
        {
            if(QuizTopic==0)return index==0?"4 LEVELS":"8 LEVELS";
            if(QuizTopic==1)return index==0?"4 LEVELS":"2 LEVELS";
            return "NODE "+Tree.KthAncestor(StartNode,index==0?2:1);
        }
        public bool OpenQuiz(){if(Phase!=AdventurePhase.Complete)return false;Phase=AdventurePhase.Quiz;return true;}
        public bool CloseQuiz(){if(Phase!=AdventurePhase.Quiz)return false;Phase=AdventurePhase.Complete;return true;}
        public bool AnswerQuiz(int index)
        {
            if(Phase!=AdventurePhase.Quiz||QuizSolved||index<0||index>1)return false;
            string explanation=QuizTopic==0?"Two adjacent twos cover FOUR parent edges.":QuizTopic==1?"Two fits inside three; four passes the target.":"parent(parent("+StartNode+")) = "+Tree.KthAncestor(StartNode,2)+".";
            if(index!=CorrectQuizAnswer){QuizFeedback="The monkey's accounting needs work. Try again.\n"+explanation;return true;}
            QuizSolved=true;ResearchStamps++;QuizFeedback="AUDIT STAMP EARNED. Banana fraud prevented.\n"+explanation;return true;
        }
        public int PendingSpan { get; private set; }
        public int PendingStart { get; private set; } = -1;
        public int PendingMiddle => PendingStart < 0 ? -1 : Tree.KthAncestor(PendingStart, PendingSpan / 2);
        public int PendingEnd => PendingStart < 0 ? -1 : Tree.KthAncestor(PendingStart, PendingSpan);
        public float EdgeProgress { get; private set; }
        public float TotalPull { get; private set; }
        public float Progress => Span == 0 ? 0 : (CompletedInSelection + EdgeProgress) / Span;
        public int Remaining => RequiredLevels - MissionLevels;
        public int ConeNode => Searches == 0 && MissionLevels < 2 ? Tree.KthAncestor(StartNode, 2) : Goal;
        public const float PullPerEdge = .65f;

        public AdventureState(int seed = 13)
        {
            Tree = TreeGenerator.Generate(seed, 16, 18);
            var original = RaceState.Create(Tree, seed);
            StartNode = Node = original.Start; OriginalGoal = original.Target; OriginalLevels = original.K;
            Goal = Tree.KthAncestor(StartNode, RequiredLevels);
        }
        public bool ExitClassroom() { if (Phase != AdventurePhase.Arrival) return false; Phase = AdventurePhase.Courtyard; return true; }
        public bool BeginChase() { if (Phase != AdventurePhase.Courtyard) return false; Phase = AdventurePhase.Chase; return true; }
        public bool ReachGate() { if (Phase != AdventurePhase.Chase) return false; Phase = AdventurePhase.Mapping; return true; }
        public bool EnterTree() { if (Phase != AdventurePhase.Mapping) return false; Phase = AdventurePhase.Choosing; return true; }
        public bool Knows(int node, int span)
        {
            if (node < 0 || node >= Tree.Count) return false;
            return span == 1 ? traversed.Contains(node) : (span == 2 || span == 4) && connections.Contains(node * 8 + span);
        }
        public bool CanChoose(int span) => Phase == AdventurePhase.Choosing &&
            (span == 1 || ((span == 2 || span == 4) && Knows(Node, span))) && span <= Remaining;
        public bool Choose(int span)
        {
            if (!CanChoose(span)) return false;
            Span = span; Destination = Tree.KthAncestor(Node, span); CompletedInSelection = 0; EdgeProgress = 0;
            Selections++; MissionSelections++; if (span > 1) ReusedConnections++;
            Phase = AdventurePhase.Climbing; return true;
        }
        public bool Pull(float metres)
        {
            if (Phase != AdventurePhase.Climbing || float.IsNaN(metres) || float.IsInfinity(metres) || metres <= 0 || metres > .2f) return false;
            while (metres > 0 && Phase == AdventurePhase.Climbing)
            {
                float accepted = Math.Min(metres, (1 - EdgeProgress) * PullPerEdge);
                TotalPull += accepted; metres -= accepted; EdgeProgress += accepted / PullPerEdge;
                if (EdgeProgress >= .99999f)
                {
                    traversed.Add(Node); Node = Tree.Parent[Node]; TotalLevels++; MissionLevels++; CompletedInSelection++; EdgeProgress = 0;
                    if (CompletedInSelection == Span)
                        Phase = Node == Goal || (Searches == 0 && MissionLevels == 2 && Cones == 0) ? AdventurePhase.Clue : AdventurePhase.Choosing;
                }
            }
            return true;
        }
        public bool CollectCone()
        {
            if (Phase != AdventurePhase.Clue) return false;
            Cones++;
            if (Searches > 0) { if(MissionSelections==BestRouteDecisions)EfficientRecoveries++;Phase = AdventurePhase.Complete; return true; }
            PendingSpan = 2; PendingStart = Tree.KthAncestor(StartNode, MissionLevels - 2);
            Phase = AdventurePhase.Weaving; return true;
        }
        public bool Weave()
        {
            if (Phase != AdventurePhase.Weaving || (PendingSpan != 2 && PendingSpan != 4) ||
                !Knows(PendingStart, PendingSpan / 2) || !Knows(PendingMiddle, PendingSpan / 2) ||
                Knows(PendingStart, PendingSpan)) return false;
            connections.Add(PendingStart * 8 + PendingSpan);
            if(Researching){ResearchedConnections++;Researching=false;PendingSpan=0;PendingStart=-1;Phase=AdventurePhase.Complete;return true;}
            if (PendingSpan == 2 && MissionLevels == 4) { PendingSpan = 4; PendingStart = StartNode; }
            else { PendingSpan = 0; PendingStart = -1; Phase = MissionLevels == 4 ? AdventurePhase.Retargeting : AdventurePhase.Choosing; }
            return true;
        }
        static readonly int[] targetLevels = {3,5,7,4,6};
        static readonly string[] fundingNames = {"FIELDWORK FUND", "COFFEE CONTINGENCY", "LIBRARY LATE FEES", "BANANA-FREE EQUIPMENT", "CLOISTER WIFI FUND", "RAIN RESEARCH GRANT"};
        public string FundingName => Searches==0 ? "ROUTE RESEARCH" : fundingNames[(Searches-1)%fundingNames.Length];
        public string NextFundingName => fundingNames[Searches%fundingNames.Length];
        public int NextTargetLevels => Searches == 0 ? OriginalLevels : targetLevels[(Searches-1)%targetLevels.Length];
        bool FindResearch(out int from,out int span)
        {
            // Prefer a composable four; otherwise record the highest newly explored pair.
            foreach(int width in new[]{4,2})for(int distance=Tree.Depth[StartNode];distance>=0;distance--)
            {
                int node=Tree.KthAncestor(StartNode,distance),middle=Tree.KthAncestor(node,width/2);
                if(!Knows(node,width)&&Knows(node,width/2)&&Knows(middle,width/2))
                {from=node;span=width;return true;}
            }
            from=-1;span=0;return false;
        }
        public bool BeginResearch()
        {
            int from,span;if(Phase!=AdventurePhase.Complete||!FindResearch(out from,out span))return false;
            PendingStart=from;PendingSpan=span;Researching=true;Phase=AdventurePhase.Weaving;return true;
        }
        public bool CancelResearch()
        {
            if(!Researching||Phase!=AdventurePhase.Weaving)return false;
            Researching=false;PendingStart=-1;PendingSpan=0;Phase=AdventurePhase.Complete;return true;
        }
        public int MinimumDecisions(int levels)
        {
            if(levels<0||levels>Tree.Depth[StartNode])return -1;
            var cost=new int[levels+1];for(int i=1;i<=levels;i++)cost[i]=levels+1;
            for(int i=0;i<levels;i++)foreach(int span in new[]{1,2,4})
                if(i+span<=levels&&(span==1||Knows(Tree.KthAncestor(StartNode,i),span)))cost[i+span]=Math.Min(cost[i+span],cost[i]+1);
            return cost[levels];
        }
        public bool NextTarget()
        {
            if (Phase != AdventurePhase.Retargeting && Phase != AdventurePhase.Complete) return false;
            RequiredLevels = NextTargetLevels; Goal = Tree.KthAncestor(StartNode, RequiredLevels);
            BestRouteDecisions=MinimumDecisions(RequiredLevels);
            Searches++; Node = StartNode; MissionLevels = MissionSelections = CompletedInSelection = Span = 0;
            QuizSolved=false;QuizFeedback="Optional. No grades, no banana-based penalties.";
            Destination = -1; EdgeProgress = 0; Phase = AdventurePhase.Choosing; return true;
        }
    }
}
