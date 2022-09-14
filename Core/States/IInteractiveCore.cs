using Core.Factors;

namespace Core.States
{
    public interface IInteractiveCore<T> : IResult<T>, ValueController<T>, IBaseValue<T>
    {
        
    }
}