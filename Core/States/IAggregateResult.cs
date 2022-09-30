namespace Core.States
{
    public interface IAggregateResult<TValue> : IAggregator<TValue>, IResult<TValue>
    {
        TValue BaseValue { get; set; }
    }
}