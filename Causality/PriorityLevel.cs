using System;
using System.Collections.Generic;
using Core.Causality;
using Core.States;
using Core.Tools;

namespace Causality.States
{
    public class PriorityLevel
    {
        #region Constants

        private const int initialLevelSize = 4;

        #endregion
        
        #region Instance Fields

        private IUpdateable[] queuedElements = new IUpdateable[initialLevelSize];
        private int           count;

        #endregion

        
        #region Properties

        public int Priority { get; }
        public int Count => count;

        #endregion

        
        #region Instance Methods

        public void AddUpdate(IUpdateable objectToAdd)
        {
            Collections.Add(ref queuedElements, objectToAdd, count);
            count++;
        }

        public int RunUpdates()
        {
            int index;
                
            for (index = 0; index < Count; index++)
            {
                ref IUpdateable currentSlot = ref queuedElements[index];
                
                currentSlot.Update();
                currentSlot = null;
            }
            
            count = 0;

            return index;
            
            //- Make sure if someone tries to add an element while we are iterating over the array
            //  and that causes it to expand, that we don't keep using the unexpanded array.
        }

        #endregion



        #region Constructors

        public PriorityLevel(int priority)
        {
            Priority = priority;
        }

        #endregion
    }
}