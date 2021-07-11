using System;
using Core.Subscriptions;
using JetBrains.Annotations;

namespace Subscriptions
{
    public abstract class DelegateSubscription<TArg1, TArg2, TDelegate> : SubscriptionBase<TArg1, TArg2>
    {
        [NotNull]
        protected readonly TDelegate delegateToExecute;
        
        
        public override bool DelegateEquals(SubscriptionBase<TArg1, TArg2> subscriptionToTest) => 
            subscriptionToTest.DelegateEquals(delegateToExecute);

        public override bool DelegateEquals<T>(T delegateToTest)
        {
            if (delegateToTest is TDelegate delegateOfCorrectType)
            {
                //- TODO : Test to make sure that this will return true if someone submits an Action that 
                //          is equivalent (i.e. the original was created, and now we're testing against a copy with
                //          the same target and method list).
                
                return delegateOfCorrectType.Equals(delegateToExecute);
                
                
                //if (actionToTest.Target == actionToTake.Target)
                //{
                //    var theirInvocationList = actionToTest.GetInvocationList();
                //    var ourInvocationList   = actionToTake.GetInvocationList();
                //
                //    if (theirInvocationList.Length == ourInvocationList.Length)
                //    {
                //        for (int i = 0; i < theirInvocationList.Length; i++)
                //        {
                //            if (Object.Equals(theirInvocationList[i], ourInvocationList[i]) is false)
                //            {
                //                return false;
                //            }
                //        }
                //
                //        return true;
                //    }
                //}

            }
            else
            {
                return false;
            }
        }
        
        public override bool Equals(object objectToCompare)
        {
            if (objectToCompare == this)
            {
                return true;
            }
            if (objectToCompare is TDelegate delegateToCompare)
            {
                return delegateToExecute.Equals(delegateToCompare);
            }
            else if (objectToCompare is DelegateSubscription<TArg1, TArg2, TDelegate> subscriptionToCompare)
            {
                return this.delegateToExecute.Equals(subscriptionToCompare.delegateToExecute);
            }
            else
            {
                return base.Equals(objectToCompare);
            }
        }

        protected DelegateSubscription(TDelegate delegateToExecute, ISubscribable<TArg1, TArg2> attachedProvider) : base(attachedProvider)
        {
            if (delegateToExecute == null) { throw new ArgumentNullException(nameof(delegateToExecute)); }
            
            this.delegateToExecute = delegateToExecute;
        }
    }
}