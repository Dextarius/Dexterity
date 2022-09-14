namespace Core.Factors
{
    public interface IValueControllerBase<T> : IValue<T>
    {
        T      FlatAdded                { get; }
        double AdditiveMultiplier       { get; }
        double MultiplicativeMultiplier { get; }
        //   double ConstantValue            { get; }

        //  void AddModifier(INumericMod modifier);
        //  void RemoveModifier(INumericMod modifierToRemove);
        //  bool ContainsModifier(INumericMod modifierToFind);;
        
        void AddFlatModifier(INumericMod<T> modifierToAdd);
        void AddAdditiveModifier(INumericMod<double> modifierToAdd);
        void AddMultiplicativeModifier(INumericMod<double> modifierToAdd);
        void AddMaximumValueModifier(IFactor<INumericMod<T>> modifierToAdd);
        void AddMinimumValueModifier(IFactor<INumericMod<T>> modifierToAdd);
        void AddConstantValueModifier(IFactor<INumericMod<T>> modifierToAdd);
        void RemoveFlatModifier(INumericMod<T> modifierToRemove);
        void RemoveAdditiveModifier(INumericMod<double> modifierToRemove);
        void RemoveMultiplicativeModifier(INumericMod<double> modifierToRemove);
        void RemoveMaximumValueModifier(IFactor<INumericMod<T>> modifierToRemove);
        void RemoveMinimumValueModifier(IFactor<INumericMod<T>> modifierToRemove);
        void RemoveConstantValueModifier(IFactor<INumericMod<T>> modifierToRemove);
    }

    public interface ValueController<T> : IValueControllerBase<T> , IFactor<T>
    {
        
    }
    
}