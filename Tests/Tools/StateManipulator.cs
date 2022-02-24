namespace Tests.Tools
{
    public abstract class ManipulatableState<TState, TValue>  
    {
        #region Instance Fields

        protected TState state;

        #endregion

        public abstract void SetValueTo(TValue value);
        
        
    }
}