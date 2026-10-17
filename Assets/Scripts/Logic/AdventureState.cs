using System;
namespace BinaryLift
{
    public enum AdventurePhase { Arrival, Courtyard, Chase, Mapping, Choosing, Climbing, Clue, Complete }
    /// <summary>The original tree, traversed edge by edge by deliberate pull input.</summary>
    public sealed class AdventureState
    {
        public readonly TreeModel Tree;
        public readonly int StartNode, Goal, RequiredLevels;
        public AdventurePhase Phase { get; private set; } = AdventurePhase.Arrival;
        public int Node { get; private set; }
        public int Destination { get; private set; } = -1;
        public int Span { get; private set; }
        public int CompletedInSelection { get; private set; }
        public int TotalLevels { get; private set; }
        public int Selections { get; private set; }
        public float EdgeProgress { get; private set; }
        public float TotalPull { get; private set; }
        public float Progress => Span==0 ? 0 : (CompletedInSelection+EdgeProgress)/Span;
        public int Remaining => RequiredLevels-TotalLevels;
        public const float PullPerEdge=.65f;
        public AdventureState(int seed=13)
        {
            Tree=TreeGenerator.Generate(seed,16,18);
            var original=RaceState.Create(Tree,seed);
            StartNode=Node=original.Start;Goal=original.Target;RequiredLevels=original.K;
        }
        public bool ExitClassroom(){if(Phase!=AdventurePhase.Arrival)return false;Phase=AdventurePhase.Courtyard;return true;}
        public bool BeginChase(){if(Phase!=AdventurePhase.Courtyard)return false;Phase=AdventurePhase.Chase;return true;}
        public bool ReachGate(){if(Phase!=AdventurePhase.Chase)return false;Phase=AdventurePhase.Mapping;return true;}
        public bool EnterTree(){if(Phase!=AdventurePhase.Mapping)return false;Phase=AdventurePhase.Choosing;return true;}
        public bool CanChoose(int span)=>Phase==AdventurePhase.Choosing&&(span==1||span==2||span==4||span==8)&&span<=Remaining;
        public bool Choose(int span)
        {
            if(!CanChoose(span))return false;
            Span=span;Destination=Tree.KthAncestor(Node,span);CompletedInSelection=0;EdgeProgress=0;
            Selections++;Phase=AdventurePhase.Climbing;return true;
        }
        public bool Pull(float metres)
        {
            if(Phase!=AdventurePhase.Climbing||float.IsNaN(metres)||float.IsInfinity(metres)||metres<=0||metres>.2f)return false;
            while(metres>0&&Phase==AdventurePhase.Climbing)
            {
                float accepted=Math.Min(metres,(1-EdgeProgress)*PullPerEdge);
                TotalPull+=accepted;metres-=accepted;EdgeProgress+=accepted/PullPerEdge;
                if(EdgeProgress>=.99999f)
                {
                    Node=Tree.Parent[Node];TotalLevels++;CompletedInSelection++;EdgeProgress=0;
                    if(CompletedInSelection==Span)Phase=Node==Goal?AdventurePhase.Clue:AdventurePhase.Choosing;
                }
            }
            return true;
        }
        public bool CollectCone(){if(Phase!=AdventurePhase.Clue)return false;Phase=AdventurePhase.Complete;return true;}
    }
}
