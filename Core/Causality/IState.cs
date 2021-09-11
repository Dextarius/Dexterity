namespace Core.Causality
{
    public interface IState 
    {
        long CurrentVersion   { get; }
        bool IsConsequential { get; }
        bool IsValid         { get; }
        bool IsInvalid       { get; }
        bool IsNecessary     { get; }
        bool IsPendingUpdate { get; }
        bool IsStable         { get; set; }
        int  Depth           { get; }

        void NotifyInvolved();
        bool AddDependent(IOutcome dependentOutcome);
        void ReleaseDependent(IOutcome dependentOutcome);
        bool Invalidate();
        void InvalidateDependents();
        bool RecalculateUp();
        bool ManuallyRecalculate();
    }
    
    public interface IState<T> : IState
    {
        T Value { get; }
    }
    
    
}