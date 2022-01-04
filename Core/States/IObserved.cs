using Core.Factors;

namespace Core.States
{
    public interface IObserved
    {
        void Notify_InfluencedBy(IFactor determinant);
    }
}