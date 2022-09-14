namespace Core.Factors
{
    public interface IInvolved
    {
        void NotifyInvolved();
        void NotifyInvolved(long triggerFlags);
    }
}