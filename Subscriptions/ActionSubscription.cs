using System;
using Core.Subscriptions;

namespace Subscriptions
{
    public class ActionSubscription<TArg1, TArg2> : DelegateSubscription<TArg1, TArg2, Action>
    {
        public override void Execute(TArg1 arg1, TArg2 arg2) => delegateToExecute.Invoke();
        
        public ActionSubscription(Action actionToTake, ISubscribable<TArg1, TArg2> attachedProvider) : base(actionToTake, attachedProvider)
        {
        }
    }
    
    
    public class ActionTSubscription<TArg1, TArg2> : DelegateSubscription<TArg1, TArg2, Action<TArg1>>
    {
        //- We could make a flag that controls which argument is passed into the delegate.
        public override void Execute(TArg1 arg1, TArg2 arg2) => delegateToExecute.Invoke(arg1);

        public ActionTSubscription(Action<TArg1> actionToTake, ISubscribable<TArg1, TArg2> attachedProvider) : base(actionToTake, attachedProvider)
        {
        }
    }
    
    
    public class ActionT2Subscription<TArg1, TArg2> : DelegateSubscription<TArg1, TArg2, Action<TArg1,TArg2>>
    {
        public override void Execute(TArg1 arg1, TArg2 arg2) => delegateToExecute.Invoke(arg1, arg2);

        public ActionT2Subscription(Action<TArg1, TArg2> actionToTake, ISubscribable<TArg1, TArg2> attachedProvider) : base(actionToTake, attachedProvider)
        {
        }
    }
}