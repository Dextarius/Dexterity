using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Outcomes.Influences;
using JetBrains.Annotations;

namespace Factors
{
    // //- TODO : Do we really need this class?
    // //- The difference between a Proactor and a Reactor is that a user can specify a Proactor's state,
    //     but not a Reactor's state;
    // public abstract class Proactor : Factor<>
    // {
    //     protected abstract IFactor Influence  { get; }
    //     public    bool    IsNecessary         => Influence.IsNecessary;
    //     public    bool    HasSubscribers      => Influence.HasSubscribers;
    //     public    int     NumberOfSubscribers => Influence.NumberOfSubscribers;
    //     public    int     Priority            => Influence.Priority;
    //
    //     public bool Subscribe(IFactorSubscriber subscriberToAdd)      => Influence.Subscribe(subscriberToAdd);
    //     public void Unsubscribe(IFactorSubscriber subscriberToRemove) => Influence.Unsubscribe(subscriberToRemove);
    //     public void TriggerSubscribers()                              => Influence.TriggerSubscribers();
    //     public void NotifyNecessary()                                 => Influence.NotifyNecessary();
    //     public void NotifyNotNecessary()                              => Influence.NotifyNotNecessary();
    //     public bool Reconcile()                                       => Influence.Reconcile();
    //
    //
    //     protected Proactor(string name) : base(name)
    //     {
    //         
    //     }
    // }
}