using Core.Factors;

namespace Core.States
{
    public interface IAggregateValue<T> : IAggregator<T>, IFactor<T> 
    {

    }
}