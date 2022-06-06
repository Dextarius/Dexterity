using Core.Factors;

namespace Core.States
{
    public interface IInteractiveCore<T> : IResult<T>, IModifiable<T>, IBaseValue<T>
    {
        
    }
}