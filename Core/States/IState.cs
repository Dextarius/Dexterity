using Core.Factors;

namespace Core.States
{
    // public interface IState : IFactor
    // {
    //     
    // }
    
    public interface IState<out T> : IFactor
    {
        T Value { get; }

        T Peek();
    }
}