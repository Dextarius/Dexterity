namespace Core.Factors
{
    public interface IValue<out T>
    {
        T    Value { get; }
    }
}