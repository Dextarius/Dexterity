namespace Core.Factors
{
    public interface IChannel<T> 
    {
        bool Subscribe(IChannelSubscriber<T> subscriberToAdd);
        bool Unsubscribe(IChannelSubscriber<T> subscriberToRemove);
    }
}