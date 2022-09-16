using System;
using Core.Factors;
using Factors.Cores;

namespace Factors
{
    public class ValueControllerMod<T> : Reactor<IValueControllerCore<T>>, ValueController<T>
    {
        #region Properties
        
        public T      Value                    => core.Value;
        public T      FlatAdded                => core.FlatAdded;
        public double AdditiveMultiplier       => core.AdditiveMultiplier;
        public double MultiplicativeMultiplier => core.MultiplicativeMultiplier;
        //  public double ConstantValue            => core.ConstantValue;
        
        #endregion
        

        #region Instance Methods

        //  public void AddModifier(INumericMod modifierToAdd)       => core.AddModifier(modifierToAdd);
        //  public void RemoveModifier(INumericMod modifierToRemove) => core.RemoveModifier(modifierToRemove);
        //  public bool ContainsModifier(INumericMod modifierToFind) => core.ContainsModifier(modifierToFind);
        //  public bool ValueEquals(T valueToCompare)                => core.ValueEquals(valueToCompare);

        public void AddFlatModifier(INumericMod<T> modifierToAdd)                         => core.AddFlatModifier(modifierToAdd);
        public void AddAdditiveModifier(INumericMod<double> modifierToAdd)                => core.AddAdditiveModifier(modifierToAdd);
        public void AddMultiplicativeModifier(INumericMod<double> modifierToAdd)          => core.AddMultiplicativeModifier(modifierToAdd);
        public void AddMaximumValueModifier(IFactor<INumericMod<T>> modifierToAdd)        => core.AddMaximumValueModifier(modifierToAdd);
        public void AddMinimumValueModifier(IFactor<INumericMod<T>> modifierToAdd)        => core.AddMinimumValueModifier(modifierToAdd);
        public void AddConstantValueModifier(IFactor<INumericMod<T>> modifierToAdd)       => core.AddConstantValueModifier(modifierToAdd);
        public void RemoveFlatModifier(INumericMod<T> modifierToRemove)                   => core.RemoveFlatModifier(modifierToRemove);
        public void RemoveAdditiveModifier(INumericMod<double> modifierToRemove)          => core.RemoveAdditiveModifier(modifierToRemove);
        public void RemoveMultiplicativeModifier(INumericMod<double> modifierToRemove)    => core.RemoveMultiplicativeModifier(modifierToRemove);
        public void RemoveMaximumValueModifier(IFactor<INumericMod<T>> modifierToRemove)  => core.RemoveMaximumValueModifier(modifierToRemove);
        public void RemoveMinimumValueModifier(IFactor<INumericMod<T>> modifierToRemove)  => core.RemoveMinimumValueModifier(modifierToRemove);
        public void RemoveConstantValueModifier(IFactor<INumericMod<T>> modifierToRemove) => core.RemoveConstantValueModifier(modifierToRemove);

        public bool ValueEquals(T valueToCompare) => core.ValueEquals(valueToCompare);

        public T Peek() => core.Peek();

        public override bool CoresAreNotEqual(IValueControllerCore<T> oldCore, IValueControllerCore<T> newCore)
        {
            throw new NotImplementedException();
        }
        //- TODO : We're going to want to add something to SwapCore() so that we keep the mods that have been added to us.
        
        #endregion

        
        #region Constructors

        public ValueControllerMod(IValueControllerCore<T> reactorCore, T initialBaseValue, string nameToGive = null) : 
            this(reactorCore, nameToGive)
        {
            core.BaseValue = initialBaseValue;
        }
        
        public ValueControllerMod(IValueControllerCore<T> reactorCore, string nameToGive = null) : 
            base(reactorCore, nameToGive)
        {
        }
        
        #endregion


        #region Operators

        public static implicit operator T(ValueControllerMod<T> modifiable) => modifiable.Value;

        #endregion
    }
    
    
    public static class ValueController
    {
        #region Static Methods

        public static ValueControllerMod<double> Create(double initialValue) => 
            new ValueControllerMod<double>(new DoubleControllerModCore(initialValue));

        public static ValueControllerMod<int> Create(int initialValue) =>
            new ValueControllerMod<int>(new IntControllerModCore(initialValue));

        public static ValueControllerMod<uint> Create(uint initialValue) =>
            new ValueControllerMod<uint>(new UIntControllerModCore(initialValue));

        public static ValueControllerMod<TimeSpan> Create(TimeSpan initialValue) =>
            new ValueControllerMod<TimeSpan>(new TimeSpanControllerModCore(initialValue));

        #endregion
    }
}