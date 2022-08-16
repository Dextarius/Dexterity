namespace Core.Factors
{
    public interface IInfluenceOwner
    {
        void OnNecessary();
        void OnNotNecessary();
        void OnLastSubscriberLost();
        void OnFirstSubscriberGained();
    }
}