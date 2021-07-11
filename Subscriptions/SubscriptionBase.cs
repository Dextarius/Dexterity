using Core.Subscriptions;
using static Core.InterlockedUtils;

namespace Subscriptions
{
    public abstract class SubscriptionBase<TArg1, TArg2> : ISubscription, ISubscriber<TArg1, TArg2>
    {
        #region Instance Fields

        private ISubscribable<TArg1, TArg2> subscriptionProvider;

        #endregion

        
        #region Instance Methods

        public abstract void Execute(TArg1 arg1, TArg2 arg2);

        public abstract bool DelegateEquals(SubscriptionBase<TArg1, TArg2> subscriptionToTest);
        public abstract bool DelegateEquals<T>(T delegateToTest);
        
        public void Unsubscribe()
        {
            var formerProvider = subscriptionProvider;

            if (formerProvider != null && 
                TryCompareExchange(ref subscriptionProvider, null, formerProvider))
            {
                formerProvider.Unsubscribe(this);    
                
                //- TODO : We have to make a base interface that we can pass to a SubManager's
                //         Unsubscribe method.  ISubscription comes to mind, but it's not a base
                //         interface, it only applies to subs backed by Actions.
            }
        }

        #endregion


        #region Constructors

        //- TODO : We should probably use an interface other than ISubscribable,
        //         since that one only applies to Actions without parameters.
        
        protected SubscriptionBase(ISubscribable<TArg1, TArg2> attachedProvider)
        {
            subscriptionProvider = attachedProvider;
        }

        #endregion
    }
}