namespace Core.Factors
{
    public interface IModifiableDouble  : IValue<double>
    {
        double BaseValue                { get; set; }
        double FlatAmount               { get; }
        double AdditiveMultiplier       { get; }
        double MultiplicativeMultiplier { get; }
        double ConstantValue            { get; }

        void AddModifier(INumericMod modifier);
        void RemoveModifier(INumericMod modifierToRemove);
        bool ContainsModifier(INumericMod modifierToFind);
    }
}