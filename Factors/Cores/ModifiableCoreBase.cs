using System;
using System.Collections.Generic;
using Core.Factors;
using static Core.Tools.Numerics;

namespace Factors.Cores
{
    public abstract class ModifiableCore<T> : ReactorCore, IModifiableCore<T>
    {
        #region Static Fields

        private static readonly IModTypeOrder defaultModTypeOrder = new DefaultModTypeOrder();

        #endregion
        
        
        #region Instance Fields

        private readonly List<INumericMod> modifiers = new List<INumericMod>();
        private readonly IModTypeOrder     modTypeOrder;
        private          T                 modifiedValue;
        private          T                 baseBaseValue;

        #endregion


        #region Properties

        //- We should consider changing the FlatAmount mod type to be of type T instead of being a double.
        public             double               FlatAmount               { get; private set; }
        public             double               AdditiveMultiplier       { get; private set; } = 1;
        public             double               MultiplicativeMultiplier { get; private set; } = 1;
        protected override IEnumerable<IFactor> Triggers                 => modifiers;
        public    override bool                 HasTriggers              => modifiers.Count > 0;
        public    override int                  NumberOfTriggers         => modifiers.Count;
        public             T                    Value                    => modifiedValue;
        // public T      ConstantValue            { get; private set; }
        // public bool   HasConstantValue         { get; private set; }
        
        //^ We'll have to revisit the ConstantValue mods later.  Either they can't be INumericMods, or
        //  we'll have to require inheritors to implement a method that converts a double into a T, or
        //  we'll have to change INumericMod to INumericMod<T> and add a Modify<T> where we pass the
        //  current value to the mod and it returns a modified version.
        
        public T BaseValue
        {
            get => baseBaseValue;
            set
            {
                if (ValuesAreDifferent(baseBaseValue, value))
                {
                    baseBaseValue = value;
                    Trigger();
                }
            }
        }

        #endregion
        

        #region Instance Methods

        protected override bool CreateOutcome()
        {
            RecalculateModifiers();

            T newModifiedValue = Modify(baseBaseValue);

            if (ValuesAreDifferent(modifiedValue, newModifiedValue))
            {
                modifiedValue = newModifiedValue;
                return true;
            }
            else return false;
        }
        
        public void AddModifier(INumericMod modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            AddTrigger(modifierToAdd, IsReflexive);
            modifiers.Add(modifierToAdd);
            Trigger();
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
        
        public void RemoveModifier(INumericMod modifierToRemove)
        {
            if (modifierToRemove != null && 
                modifiers.Remove(modifierToRemove))
            {
                RemoveTrigger(modifierToRemove);
                Trigger();
                // Ideally we'd be able to just check the mod's type and remove its amount from the correct 
                // property, but it seems impossible to guarantee that the mod's Amount or type hasn't changed
                // since we last used it.
            }
        }

        public bool ContainsModifier(INumericMod modifierToFind) => modifiers.Contains(modifierToFind);

        protected void RecalculateModifiers()
        {
            double      multiplicativeMultiplier = 1;
            double      additiveMultiplier       = 1;
            double      flatAmount               = 0;
           // INumericMod constantValueMod         = null;
            
            for (int i = 0; i < modifiers.Count; i++)
            {
                var currentModifier = modifiers[i];
                var modType         = currentModifier.ModType;

                switch (modType)
                {
                    case NumericModType.Ignore:         { break; }
                    case NumericModType.Multiplicative: { multiplicativeMultiplier *= currentModifier.Amount; break; }
                    case NumericModType.Additive:       { additiveMultiplier       += currentModifier.Amount; break; }
                    case NumericModType.Flat:           { flatAmount               += currentModifier.Amount; break; }
                    // case NumericModType.ConstantValue:          
                    // {
                    //     if (constantValueMod is null ||
                    //         currentModifier.ModPriority < constantValueMod.ModPriority)
                    //     {
                    //         constantValueMod = currentModifier;
                    //     }
                    //
                    //     break; 
                    // }
                    default:                            
                    {     throw new InvalidOperationException($"Unhandled case {modType} in {nameof(RecalculateModifiers)}()."); }
                    
                    //- TODO : Decide if you're going to implement Minimum and Maximum mods.
                }
            }

            MultiplicativeMultiplier = multiplicativeMultiplier;
            AdditiveMultiplier = additiveMultiplier;
            FlatAmount = flatAmount;

            // if (constantValueMod != null)
            // {
            //    // ConstantValue = constantValueMod.Amount;
            //     HasConstantValue = true;
            // }
            // else
            // {
            //     ConstantValue = 0;
            //     HasConstantValue = false;
            // }
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
                    case NumericModType.Flat:           { result = Add(result, FlatAmount);                    break; }
                    // case NumericModType.ConstantValue:
                    // {
                    //     if (HasConstantValue)
                    //     {
                    //         result = ConstantValue;
                    //     }
                    //     
                    //     break;
                    // }
                    default: { throw new InvalidOperationException($"Unhandled case {modType} in {nameof(Modify)}(). "); }
                }
            }

            return result;
        }
        
        protected override void InvalidateOutcome(IFactor changedFactor) { }
        
        public bool ValueEquals(T valueToCompare) => ValuesAreDifferent(Value, valueToCompare);


        protected abstract T    Multiply(T valueToModify, double multiplier);
        protected abstract T    Add(T valueToModify, double amountToAdd);
        protected abstract bool ValuesAreDifferent(T first, T second);
        

        #endregion


        #region Constructors

        protected ModifiableCore(T initialBaseValue, IModTypeOrder modOrder)
        {
            baseBaseValue = initialBaseValue;
            modTypeOrder = modOrder;
        }

        protected ModifiableCore(T initialBaseValue = default) : this(initialBaseValue, defaultModTypeOrder)
        {
            
        }

        #endregion

    }
}