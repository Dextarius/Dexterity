namespace Core.Factors
{
    public interface IModifiableDouble  : IValue<double>
    {
        double       BaseValue                { get; set; }
        ModTypeOrder ModTypeOrder             { get; }
        double       FlatAmount               { get; }
        double       AdditiveMultiplier       { get; }
        double       MultiplicativeMultiplier { get; }
        double       SetTo                    { get; }
        void         AddModifier(INumericMod modifier);
        void         RemoveModifier(INumericMod modifierToRemove);
        bool         ContainsModifier(INumericMod modifierToFind);
    }
}