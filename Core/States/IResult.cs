using Core.Factors;
using Core.Redirection;

namespace Core.States
{
    public interface IResult<T> : IReactorCore, IValueCore<T>, IModifiable<T>
    {
        
    }
    
    
}