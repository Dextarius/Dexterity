namespace Core.Factors
{
    public interface INumericModCore<T> : INumericModBase<T>, IFactorCore
    {
      //  bool               IsEnabled   { get; set; }
        new NumericModType ModType     { get; set; }
        new int            ModPriority { get; set; }
    }
}