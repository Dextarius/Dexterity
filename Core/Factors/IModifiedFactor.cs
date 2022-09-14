namespace Core.Factors
{
    public interface IModifiedFactor<T> : IModifiable<T>, IFactor<T>
    {
        IFactor<T> BaseValue { get; }
    }
}