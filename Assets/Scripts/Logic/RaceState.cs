using System;
using System.Collections.Generic;

namespace BinaryLift
{
    public enum RacePhase { Countdown, Running, Won, Lost }

    /// <summary>
    /// Pure game rules: player vs NPC racing to the K-th ancestor of Start.
    /// The player may step to an adjacent node or take a power-of-two portal;
    /// the NPC only ever steps to its parent.
    /// </summary>
    public sealed class RaceState
    {
        public readonly TreeModel Tree;
        public readonly int Start;
        public readonly int K;
        public readonly int Target;

        public int Player { get; private set; }
        public int Npc { get; private set; }
        public int Moves { get; private set; }
        public int NpcSteps { get; private set; }
        public RacePhase Phase { get; private set; } = RacePhase.Countdown;

        RaceState(TreeModel tree, int start, int k)
        {
            Tree = tree;
            Start = start;
            K = k;
            Target = tree.KthAncestor(start, k);
            Player = start;
            Npc = start;
        }

        /// <summary>Picks a deep start node and a k whose binary form has at least two set bits.</summary>
        public static RaceState Create(TreeModel tree, int seed)
        {
            var rng = new Random(unchecked(seed * 7919 + 13));

            int minDepth = Math.Max(1, tree.MaxDepth - 3);
            var candidates = new List<int>();
            for (int v = 0; v < tree.Count; v++)
                if (tree.Depth[v] >= minDepth) candidates.Add(v);
            int start = candidates[rng.Next(candidates.Count)];

            int maxK = tree.Depth[start];
            var ks = new List<int>();
            for (int k = Math.Min(5, maxK); k <= maxK; k++)
                if (TreeModel.PopCount(k) >= 2) ks.Add(k);
            int chosen = ks.Count > 0 ? ks[rng.Next(ks.Count)] : maxK;

            return new RaceState(tree, start, chosen);
        }

        public void Begin()
        {
            if (Phase == RacePhase.Countdown) Phase = RacePhase.Running;
        }

        /// <summary>Walk to the parent or to a direct child of the current node.</summary>
        public bool TryStepTo(int node)
        {
            if (Phase != RacePhase.Running) return false;
            if (node < 0 || node >= Tree.Count || node == Player) return false;
            if (Tree.Parent[Player] != node && Tree.Parent[node] != Player) return false;
            MovePlayer(node);
            return true;
        }

        /// <summary>Teleport to the 2^j-th ancestor of the current node.</summary>
        public bool TryPortal(int j)
        {
            if (Phase != RacePhase.Running) return false;
            if (j < 0 || j >= Tree.Bits) return false;
            int dest = Tree.Up[j][Player];
            if (dest < 0) return false;
            MovePlayer(dest);
            return true;
        }

        public bool StepNpc()
        {
            if (Phase != RacePhase.Running) return false;
            Npc = Tree.Parent[Npc];
            NpcSteps++;
            if (Npc == Target) Phase = RacePhase.Lost;
            return true;
        }

        void MovePlayer(int node)
        {
            Player = node;
            Moves++;
            if (Player == Target) Phase = RacePhase.Won;
        }
    }
}
