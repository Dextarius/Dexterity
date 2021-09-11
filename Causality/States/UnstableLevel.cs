using System;
using System.Collections.Generic;
using Core.Causality;

namespace Causality.States
{
    public class UnstableLevel
    {
        private HashSet<IOutcome> unstableOutcomes = new HashSet<IOutcome>();
        private int               nextIndex;

        public  int Depth { get; }
    

        public void AddOutcome(IOutcome outcome)
        {
            unstableOutcomes.Add(outcome);
        }

        public bool RemoveOutcome(IOutcome outcome) => unstableOutcomes.Remove(outcome);

        private void ExpandArray()
        {
            var oldArray = unstableOutcomes;
            var newArray = new IOutcome[oldArray.Length * 2];
        
            Array.Copy(oldArray, newArray, oldArray.Length);
            unstableOutcomes = newArray;
        }

        public void RecalculateOutcomes()
        {
            for (int i = 0; i < nextIndex; i++)
            {
                unstableOutcomes[i].Recalculate();
            }
        }

        public void Reset()
        {
            Array.Clear(unstableOutcomes, 0, nextIndex - 1);
            nextIndex = 0;
        }

        public UnstableLevel(int depth)
        {
            Depth = depth;
        }
    }
}