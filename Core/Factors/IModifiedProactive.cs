using Core.States;

namespace Core.Factors
{
    public interface IModifiedProactive<T> : IModifiable<T>, IFactor<T>
    {
        IProactive<T> BaseValue { get; }
    }
}