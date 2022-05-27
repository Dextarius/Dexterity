using Core.States;

namespace Core.Factors
{
    public interface IDeterminant : IFactor, INecessary
    {
        bool HasSubscribers      { get; }
        int  NumberOfSubscribers { get; }
        
        void TriggerSubscribers();
       // void CopySubscriptionsTo(IFactor factorToCopyTo);
       
    }
}