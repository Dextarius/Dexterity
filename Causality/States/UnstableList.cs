using System;
using Core.Causality;

namespace Causality.States
{
    public class UnstableList
    {
        private UnstableLevel[] levels;
        private int lowestCreatedDepth;

        public static UnstableList Default { get; } = new UnstableList();
    
    
        public void AddOutcome(IOutcome outcome, int depth)
        {
            if (depth > lowestCreatedDepth)
            {
                CreateDepth(depth);
            }
        
            levels[depth].AddOutcome(outcome);
        }

        public bool MoveOutcome(IOutcome outcome, int previousDepth, int newDepth)
        {
            bool removedPreviousEntry = false;
        
            if (previousDepth < lowestCreatedDepth)
            {
                removedPreviousEntry = levels[previousDepth].RemoveOutcome(outcome);
            }
        
            if (newDepth > lowestCreatedDepth)
            {
                CreateDepth(newDepth);
            }
        
            levels[newDepth].AddOutcome(outcome);

            return removedPreviousEntry;
        }

        private void EnsureDepthExists(int depth)
        {
        
        }

        private void CreateDepth(int depth)
        {
            if (depth >= levels.Length)
            {
                ExpandLevels(depth);
            }
        
            for (int i = lowestCreatedDepth; i <= depth; ++i)
            {
                levels[i] = new UnstableLevel(i);
            }

            lowestCreatedDepth = depth;
        }

        private void ExpandLevels(int minSize)
        {
            UnstableLevel[] oldArray = levels;
            int             newSize  = oldArray.Length;
            UnstableLevel[] newArray;

            if (newSize < minSize)
            {
                newSize = minSize;
            }
        
            newArray = new UnstableLevel[newSize];
        
            Array.Copy(oldArray, newArray, oldArray.Length);
            levels = newArray;
        }
    }
}