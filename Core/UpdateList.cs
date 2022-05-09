using System;
using System.Diagnostics;
using Core.Causality;
using Core.States;

namespace Core
{
    public class UpdateList : IUpdateQueue
    {
        #region Constants

        private const int numberOfLevelsToStartWith = 8;

        #endregion
        
        #region Instance Fields
        
        private PriorityLevel[]  priorityLevels                    = new PriorityLevel[numberOfLevelsToStartWith];
        private int              lowestCreatedPriority             = -1;
        private int              lowestPriorityContainingElements  = -1;
        private int              highestPriorityContainingElements = int.MaxValue;
        private int              numberOfQueuedUpdates;
        private bool             updateInProgress;
        private bool             updatesAreBeingQueued;
        private UpdateQueueToken updateQueueToken;

        //- We could use UpdatePriority 0 for elements that have no priority and should always update last.
        //  Although that may be counterintuitive.  
        
        #endregion


        #region Properties

        public bool IsUpdating => updateInProgress;

        #endregion


        #region Instance Methods

        public UpdateQueueToken QueueUpdates()
        {
            if (updatesAreBeingQueued is false)
            {
                updatesAreBeingQueued = true;

                return updateQueueToken;
            }

            return default(UpdateQueueToken);
        }

        protected void StopQueueingUpdates()
        {
            if (updatesAreBeingQueued)
            {
                updatesAreBeingQueued = false;
            }
            else
            {
                throw new InvalidOperationException("Updates were not being queued");
            }
        }
        
        public void Update(IUpdateable updateable, int priority)
        {
            if (updatesAreBeingQueued)
            {
                AddUpdate(updateable, priority);
            }
            else
            {
                updateable.Update();
            }
        }
        
        public void Update<TUpdate>(TUpdate updateable)  where TUpdate : IUpdateable, IPrioritizable
        {
            Update(updateable, updateable.UpdatePriority);
        }
        
        protected void AddUpdate(IUpdateable objectToAdd, int priority)
        {
            
            if (priority > lowestCreatedPriority)
            {
                CreateLevelForPriority(priority);
            }
            
            //- Test to make sure we don't overwrite any levels.

            if (priority < highestPriorityContainingElements)
            {
                highestPriorityContainingElements = priority;
            }
            
            if (priority > lowestPriorityContainingElements)
            {
                lowestPriorityContainingElements = priority;
            }

            priorityLevels[priority].AddUpdate(objectToAdd);
            numberOfQueuedUpdates++;
            //- Add a log statement if an update triggers another update with a lower priority to be added.
        }
        
        
        //- Currently once this implementation starts updating a given priority level, it will finish updating all
        //  of the elements in that priority before returning to this method.  This means that if an element's 
        //  update causes an element with a higher priority to be queued, that higher priority element won't 
        //  be updated until the current priority level finishes.  Revisit this later and decide if we want to 
        //  pause the lower priority level's updates and run the higher priority one.  We could also just
        //  make it so that if we are running updates and an update with a priority higher than the one we're
        //  working on comes in, we immediately execute that update.   
        protected void RunUpdates()
        {
            if (updateInProgress is false  &&  numberOfQueuedUpdates > 0)
            {
                updateInProgress = true;

                try
                {
                    var levelIndex = highestPriorityContainingElements;
                    
                    while (levelIndex <= lowestPriorityContainingElements)
                    {
                        highestPriorityContainingElements++;

                        var currentLevel = priorityLevels[levelIndex];
                
                        if (currentLevel.Count > 0)
                        {
                            int numberOfUpdatesInLevel = currentLevel.RunUpdates();
                            
                            numberOfQueuedUpdates -= numberOfUpdatesInLevel;
                        }

                        levelIndex = highestPriorityContainingElements;
                        //- This means we'll go back to a higher priority level if an update is
                        //  added while we're running the ones for this level.
                    }

                    highestPriorityContainingElements = int.MaxValue;
                    lowestPriorityContainingElements  = -1;
                    
                    Debug.Assert(numberOfQueuedUpdates == 0);
                }
                finally
                {
                    updateInProgress = false;
                }
            }
            
            //- Can we add a mechanic where if an updating Reactive wants a value that's 'blocked', part of whatever
            //  is blocking it gets added to our queue to process at a higher priority?
        }

        private void CreateLevelForPriority(int priority)
        {
            //- Make a Debug that checks to make sure none of the
            //  from lowestCreatedPriority - priority have levels in them.
            
            if (priority <= lowestCreatedPriority)
            {
                throw new InvalidOperationException("UpdatePriority already exists. ");
            }

            for (int i = lowestCreatedPriority + 1; i <= priority; ++i)
            {
                Tools.Collections.Add(ref priorityLevels, new PriorityLevel(i), i);
            }

            lowestCreatedPriority = priority;
        }

        #endregion


        #region Constructors

        public UpdateList()
        {
            updateQueueToken = new UpdateQueueToken(true, this);
            CreateLevelForPriority(numberOfLevelsToStartWith - 1); 
        }

        #endregion

        void IUpdateQueue.RunUpdates()   => RunUpdates();
        void       IQueue.StopQueuing()  => StopQueueingUpdates();
        void       IQueue.StartQueuing() => QueueUpdates();
    }
}