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

        Console.WriteLine(failures == 0 ? "ALL PASSED" : failures + " FAILURE(S)");
        return failures == 0 ? 0 : 1;
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
}
