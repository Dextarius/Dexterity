namespace Core.States
{
    public interface IMutableState<T> : IState<T>
    {
        new T Value { get; set; }
        
    }
}