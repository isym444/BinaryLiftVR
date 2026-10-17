namespace BinaryLift
{
    public static class TreeLayout
    {
        /// <summary>
        /// Horizontal position per node: leaves are spaced one slot apart in DFS order,
        /// internal nodes sit at the mean of their children. Two nodes on the same
        /// level are therefore always at least <paramref name="spacing"/> apart.
        /// </summary>
        public static float[] ComputeX(TreeModel tree, float spacing)
        {
            var x = new float[tree.Count];
            int nextLeaf = 0;
            Place(tree, 0, x, ref nextLeaf, spacing);
            return x;
        }

        static void Place(TreeModel tree, int v, float[] x, ref int nextLeaf, float spacing)
        {
            var kids = tree.Children[v];
            if (kids.Count == 0)
            {
                x[v] = nextLeaf * spacing;
                nextLeaf++;
                return;
            }

            float sum = 0f;
            foreach (int c in kids)
            {
                Place(tree, c, x, ref nextLeaf, spacing);
                sum += x[c];
            }
            x[v] = sum / kids.Count;
        }
    }
}
