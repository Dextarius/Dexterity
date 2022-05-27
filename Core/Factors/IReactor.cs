using Core.Redirection;
using Core.States;

namespace Core.Factors
{
    public interface IReactor : IDeterminant, IFactorSubscriber, ITriggeredState
    {
        bool IsReacting       { get; }
        bool IsStabilizing    { get; }
        bool HasTriggers      { get; }
        int  NumberOfTriggers { get; }
        bool HasReacted       { get; }
        bool IsReflexive      { get; set; }


        bool AttemptReaction();
        bool ForceReaction();
        
    }
    
    //- TODO : Come back and separate IReactorCoreOwner to something else.  Only the core should need that interface.

    public interface IReactor<out T> : IReactor, IValue<T>
    {
        
    }
}