using System;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Cores;

namespace Factors.Modifiers
{
    public class Modifier<T> : Reactor<IModifierCore<T>>, IModifier<T>
    {
        public int ModPriority { get => core.ModPriority; 
                                 set => core.ModPriority = value; }

        public T Modify(T valueToModify) => core.Modify(valueToModify);
        
        public override bool CoresAreNotEqual(IModifierCore<T> oldCore, IModifierCore<T> newCore) => false;

        
        public Modifier(IModifierCore<T> reactorCore, string nameToGive = null) : base(reactorCore, nameToGive)
        {
        }
    }


    public abstract class RangeModifierCore<T> : ReactorCore, IModifierCore<T>
    {
        #region Constants

        protected const int DefaultPriority = 10;

        #endregion

        
        #region Instance Fields

        protected IFactor<T> minimum;
        protected IFactor<T> maximum;

        #endregion

        
        #region Properties

        protected override IEnumerable<IFactor> Triggers         { get { yield return minimum;  yield return maximum; } }
        public override    bool                 HasTriggers      => true;
        public override    int                  NumberOfTriggers => 2;
        public             int                  ModPriority      { get; set; } = DefaultPriority;

        #endregion

        protected override long CreateOutcome() => TriggerFlags.Default;

        public T Modify(T valueToModify)
        {
            if (IsLessThan(minimum.Value, valueToModify))
            {
                valueToModify = minimum.Value;
            }
            
            if (IsGreaterThan(maximum.Value, valueToModify))
            {
                valueToModify = maximum.Value;
            }

            return valueToModify;
        }

        protected abstract bool IsGreaterThan(T baseValue, T valueToCheck);
        protected abstract bool    IsLessThan(T baseValue, T valueToCheck);

        protected RangeModifierCore(IFactor<T> minimumValueFactor, IFactor<T> maximumValueFactor)
        {
            IsTriggered = false;
            minimum     = minimumValueFactor;
            maximum     = maximumValueFactor;
            AddTrigger(minimumValueFactor, IsReflexive);
            AddTrigger(maximumValueFactor, IsReflexive);
        }
    }
}