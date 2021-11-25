using System.Collections.Generic;

namespace Causality.States.CollectionStates
{
    // public abstract class CollectionState<TCollection, TValue> : State  where TCollection : ICollection<TValue>
    // {
    //     protected TCollection currentCollection;
    //     
    //     protected TCollection Collection
    //     {
    //         get
    //         {
    //             Observer.NotifyInvolved(this);
    //             return currentCollection;
    //         }
    //         set
    //         {
    //             if (AreCollectionsEqual(value, currentCollection) is false)
    //             {
    //                 currentCollection = value;
    //                 NotifyChanged();
    //             }
    //         }
    //     }
    //
    //     protected abstract bool AreCollectionsEqual(TCollection first, TCollection second);
    // }
}