using Core.Redirection;

namespace Core.Factors
{
    public interface IValueCore<T> : IValue<T>, IValueEquatable<T>
    {
        T Peek();
    }
}