namespace Core.Factors
{
    public interface IModifiableBase<T> : IValue<T>
    {
        T      BaseValue                { get;  }
        double FlatAmount               { get; }
        double AdditiveMultiplier       { get; }
        double MultiplicativeMultiplier { get; }
   //   double ConstantValue            { get; }

        void AddModifier(INumericMod modifier);
        void RemoveModifier(INumericMod modifierToRemove);
        bool ContainsModifier(INumericMod modifierToFind);
    }

    public interface IModifiable<T> : IModifiableBase<T> , IFactor<T>
    {
        
    }
}