using Core.Redirection;
using Core.States;

namespace Core.Factors
{
    public interface IReactor : IDeterminant, ITriggeredState
    {
        bool IsReflexive { get; set; }
        bool AttemptReaction();
        bool ForceReaction();
        
    }
    
    //- TODO : Come back and separate IReactorCoreOwner to something else.  Only the core should need that interface.
}