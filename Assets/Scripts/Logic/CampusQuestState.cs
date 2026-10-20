using System;
using System.Collections.Generic;

namespace BinaryLift
{
    public enum QuestPhase { Briefing, Exploring, CarryingCone, Complete }

    /// <summary>Deterministic tutorial progression; no Unity dependency.</summary>
    public sealed class CampusQuestState
    {
        public readonly TreeModel Tree;
        public readonly int[] ClueNodes = { 19, 17, 13, 0 };
        public QuestPhase Phase { get; private set; } = QuestPhase.Briefing;
        public int Node { get; private set; } = 20;
        public int Cones { get; private set; }
        public int Moves { get; private set; }
        public int LastJump { get; private set; }
        public int MaxJump => 1 << Math.Min(Cones, 3);
        public int Goal => ClueNodes[Math.Min(Cones, ClueNodes.Length - 1)];
        public int Remaining => Tree.Depth[Node] - Tree.Depth[Goal];

        public CampusQuestState()
        {
            // The main route is an ancestor chain. Side branches make the route map a tree.
            var parents = new int[27];
            parents[0] = -1;
            for (int i = 1; i <= 20; i++) parents[i] = i - 1;
            for (int i = 21; i < parents.Length; i++) parents[i] = (i - 21) * 3 + 2;
            Tree = new TreeModel(parents);
        }

        public bool Begin()
        {
            if (Phase != QuestPhase.Briefing) return false;
            Phase = QuestPhase.Exploring;
            return true;
        }

        public bool CanJump(int levels)
        {
            return Phase == QuestPhase.Exploring && levels > 0 &&
                (levels & (levels - 1)) == 0 && levels <= MaxJump && levels <= Remaining;
        }

        public bool TryJump(int levels)
        {
            if (!CanJump(levels)) return false;
            Node = Tree.KthAncestor(Node, levels);
            LastJump = levels;
            Moves++;
            return true;
        }

        public bool PickUpCone()
        {
            if (Phase != QuestPhase.Exploring || Node != Goal) return false;
            Phase = QuestPhase.CarryingCone;
            return true;
        }

        public bool ReturnCone()
        {
            if (Phase != QuestPhase.CarryingCone) return false;
            Phase = QuestPhase.Exploring;
            return true;
        }

        public bool DepositCone()
        {
            if (Phase != QuestPhase.CarryingCone) return false;
            Cones++;
            Phase = Cones == ClueNodes.Length ? QuestPhase.Complete : QuestPhase.Exploring;
            return true;
        }

        public List<int> Preview(int levels)
        {
            var route = new List<int>();
            if (!CanJump(levels)) return route;
            int node = Node;
            route.Add(node);
            for (int i = 0; i < levels; i++) { node = Tree.Parent[node]; route.Add(node); }
            return route;
        }

        public List<int> BreadthFirstOrder()
        {
            var order = new List<int>();
            var queue = new Queue<int>();
            queue.Enqueue(0);
            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                order.Add(node);
                foreach (int child in Tree.Children[node]) queue.Enqueue(child);
            }
            return order;
        }
    }
}
