namespace Core.Factors
{
    public interface IModifiableChannel<T> 
    {
        bool Subscribe(IChannelModifier<T> subscriberToAdd);
        bool Unsubscribe(IChannelModifier<T> subscriberToRemove);
    }
}