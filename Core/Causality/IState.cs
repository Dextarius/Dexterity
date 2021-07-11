namespace Core.Causality
{
    public interface IState 
    {
        bool IsConsequential { get; }
        bool IsValid         { get; }
        bool IsInvalid       { get; }

        void NotifyInvolved();
        bool AddDependent(IOutcome dependentOutcome);
        void ReleaseDependent(IOutcome dependentOutcome);
        bool Invalidate();
        void InvalidateDependents();
    }
    
    public interface IState<T> : IState
    {
        T Value { get; }
    }
    
    
}