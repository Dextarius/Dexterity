using System;
using System.Collections.Generic;
using Core.Factors;
using Factors.Cores.DirectReactorCores;
using static Core.Tools.Numerics;

namespace Factors.Cores
{
    //- We could reverse this setup and have the Modifiers keep a list of Modifiables
    //  and when you add one the Modifier manually adds/subtracts from the Modifiable's
    // multipliers. 
    public abstract class ValueControllerModCore<T> : ReactorCore, IValueControllerCore<T>
    {
        #region Static Fields

        private static readonly IModTypeOrder defaultModTypeOrder = new DefaultModTypeOrder();

        #endregion
        
        
        #region Instance Fields
        
        private readonly IModTypeOrder              modTypeOrder;
        private          T                          modifiedValue;
        private          T                          baseValue;
        private          Aggregator<INumericMod<T>> constantValue;
        private          Aggregator<T>              flatAdded;
        private          Aggregator<double>         additiveMultiplier;
        private          Aggregator<double>         multiplicativeMultiplier;
        private          Aggregator<INumericMod<T>> maximumValue;
        private          Aggregator<INumericMod<T>> minimumValue;

        #endregion


        #region Properties

        public             T                    FlatAdded                => (flatAdded is null)? default : flatAdded.Value;
        public             double               AdditiveMultiplier       => additiveMultiplier?.Value ?? 1;
        public             double               MultiplicativeMultiplier => multiplicativeMultiplier?.Value ?? 1;
        public             INumericMod<T>       ConstantValue            => constantValue?.Value;
        protected override IEnumerable<IFactor> Triggers                 => default; // modifiers;
        public override    bool                 HasTriggers              => true;    // modifiers.Count > 0;
        public override    int                  NumberOfTriggers         => 1;       // modifiers.Count;
        
        public T Value
        {
            get
            {
                AttemptReaction();
                return modifiedValue;
            }
        }
        
        //- public bool AllowOverflow { get; set;}
        
        //- TODO : Fix the trigger related properties. 

        //^ We'll have to revisit the ConstantValue mods later.  Either they can't be INumericMods, or
        //  we'll have to require inheritors to implement a method that converts a double into a T, or
        //  we'll have to change INumericMod to INumericMod<T> and add a Modify<T> where we pass the
        //  current value to the mod and it returns a modified version.
        
        public T BaseValue
        {
            get => baseValue;
            set
            {
                if (ValuesAreDifferent(baseValue, value, out _))
                {
                    baseValue = value;
                    Trigger();
                }
            }
        }

        #endregion
        

        #region Instance Methods

        protected override long CreateOutcome()
        {
           // RecalculateModifiers();
           
           T oldModifiedValue = modifiedValue;
           T newModifiedValue = Modify(baseValue);

            if (ValuesAreDifferent(oldModifiedValue, newModifiedValue, out var triggerFlags))
            {
                modifiedValue = newModifiedValue;
            }
            
            return triggerFlags;
        }

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
        
        // protected int FindIndexForModType(NumericModType modType)
        // {
        //     int priorityForModType = modTypeOrder.GetPriorityForModType(modType);
        //     
        //     for (int i = 0; i < modifiers.Count; i++)
        //     {
        //         var currentModsType     = modifiers[i].ModType;
        //         int currentModsPriority = modTypeOrder.GetPriorityForModType(currentModsType);
        //         
        //         if (currentModsPriority > priorityForModType)
        //         {
        //             return i;
        //         }
        //     }
        //
        //     return modifiers.Count;
        // }
        
        // public void RemoveModifier(INumericMod modifierToRemove)
        // {
        //     if (modifierToRemove != null && 
        //         modifiers.Remove(modifierToRemove))
        //     {
        //         RemoveTrigger(modifierToRemove);
        //         Trigger();
        //         // Ideally we'd be able to just check the mod's type and remove its amount from the correct 
        //         // property, but it seems impossible to guarantee that the mod's Amount or type hasn't changed
        //         // since we last used it.
        //     }
        // }

       // public bool ContainsModifier(INumericMod modifierToFind) => modifiers.Contains(modifierToFind);

        // protected void RecalculateModifiers()
        // {
        //     double      multiplicativeMultiplier = 1;
        //     double      additiveMultiplier       = 1;
        //     T           flatAmount               = default;
        //     INumericMod minimumValueMod          = null;
        //     INumericMod maximumValueMod          = null;
        //    // INumericMod constantValueMod       = null;
        //     
        //     for (int i = 0; i < modifiers.Count; i++)
        //     {
        //         var currentModifier = modifiers[i];
        //         var modType         = currentModifier.ModType;
        //
        //         switch (modType)
        //         {
        //             case NumericModType.Ignore:         {                                                     break; }
        //             case NumericModType.Multiplicative: { multiplicativeMultiplier *= currentModifier.Amount; break; }
        //             case NumericModType.Additive:       { additiveMultiplier       += currentModifier.Amount; break; }
        //             case NumericModType.Flat:           { Add(flatAmount, currentModifier.Amount); break; }
        //             case NumericModType.Minimum:          
        //             {
        //                 if (minimumValueMod is null ||
        //                     currentModifier.Amount > minimumValueMod.Amount &&
        //                     currentModifier.ModPriority <= minimumValueMod.ModPriority)
        //                 {
        //                     minimumValueMod = currentModifier;
        //                 }
        //             
        //                 break; 
        //             }
        //             case NumericModType.Maximum:          
        //             {
        //                 if (maximumValueMod is null ||
        //                     currentModifier.Amount < maximumValueMod.Amount &&
        //                     currentModifier.ModPriority <= maximumValueMod.ModPriority)
        //                 {
        //                     maximumValueMod = currentModifier;
        //                 }
        //             
        //                 break; 
        //             }
        //             // case NumericModType.ConstantValue:          
        //             // {
        //             //     if (constantValueMod is null ||
        //             //         currentModifier.ModPriority < constantValueMod.ModPriority)
        //             //     {
        //             //         constantValueMod = currentModifier;
        //             //     }
        //             //
        //             //     break; 
        //             // }
        //             default:                            
        //             {     throw new InvalidOperationException($"Unhandled case {modType} in {nameof(RecalculateModifiers)}()."); }
        //             
        //             //- TODO : Decide if you're going to implement Minimum and Maximum mods.
        //         }
        //     }
        //
        //     MultiplicativeMultiplier = multiplicativeMultiplier;
        //     AdditiveMultiplier = additiveMultiplier;
        //     FlatAdded = flatAmount;
        //
        //     // if (constantValueMod != null)
        //     // {
        //     //    // ConstantValue = constantValueMod.Amount;
        //     //     HasConstantValue = true;
        //     // }
        //     // else
        //     // {
        //     //     ConstantValue = 0;
        //     //     HasConstantValue = false;
        //     // }
        // }
        
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
        
        public bool ValueEquals(T valueToCompare) => ValuesAreDifferent(Value, valueToCompare, out _);

        public T Peek() => modifiedValue;

        protected abstract T    ApplyMaximum(T valueToModify, T maximumValue);
        protected abstract T    ApplyMinimum(T valueToModify, T minimumValue);
        protected abstract T    Multiply(T valueToModify, double multiplier);
        protected abstract T    Add(T valueToModify, T amountToAdd);
        protected abstract bool ValuesAreDifferent(T first, T second, out long triggerFlags);
        

        #endregion


        #region Constructors

        protected ValueControllerModCore(T initialBaseValue, IModTypeOrder modOrder)
        {
            baseValue = initialBaseValue;
            modTypeOrder = modOrder;
        }

        protected ValueControllerModCore(T initialBaseValue = default) : this(initialBaseValue, defaultModTypeOrder)
        {
            
        }

        #endregion

    }

    //- We could make a class that applies one or more mods to a collection of 
    //  Modifiable Factors.

    //- We could reverse this setup and have the Modifiers keep a list of Modifiables
    //  and when you add one the Modifier manually adds/subtracts from the Modifiable's
    // multipliers. 

}