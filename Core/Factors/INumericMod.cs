namespace Core.Factors
{
    public interface INumericMod : IFactor, INumericModBase
    {

    }

    public interface INumericModBase 
    {
        NumericModType ModType     { get; }
        int            ModPriority { get; }
        double         Amount      { get; }
    }
}