namespace Tests.Tools
{
    public abstract class ManipulatableState<TState, TValue>  
    {
        protected TState state;

        public abstract void SetValueTo(TValue value);
    }
}