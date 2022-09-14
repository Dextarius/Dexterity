using Core.Factors;
using Core.States;

namespace Tests.Tools.Mocks
{
    public class MockObserved : IObserved
    {
        public bool WasInfluenced { get; private set; }
        
        public void ResetWasInfluenced_ToFalse() => WasInfluenced = false;
        
        public void Notify_InfluencedBy(IFactor determinant)
        {
            WasInfluenced = true;
        }

        public void Notify_InfluencedBy(IFactor determinant, long triggerFlags)
        {
            WasInfluenced = true;
        }
    }
}