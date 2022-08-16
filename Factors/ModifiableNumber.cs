using System;
using Core.Factors;
using Factors.Cores;
using Factors.Modifiers;

namespace Factors
{ 
    public class Modifiable<T> : Reactor<IModifiableCore<T>>, IModifiable<T>
    {
        #region Properties

        public T      BaseValue          { get => core.BaseValue;
                                           set => core.BaseValue = value; }
        public T      Value                    => core.Value;
        public double FlatAmount               => core.FlatAmount;
        public double AdditiveMultiplier       => core.AdditiveMultiplier;
        public double MultiplicativeMultiplier => core.MultiplicativeMultiplier;
    //  public double ConstantValue            => core.ConstantValue;
        
        #endregion
        

        #region Instance Methods

        public void AddModifier(INumericMod modifierToAdd)       => core.AddModifier(modifierToAdd);
        public void RemoveModifier(INumericMod modifierToRemove) => core.RemoveModifier(modifierToRemove);
        public bool ContainsModifier(INumericMod modifierToFind) => core.ContainsModifier(modifierToFind);
        public bool ValueEquals(T valueToCompare)                => core.ValueEquals(valueToCompare);

        #endregion

        
        #region Constructors

        public Modifiable(IModifiableCore<T> reactorCore, T initialBaseValue, string nameToGive = null) : 
            this(reactorCore, nameToGive)
        {
            core.BaseValue = initialBaseValue;
        }
        
        public Modifiable(IModifiableCore<T> reactorCore, string nameToGive = null) : 
            base(reactorCore, nameToGive)
        {
        }
        
        #endregion


        #region Operators

        public static implicit operator T(Modifiable<T> modifiable) => modifiable.Value;

        #endregion
    }

    
    public static class Modifiable
    {
        #region Static Methods

        public static Modifiable<double>   Create(double initialValue)   => new Modifiable<double>(new ModifiableDoubleCore(initialValue));
        public static Modifiable<int>      Create(int initialValue)      => new Modifiable<int>(new ModifiableIntCore(initialValue));
        public static Modifiable<uint>     Create(uint initialValue)     => new Modifiable<uint>(new ModifiableUintCore(initialValue));
        public static Modifiable<TimeSpan> Create(TimeSpan initialValue) => new Modifiable<TimeSpan>(new ModifiableTimeSpanCore(initialValue));

        #endregion
    }

}