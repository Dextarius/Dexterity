using Core.Subscriptions;
using Subscriptions;

namespace Factors
{
    //- TODO : We need to consider how we're going to prevent memory leaks caused by the delegates having references
    //         to their targets.
    //- TODO : Consider if we want subscribers to keep this object alive or not.

    public abstract class SubscribableReactor<TArg1,TArg2> : Reactor
    {
        #region Instance Fields

        //- TODO : Consider if we really want every subscribable to have a manager by default.
        protected readonly ISubscriptionManager<TArg1, TArg2> SubscriptionManager = new SubscriptionManager<TArg1, TArg2>(); 
        
        #endregion
        
        
        #region Properties
        
        public ISubscribable<TArg1, TArg2> Subscriptions => SubscriptionManager;
        
        #endregion


        #region Constructors

        protected SubscribableReactor(string nameToGive = null) : base(nameToGive)
        {
        }

        #endregion
    }
}