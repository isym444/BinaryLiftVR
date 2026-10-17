using System;
using System.Collections.Generic;

namespace BinaryLift
{
    public static class TreeGenerator
    {
        /// <summary>
        /// A long trunk (guarantees depth) with random side branches hanging off it.
        /// Side branches stay shallower than the trunk end so the deepest node is the trunk tip.
        /// </summary>
        public static TreeModel Generate(int seed, int trunkLength, int extraNodes)
        {
            if (trunkLength < 2) throw new ArgumentException("trunkLength must be >= 2");

            var rng = new Random(seed);
            var parent = new List<int> { -1 };
            var depth = new List<int> { 0 };

            for (int i = 1; i <= trunkLength; i++)
            {
                parent.Add(i - 1);
                depth.Add(i);
            }

            for (int e = 0; e < extraNodes; e++)
            {
                int p;
                do { p = rng.Next(parent.Count); } while (depth[p] > trunkLength - 2);
                parent.Add(p);
                depth.Add(depth[p] + 1);
            }

            return new TreeModel(parent.ToArray());
        }
    }
}
