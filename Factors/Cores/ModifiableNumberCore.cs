using System;
using System.Collections.Generic;
using Core.Factors;
using static Core.Tools.Numerics;

namespace Factors.Cores
{
    public class ModifiableNumberCore : ReactorCore, IModifiableNumberCore
    {
        #region Static Fields

        private static readonly IModTypeOrder defaultModTypeOrder = new DefaultModTypeOrder();

        #endregion
        
        
        #region Instance Fields

        private readonly List<INumericMod> modifiers = new List<INumericMod>();
        private readonly IModTypeOrder     modTypeOrder;
        private          double            modifiedValue;
        private          double            baseValue;

        #endregion


        #region Properties

        public double FlatAmount               { get; private set; }
        public double AdditiveMultiplier       { get; private set; } = 1;
        public double MultiplicativeMultiplier { get; private set; } = 1;
        public double ConstantValue            { get; private set; } = 0;
        public bool   HasConstantValue         { get; private set; }
        
        protected override IEnumerable<IFactor> Triggers         => modifiers;
        public    override bool                 HasTriggers      => modifiers.Count > 0;
        public    override int                  NumberOfTriggers => modifiers.Count;
        public             double               Value            => modifiedValue;

        public double BaseValue
        {
            get => baseValue;
            set
            {
                if (DoublesAreNotEqual(baseValue, value))
                {
                    baseValue = value;
                    Trigger();
                }
            }
        }

        #endregion
        

        #region Instance Methods

        protected override bool CreateOutcome()
        {
            RecalculateModifiers();

            double newModifiedValue = Modify(baseValue);

            if (DoublesAreNotEqual(modifiedValue, newModifiedValue))
            {
                modifiedValue = newModifiedValue;
                return true;
            }
            else return false;
        }
        
        public void AddModifier(INumericMod modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            AddTrigger(modifierToAdd, IsNecessary);
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
            INumericMod constantValueMod         = null;
            
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
                    case NumericModType.ConstantValue:          
                    {
                        if (constantValueMod is null ||
                            currentModifier.ModPriority < constantValueMod.ModPriority)
                        {
                            constantValueMod = currentModifier;
                        }
                    
                        break; 
                    }
                    default:                            
                    {     throw new InvalidOperationException($"Unhandled case {modType} in {nameof(RecalculateModifiers)}()."); }
                    
                    //- TODO : Decide if you're going to implement Minimum and Maximum mods.
                }
            }

            MultiplicativeMultiplier = multiplicativeMultiplier;
            AdditiveMultiplier = additiveMultiplier;
            FlatAmount = flatAmount;

            if (constantValueMod != null)
            {
                ConstantValue = constantValueMod.Amount;
                HasConstantValue = true;
            }
            else
            {
                ConstantValue = 0;
                HasConstantValue = false;
            }
        }
        
        public double Modify(double valueToModify)
        {
            double result = valueToModify;
            
            foreach (var modType in modTypeOrder.ModTypesByPriority)
            {
                switch (modType)
                {
                    case NumericModType.Multiplicative: { result *= MultiplicativeMultiplier; break; }
                    case NumericModType.Additive:       { result *= AdditiveMultiplier;       break; }
                    case NumericModType.Flat:           { result += FlatAmount;               break; }
                    case NumericModType.ConstantValue:
                    {
                        if (HasConstantValue)
                        {
                            result = ConstantValue;
                        }
                        
                        break;
                    }
                    default: { throw new InvalidOperationException($"Unhandled case {modType} in {nameof(Modify)}(). "); }
                }
            }

            return result;
        }
        
        protected override void InvalidateOutcome(IFactor changedFactor) { }

        #endregion


        #region Constructors
        
        public ModifiableNumberCore(IModTypeOrder modOrder)
        {
            modTypeOrder = modOrder;
        }
        
        public ModifiableNumberCore() : this(defaultModTypeOrder)
        {
        }

        #endregion
    }
}