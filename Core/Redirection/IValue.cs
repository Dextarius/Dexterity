namespace Core.Redirection
{
    public interface IValue<out T>
    {
        T Value { get; }
    }
}