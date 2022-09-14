using System;
using System.Collections.Generic;
using System.Linq;
using Core.Factors;

namespace Factors.Cores
{
    public abstract class ModifierListCore<T> : ReactorCore
    {
        #region Static Fields

        private static readonly IModTypeOrder defaultModTypeOrder = new DefaultModTypeOrder();

        #endregion
        
        
        #region Instance Fields
        
        private readonly IModTypeOrder              modTypeOrder;
        private          Aggregator<INumericMod<T>> constantValue;
        private          Aggregator<INumericMod<T>> minimumValue;
        private          Aggregator<INumericMod<T>> maximumValue;
        private          Aggregator<T>              flatAdded;
        private          Aggregator<double>         additiveMultiplier;
        private          Aggregator<double>         multiplicativeMultiplier;

        #endregion


        #region Properties

        public             T                    FlatAdded                => (flatAdded is null)? default : flatAdded.Value;
        public             double               AdditiveMultiplier       => additiveMultiplier?.Value ?? 1;
        public             double               MultiplicativeMultiplier => multiplicativeMultiplier?.Value ?? 1;
        public             INumericMod<T>       MinimumValue             => minimumValue?.Value;
        public             INumericMod<T>       MaximumValue             => maximumValue?.Value;
        public             INumericMod<T>       ConstantValue            => constantValue?.Value;
        protected override IEnumerable<IFactor> Triggers                 => default; // modifiers;
        public override    bool                 HasTriggers              => true;    // modifiers.Count > 0;
        public override    int                  NumberOfTriggers         => 1;       // modifiers.Count;
        
        //- TODO : Fix the trigger related properties. 

        //^ We'll have to revisit the ConstantValue mods later.  Either they can't be INumericMods, or
        //  we'll have to require inheritors to implement a method that converts a double into a T, or
        //  we'll have to change INumericMod to INumericMod<T> and add a Modify<T> where we pass the
        //  current value to the mod and it returns a modified version.

        #endregion
        

        #region Instance Methods

        protected override long CreateOutcome() => TriggerFlags.Default;


        public void AddFlatModifier(INumericMod<T> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            if (flatAdded is null)
            {
               // flatAdded = new Aggregator<T>();
                AddTrigger(flatAdded, IsReflexive);
            }


            flatAdded.Include(modifierToAdd);
            //- Include() should trigger us if the mod changes anything right?
        }
        
        public void AddAdditiveModifier(INumericMod<double> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            if (additiveMultiplier is null)
            {
             //   additiveMultiplier = new Aggregator<double>();
                AddTrigger(additiveMultiplier, IsReflexive);
            }

            additiveMultiplier.Include(modifierToAdd);
        }
        
        public void AddMultiplicativeModifier(INumericMod<double> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            if (multiplicativeMultiplier is null)
            {
              //  multiplicativeMultiplier = new Aggregator<double>();
                AddTrigger(multiplicativeMultiplier, IsReflexive);
            }


            multiplicativeMultiplier.Include(modifierToAdd);
        }
        
        public void AddMaximumValueModifier(IFactor<INumericMod<T>> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            if (maximumValue is null)
            {
              //  maximumValue = new Aggregator<INumericMod<T>>();
                AddTrigger(maximumValue, IsReflexive);
            }

            maximumValue.Include(modifierToAdd);
        }
        
        public void AddMinimumValueModifier(IFactor<INumericMod<T>> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            if (minimumValue is null)
            {
              //  minimumValue = new Aggregator<INumericMod<T>>();
                AddTrigger(minimumValue, IsReflexive);
            }

            minimumValue.Include(modifierToAdd);
        }
        
        public void AddConstantValueModifier(IFactor<INumericMod<T>> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            if (constantValue is null)
            {
               // constantValue = new Aggregator<INumericMod<T>>();
                AddTrigger(constantValue, IsReflexive);
            }

            constantValue.Include(modifierToAdd);
        }
        
        
        public void RemoveFlatModifier(INumericMod<T> modifierToRemove)
        {
            if (modifierToRemove is null) { throw new ArgumentNullException(nameof(modifierToRemove)); }

            flatAdded?.Remove(modifierToRemove);
        }
        
        public void RemoveAdditiveModifier(INumericMod<double> modifierToRemove)
        {
            if (modifierToRemove is null) { throw new ArgumentNullException(nameof(modifierToRemove)); }

            additiveMultiplier?.Remove(modifierToRemove);
        }
        
        public void RemoveMultiplicativeModifier(INumericMod<double> modifierToRemove)
        {
            if (modifierToRemove is null) { throw new ArgumentNullException(nameof(modifierToRemove)); }
            
            multiplicativeMultiplier?.Remove(modifierToRemove);
        }
        
        public void RemoveMaximumValueModifier(IFactor<INumericMod<T>> modifierToRemove)
        {
            if (modifierToRemove is null) { throw new ArgumentNullException(nameof(modifierToRemove)); }

            maximumValue?.Remove(modifierToRemove);
        }
        
        public void RemoveMinimumValueModifier(IFactor<INumericMod<T>> modifierToRemove)
        {
            if (modifierToRemove is null) { throw new ArgumentNullException(nameof(modifierToRemove)); }

            minimumValue?.Remove(modifierToRemove);
        }
        
        public void RemoveConstantValueModifier(IFactor<INumericMod<T>> modifierToRemove)
        {
            if (modifierToRemove is null) { throw new ArgumentNullException(nameof(modifierToRemove)); }

            constantValue?.Remove(modifierToRemove);
        }

        public T Modify(T valueToModify)
        {
            T result = valueToModify;
            
            foreach (var modType in modTypeOrder.ModTypesByPriority)
            {
                switch (modType)
                {
                    case NumericModType.Multiplicative: { result = Multiply(result, MultiplicativeMultiplier); break; }
                    case NumericModType.Additive:       { result = Multiply(result, AdditiveMultiplier);       break; }
                    case NumericModType.Flat:           { result =      Add(result, FlatAdded);                break; }
                    case NumericModType.Minimum:
                    {
                        if (minimumValue?.Value != null)
                        {
                            result = ApplyMinimum(result, minimumValue.Value.Value);
                        }
                        
                        break;
                    }
                    case NumericModType.Maximum:
                    {
                        if (maximumValue?.Value != null)
                        {
                            result = ApplyMaximum(result, maximumValue.Value.Value);
                        }
                        
                        break;
                    }
                    case NumericModType.ConstantValue:
                    {
                        if (constantValue?.Value != null)
                        {
                            result = constantValue.Value.Value;
                        }
                        
                        break;
                    }
                    default: { throw new InvalidOperationException($"Unhandled case {modType} in {nameof(Modify)}(). "); }
                }
            }

            return result;
        }

        protected override void InvalidateOutcome(IFactor changedFactor) { }
        
        protected abstract T    ApplyMaximum(T valueToModify, T maximumValue);
        protected abstract T    ApplyMinimum(T valueToModify, T minimumValue);
        protected abstract T    Multiply(T valueToModify, double multiplier);
        protected abstract T    Add(T valueToModify, T amountToAdd);
        protected abstract bool ValuesAreDifferent(T first, T second);
        

        #endregion


        #region Constructors

        protected ModifierListCore(IModTypeOrder modOrder)
        {
            modTypeOrder = modOrder;
        }

        protected ModifierListCore() : this(defaultModTypeOrder)
        {
            
        }

        #endregion

    }
    
}