using Core.Factors;
using Factors.Cores;
using Factors.Modifiers;

namespace Factors
{
    public class ModifiableNumber : Reactor<IModifiableNumberCore>, IModifiableNumber
    {
        #region Properties

        public double       BaseValue          { get => core.BaseValue; 
                                                 set => core.BaseValue = value; }
        public double       Value                    => core.Value;
        public ModTypeOrder ModTypeOrder             => core.ModTypeOrder;
        public double       FlatAmount               => core.FlatAmount;
        public double       AdditiveMultiplier       => core.AdditiveMultiplier;
        public double       MultiplicativeMultiplier => core.MultiplicativeMultiplier;
        public double       SetTo                    => core.SetTo;
        
        #endregion

        
        #region Instance Methods

        public void      AddModifier(INumericMod modifierToAdd)    => core.AddModifier(modifierToAdd);
        public void   RemoveModifier(INumericMod modifierToRemove) => core.RemoveModifier(modifierToRemove);
        public bool ContainsModifier(INumericMod modifierToFind)   => core.ContainsModifier(modifierToFind);

        #endregion

        
        #region Constructors

        protected ModifiableNumber(IModifiableNumberCore reactorCore, string nameToGive) : base(reactorCore, nameToGive)
        {
        }

        public ModifiableNumber(string nameToGive = null) : this(new ModifiableNumberCore(), nameToGive)
        {
            
        }
        
        public ModifiableNumber(double initialValue, string nameToGive = null) : this(new ModifiableNumberCore(), nameToGive)
        {
            core.BaseValue = initialValue;
        }
        
        #endregion


        #region Operators

        public static implicit operator double(ModifiableNumber number) => number.Value;

        #endregion
    }
}