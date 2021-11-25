using Core.States;

namespace Core.Factors
{
    public interface IFactor : IInfluence  //- IDeterminant?
    {
        bool HasDependents      { get; }
        int  NumberOfDependents { get; }
        
        void NotifyInvolved();
        void OnChanged();
        void InvalidateDependents();
    }
}