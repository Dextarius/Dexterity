using Core.Redirection;

namespace Core.Factors
{
    public interface IValueCore<T> : IValue<T>
    {
        bool ValueEquals(T valueToCompare);
    }
}