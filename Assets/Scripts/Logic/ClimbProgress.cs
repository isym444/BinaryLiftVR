using System.Collections.Generic;

namespace BinaryLift
{
    // Physical forks define the tree. Rootward shortcuts never cross sibling branches.
    public sealed class ClimbProgress
    {
        public readonly TreeModel Tree = new TreeModel(new[] {-1,0,1,2,3,4,5,6,3,8,5,10,7});
        readonly HashSet<int> collected = new HashSet<int>();
        public int Cones => collected.Count;
        public int Reach => 1 << System.Math.Min(Cones, 3);
        public bool Collect(int node)
        {
            if (node != 3 && node != 7 && node != 9 && node != 11) return false;
            return collected.Add(node);
        }
        public bool Collected(int node) => collected.Contains(node);
        public int Ancestor(int node)
        {
            if (node <= 0 || node >= Tree.Count) return -1;
            int steps = Reach;
            while (steps > Tree.Depth[node]) steps /= 2;
            return Tree.KthAncestor(node, steps);
        }
    }
}
