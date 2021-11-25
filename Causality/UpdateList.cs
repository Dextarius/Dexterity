﻿using System;
using System.Diagnostics;
using Causality.States;
using Core.Causality;
using Core.States;
using Core.Tools;

namespace Causality
{
    public partial class UpdateList : IUpdateQueue
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

        //- We could use Priority 0 for elements that have no priority and should always update last.
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
            Update(updateable, updateable.Priority);
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
        
        
        // protected void RunUpdates()
        // {
        //     if (updateInProgress is false  &&  numberOfQueuedUpdates > 0)
        //     {
        //         updateInProgress = true;
        //
        //         try
        //         {
        //             var levelToUpdate = highestPriorityContainingElements;
        //             
        //             levelBeingUpdated = highestPriorityContainingElements;
        //
        //             while (levelBeingUpdated <= lowestPriorityContainingElements)
        //             {
        //                 highestPriorityContainingElements++;
        //                 //- This means we'll go back to a higher priority level if an update is
        //                 //  added while we're running the ones for this level.
        //
        //                 var currentLevel = levels[levelBeingUpdated];
        //         
        //                 if (currentLevel.Count > 0)
        //                 {
        //                     int numberOfUpdatesInLevel = currentLevel.Count;
        //                     
        //                     currentLevel.RunUpdates();
        //                     numberOfQueuedUpdates -= numberOfUpdatesInLevel;
        //                 }
        //                 
        //                 levelBeingUpdated = highestPriorityContainingElements;
        //             }
        //
        //             highestPriorityContainingElements = int.MaxValue;
        //             lowestPriorityContainingElements  = -1;
        //             
        //             Debug.Assert(numberOfQueuedUpdates == 0);
        //         }
        //         finally
        //         {
        //             updateInProgress = false;
        //         }
        //     }
        //     
        //     //- Can we add a mechanic where if an updating Reactive wants a value that's 'blocked', part of whatever
        //     //  is blocking it gets added to our queue to process at a higher priority?
        // }

        private void CreateLevelForPriority(int priority)
        {
            //- Make a Debug that checks to make sure none of the
            //  from lowestCreatedPriority - priority have levels in them.
            
            if (priority <= lowestCreatedPriority)
            {
                throw new InvalidOperationException("Priority already exists. ");
            }

            for (int i = lowestCreatedPriority + 1; i <= priority; ++i)
            {
                Collections.Add(ref priorityLevels, new PriorityLevel(i), i);
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

        void IUpdateQueue.RunUpdates()  => RunUpdates();
        void IQueue.StopQueuing() => StopQueueingUpdates();
    }
}