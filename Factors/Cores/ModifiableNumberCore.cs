using System;
using System.Collections.Generic;
using Core.Factors;
using static Core.Tools.Numerics;

namespace Factors.Cores
{
    public class ModifiableNumberCore : ReactorCore, IModifiableNumberCore
    {
        #region Constants


        #endregion

        
        #region Instance Fields

        private readonly List<INumericMod> modifiers = new List<INumericMod>();
        private          ModTypeOrder      modTypeOrder;
        private          INumericMod       highestPrioritySetToMod;
        private          bool              modifiersChanged;
        private          double            modifiedValue;
        private          double            baseValue;

        #endregion
        

        #region Properties

        public double FlatAmount               { get; private set; }
        public double AdditiveMultiplier       { get; private set; } = 1;
        public double MultiplicativeMultiplier { get; private set; } = 1;
        public double SetTo                    { get; private set; } = 0;
        
        public             ModTypeOrder         ModTypeOrder     => modTypeOrder;
        protected override IEnumerable<IFactor> Triggers         => modifiers;
        public override    bool                 HasTriggers      => modifiers.Count > 0;
        public override    int                  NumberOfTriggers => modifiers.Count;
        
        public double Value => modifiedValue;

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
            if (modifiersChanged)
            {
                modifiersChanged = false;
                RecalculateModifiers();
            }

            double newModifiedValue = Modify(baseValue);

            if (DoublesAreNotEqual(modifiedValue, newModifiedValue))
            {
                modifiedValue = newModifiedValue;
                return true;
            }
            else return false;
        }
        
        public void AddModifier(INumericMod modifier)
        {
            if (modifier is null) { throw new ArgumentNullException(nameof(modifier)); }

            var modsType = modifier.ModType;

            AddTrigger(modifier, IsNecessary);

            if (modifiers.Count == 0)
            {
                modifiers.Add(modifier);   
            }
            else
            {
                int indexForMod = FindIndexForModType(modsType);
                modifiers.Insert(indexForMod, modifier);
            }

            modifiersChanged = true;
            Trigger();
        }
        
        protected int FindIndexForModType(NumericModType modType)
        {
            int priorityForModType = modTypeOrder.GetPriorityForModType(modType);
            
            for (int i = 0; i < modifiers.Count; i++)
            {
                var currentModsType     = modifiers[i].ModType;
                int currentModsPriority = modTypeOrder.GetPriorityForModType(currentModsType);
                
                if (currentModsPriority > priorityForModType)
                {
                    return i;
                }
            }

            return modifiers.Count;
        }
        
        public void RemoveModifier(INumericMod modifierToRemove)
        {
            if (modifierToRemove != null && 
                modifiers.Remove(modifierToRemove))
            {
                if (modifierToRemove == highestPrioritySetToMod)
                {
                    highestPrioritySetToMod = null;
                }

                modifiersChanged = true;
                RemoveTrigger(modifierToRemove);
                Trigger();
                // Ideally we'd be able to just check the mod's type and remove its amount from the correct 
                // property, but it seems impossible to guarantee something won't change the mod's Amount
                // before this gets called.
            }
        }

        public bool ContainsModifier(INumericMod modifierToFind) => modifiers.Contains(modifierToFind);

        protected void RecalculateModifiers()
        {
            MultiplicativeMultiplier = 1;
            AdditiveMultiplier = 1;
            FlatAmount = 0;
            SetTo = 0;
            highestPrioritySetToMod = null;
            
            for (int i = 0; i < modifiers.Count; i++)
            {
                var currentModifier = modifiers[i];
                
                ApplyModifier(currentModifier);
            }
        }
        
        public double Modify(double valueToModify)
        {
            foreach (var modType in modTypeOrder.ModTypesByPriority)
            {
                switch (modType)
                {
                    case NumericModType.Multiplicative: { valueToModify *= MultiplicativeMultiplier; break; }
                    case NumericModType.Additive:       { valueToModify *= AdditiveMultiplier;       break; }
                    case NumericModType.Flat:           { valueToModify += FlatAmount;               break; }
                    case NumericModType.SetTo:
                    {
                        if (highestPrioritySetToMod != null)
                        {
                            valueToModify = SetTo;
                        }
                        
                        break;
                    }
                    default:                            
                    {     throw new InvalidOperationException($"Unhandled case {modType} in {nameof(ModifiableNumber)}"); }
                }
            }

            return modifiedValue;
        }

        protected void ApplyModifier(INumericMod modifierToApply)
        {
            if (HasBeenTriggered is false)
            {
                var modType = modifierToApply.ModType;

                switch (modType)
                {
                    case NumericModType.Multiplicative: { MultiplicativeMultiplier *= modifierToApply.Amount; break; }
                    case NumericModType.Additive:       { AdditiveMultiplier       += modifierToApply.Amount; break; }
                    case NumericModType.Flat:           { FlatAmount               += modifierToApply.Amount; break; }
                    case NumericModType.SetTo:          
                    {
                        if (highestPrioritySetToMod == null ||
                            modifierToApply.ModPriority > highestPrioritySetToMod.ModPriority)
                        {
                            highestPrioritySetToMod = modifierToApply;
                            SetTo = modifierToApply.Amount;
                        }
                    
                        break; 
                    }
                    default:                            
                    {     throw new InvalidOperationException($"Unhandled case {modType} in {nameof(ModifiableNumber)}"); }
                }
            }
        }

        protected override void InvalidateOutcome(IFactor changedParentState) { }

        #endregion


        #region Constructors

                
        public void ModifiableNumber(ModTypeOrder modOrder)
        {
            modTypeOrder = modOrder;
        }

        #endregion
    }
}