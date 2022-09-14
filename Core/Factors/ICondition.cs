namespace Core.Factors
{
    public interface ICondition : IFactor<bool>
    {
        bool    IsTrue  { get; }
        bool    IsFalse { get; }
        IFactor OnTrue  { get; }
        IFactor OnFalse { get; }
    }
}