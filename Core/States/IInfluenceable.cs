namespace Core.States
{
    public interface IInfluenceable
    {
        void Notify_InfluencedBy(IInfluence influence);
    }
}