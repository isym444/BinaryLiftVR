using System;
using System.Collections.Generic;
namespace BinaryLift
{
    public enum AdventurePhase { Arrival, Courtyard, Chase, Mapping, Choosing, Climbing, Clue, Weaving, Retargeting, Complete }
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
            if (Searches > 0) { Phase = AdventurePhase.Complete; return true; }
            PendingSpan = 2; PendingStart = Tree.KthAncestor(StartNode, MissionLevels - 2);
            Phase = AdventurePhase.Weaving; return true;
        }
        public bool Weave()
        {
            if (Phase != AdventurePhase.Weaving || (PendingSpan != 2 && PendingSpan != 4) ||
                !Knows(PendingStart, PendingSpan / 2) || !Knows(PendingMiddle, PendingSpan / 2) ||
                Knows(PendingStart, PendingSpan)) return false;
            connections.Add(PendingStart * 8 + PendingSpan);
            if (PendingSpan == 2 && MissionLevels == 4) { PendingSpan = 4; PendingStart = StartNode; }
            else { PendingSpan = 0; PendingStart = -1; Phase = MissionLevels == 4 ? AdventurePhase.Retargeting : AdventurePhase.Choosing; }
            return true;
        }
        public int NextTargetLevels => Searches == 0 ? OriginalLevels : (RequiredLevels == 5 ? 6 : 5);
        public bool NextTarget()
        {
            if (Phase != AdventurePhase.Retargeting && Phase != AdventurePhase.Complete) return false;
            RequiredLevels = NextTargetLevels; Goal = Tree.KthAncestor(StartNode, RequiredLevels);
            Searches++; Node = StartNode; MissionLevels = MissionSelections = CompletedInSelection = Span = 0;
            Destination = -1; EdgeProgress = 0; Phase = AdventurePhase.Choosing; return true;
        }
    }
}
