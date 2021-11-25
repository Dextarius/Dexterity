using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Causality;
using Causality.States;
using Core.Factors;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Core.Tools.Types;
using Causality.Processes;
using Causality.States.CollectionStates;
using Core.Causality;
using Core.States;

namespace Factors.Collections
{
    public abstract class ReactiveCollection<TOutcome, TCollection, TValue> : Reactor, ICollection<TValue>, ICollection
                                       where TOutcome                       : ICollectionResult<TCollection, TValue> 
                                       where           TCollection          : ICollection<TValue>
    {

    #region Instance Fields

    protected TOutcome outcome;

    #endregion

    #region Properties

    protected override IResult Result => outcome;

    protected TCollection Collection => outcome.Collection;
    public    int         Count      => Collection.Count;

   //- TODO : public bool UsesRecycling { get; set; } 

    #endregion


    #region Instance Methods

    public             bool                Contains(TValue item)                  => outcome.Contains(item);
    public             void                CopyTo(TValue[] array, int arrayIndex) => outcome.CopyTo(array, arrayIndex);
    public             IEnumerator<TValue> GetEnumerator()                        => outcome.GetEnumerator();
    protected override IFactor             GetFactorImplementation()              => outcome;

    #endregion


    #region Constructors

    public ReactiveCollection(string name = null) : base(name)
    {
        //- TODO : We can't really initialize outcome, since the outcome constructors require the owner as a parameter 
        //         and you cant use 'this' when calling any of the arguments for a base constructor.
    }

    #endregion


    #region Operators

    //- TODO : Do we want this?  How likely are people to use it on accident?
    //  We probably shouldn't, since they'll be able to modify the collection if we give them a direct reference. 
    // public static implicit operator TCollection(ReactiveCollection<TCollection, TValue> reactiveCollection) => 
    // reactiveCollection.outcome.Value;

    #endregion

    #region Explicit Implementations

    //- We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
    //  We can't guarantee it'll be the same object every time.  
    void ICollection.          CopyTo(Array array, int index) => ((ICollection) Collection).CopyTo(array, index);
    void ICollection<TValue>.  Add(TValue item)               => throw new CannotModifyReactiveValueException();

    bool ICollection<TValue>.Remove(TValue item) => throw new CannotModifyReactiveValueException();
    void ICollection<TValue>.Clear()             => throw new CannotModifyReactiveValueException();
    bool ICollection.        IsSynchronized      => false;
    bool ICollection<TValue>.IsReadOnly          => true;
    IEnumerator IEnumerable. GetEnumerator()     => GetEnumerator();
    object ICollection.SyncRoot => throw new NotSupportedException(
                                       $"{NameOf<ReactiveCollection<TOutcome, TCollection, TValue>>()} does not support SyncRoot. ");
    //^ We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
    //  We can't guarantee it'll be the same object every time.  
    //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.

    #endregion

    }
}