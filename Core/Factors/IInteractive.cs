using Core.States;

namespace Core.Factors
{
    public interface IInteractive<T> : IReactive<T>, IModifiable<T>, IBaseValue<T>
    {
        
    }
}