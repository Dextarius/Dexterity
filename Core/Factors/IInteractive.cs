using Core.States;

namespace Core.Factors
{
    public interface IInteractive<T> : IResult<T>, IModifiable<T>
    {
        T BaseValue { get; set; }
    }
}