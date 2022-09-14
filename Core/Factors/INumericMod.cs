namespace Core.Factors
{
    public interface INumericMod<T> : IFactor<T>, INumericModBase<T>
    {

    }

    public interface INumericModBase<T> : IValue<T>
    {
        NumericModType ModType     { get; }
        int            ModPriority { get; }
    }
    
}