using System;
using System.Collections.Generic;

namespace BinaryLift
{
    /// <summary>
    /// Rooted tree with a binary-lifting table. Node 0 is the root and every
    /// node's parent has a smaller index than the node itself.
    /// Up[j][v] is the 2^j-th ancestor of v, or -1 if it runs past the root.
    /// </summary>
    public sealed class TreeModel
    {
        public readonly int[] Parent;
        public readonly int[] Depth;
        public readonly int[][] Up;
        public readonly List<int>[] Children;
        public readonly int MaxDepth;

        public int Count => Parent.Length;
        public int Bits => Up.Length;

        public TreeModel(int[] parent)
        {
            if (parent == null || parent.Length == 0 || parent[0] != -1)
                throw new ArgumentException("node 0 must be the root (parent -1)");

            Parent = parent;
            Depth = new int[parent.Length];
            Children = new List<int>[parent.Length];
            for (int v = 0; v < parent.Length; v++) Children[v] = new List<int>();

            for (int v = 1; v < parent.Length; v++)
            {
                int p = parent[v];
                if (p < 0 || p >= v)
                    throw new ArgumentException("parent of node " + v + " must have a smaller index");
                Depth[v] = Depth[p] + 1;
                Children[p].Add(v);
                if (Depth[v] > MaxDepth) MaxDepth = Depth[v];
            }

            int bits = 1;
            while ((1 << bits) <= MaxDepth) bits++;

            Up = new int[bits][];
            Up[0] = (int[])parent.Clone();
            for (int j = 1; j < bits; j++)
            {
                Up[j] = new int[parent.Length];
                for (int v = 0; v < parent.Length; v++)
                {
                    int mid = Up[j - 1][v];
                    Up[j][v] = mid < 0 ? -1 : Up[j - 1][mid];
                }
            }
        }

        public int Jump(int v, int j) => Up[j][v];

        /// <summary>k-th ancestor by binary lifting, or -1 if it doesn't exist.</summary>
        public int KthAncestor(int v, int k)
        {
            if (k < 0 || k > Depth[v]) return -1;
            for (int j = 0; j < Bits && v >= 0; j++)
                if (((k >> j) & 1) != 0) v = Up[j][v];
            return v;
        }

        /// <summary>k-th ancestor by walking parent pointers one at a time (what the NPC does).</summary>
        public int KthAncestorNaive(int v, int k)
        {
            if (k < 0 || k > Depth[v]) return -1;
            for (int i = 0; i < k; i++) v = Parent[v];
            return v;
        }

        public static int PopCount(int k)
        {
            int c = 0;
            while (k != 0) { c += k & 1; k >>= 1; }
            return c;
        }

        public static List<int> SetBits(int k)
        {
            var bits = new List<int>();
            for (int j = 0; (k >> j) != 0; j++)
                if (((k >> j) & 1) != 0) bits.Add(j);
            return bits;
        }
    }
}
