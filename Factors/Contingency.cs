using System;
using System.Collections.Generic;
using Causality;
using Causality.Processes;
using Core.Causality;
using Core.Factors;

namespace Factors
{
    //- Consider if we want to make this class contain a Reactive, as opposed to inheriting one.  Think if there are any members
    //  which might lead people to make mistakes (such as someone setting IsImpulsive to true and then IsReflexive to false,
    //  and then wondering why the command never executes).
    public class Contingency : Reactive<bool>
    {
        #region Instance Fields

      //private readonly Reactive<bool> conditionsMet;
        private readonly IProcess       command;
        private          bool           isImpulsive;

        #endregion

        
        #region Properties

        //public bool ConditionsMet => conditionsMet;
        
        
        /// <summary>
        ///     If true, this instance will monitor changes in the condition provided, and the governed action
        ///     will automatically execute every time the condition switches from false to true.
        /// </summary>
        public bool IsImpulsive
        {
            get => isImpulsive;
            set
            {
                if (value != isImpulsive)
                {
                    if (value) { AssumeImpulsiveStance(); }
                    else       {   DropImpulsiveStance(); }
                }
            }
        }
        
        //- TODO : We may need the option to specify the reactive value as Reflexive, without setting it to be Impulsive.
        
        #endregion
        
        
        #region Instance Methods

        protected override bool Act()
        {
            bool valueChanged = base.Act();
            bool newValue     = outcome.Peek();

            if (valueChanged  &&  
                IsImpulsive   &&
                newValue is true)
            {
                UpdateHandler.RequestUpdate(Execute);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AssumeImpulsiveStance()
        {
            isImpulsive = true;
            IsReflexive = true;
            //conditionsMet.SubscribeToUpdates(ExecuteIfTrue);
        }

        private void DropImpulsiveStance()
        {
            isImpulsive = false;
            IsReflexive = false;
            //conditionsMet.UnsubscribeFromUpdates(ExecuteIfTrue);        
        }
        
        public bool TryExecute()
        {
            bool shouldExecute = outcome.Value; //- in case the conditions change while we're executing this method.  
            
            ExecuteIfTrue(shouldExecute);
            
            return shouldExecute; 
        }

        protected void ExecuteIfTrue(bool shouldExecute) 
        {  
            if(shouldExecute) { Execute(); }
        }

        public    void ForceExecute() => Execute();
        protected void Execute()      => command?.Execute();

        //- Subscriptions => 
        
        #endregion
        
        
        
        public Contingency(IProcess<bool> processToDetermineValue, IProcess processToExecute, string name = null) : base(processToDetermineValue, name)
        {
            command = processToExecute??  throw new ArgumentNullException(nameof(processToExecute));
        }

        public Contingency(Func<bool> functionToDetermineValue, IProcess processToExecute, string name = null) : 
            this(new FunctionalProcess<bool>(functionToDetermineValue), processToExecute, name)
        {
            if (functionToDetermineValue is null)
            {
                throw new ArgumentNullException(nameof(functionToDetermineValue));
            }
        }
        
        public Contingency(IProcess<bool> processToDetermineValue, Action actionToExecute, string name = null) : 
            this(processToDetermineValue, new ActionProcess(actionToExecute), name)
        {
            if (actionToExecute is null)
            {
                throw new ArgumentNullException(nameof(processToDetermineValue));
            }
        }

        public Contingency(Func<bool> functionToDetermineValue, Action processToExecute, string name = null) : 
            this(functionToDetermineValue, new ActionProcess(processToExecute), name)
        {
            if (processToExecute is null)
            {
                throw new ArgumentNullException(nameof(processToExecute));
            }
        }
    }
    
    
        
    //- TODO : Make a fluent interface for users to easily create Contingencies
    public static class ContingencyPlanner {}
}