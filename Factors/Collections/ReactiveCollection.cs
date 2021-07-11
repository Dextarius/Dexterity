using System;
using System.Collections;
using System.Collections.Generic;
using Causality.States;
using Core.Factors;
using Factors.Exceptions;
using JetBrains.Annotations;
using Observer = Causality.Observer;
using static Core.Tools.Types;
using Causality.Processes;
using Core.Causality;

namespace Factors.Collections
{
    public abstract class ReactiveCollection<TCollection, TValue> : SubscribableReactor<TCollection, TCollection>, ICollection<TValue>,
                                                                    ICollection
        where TCollection : ICollection<TValue>
    {
        #region Instance Fields

        [NotNull]
        protected          IOutcome<TCollection>         outcome = InvalidOutcome<TCollection>.Default;
        protected readonly IEqualityComparer<TValue>     itemComparer;
        protected readonly IProcess<IEnumerable<TValue>> reactionProcess;

        #endregion
        
        #region Properties

        protected override IOutcome Outcome => outcome;
        
        protected TCollection Collection { get { React();  
                                                 return outcome.Value;  } }

        public bool UsesRecycling { get; set; }
        public int Count => Collection.Count;
        
        #endregion


        #region Instance Methods

        protected override bool Act()
        {
            IOutcome<TCollection> oldOutcome  = outcome;
            IOutcome<TCollection> newOutcome  = new Outcome<TCollection>(this);
            IEnumerable<TValue>   newElements = Observer.ObserveInteractions(reactionProcess, newOutcome);

            using (Observer.PauseObservation())
            {
                // if (UsesRecycling)
                // {
                //     //- TODO: Implement this.
                // }

                TCollection newCollection = CreateCollectionFromElements(newElements);
            
                newOutcome.Value = newCollection;
                outcome = newOutcome;

                if (AreCollectionsEqual(oldOutcome.Value, newOutcome.Value) == false)
                {
                    SubscriptionManager.Publish(oldOutcome.Value, newOutcome.Value); 
                    //- TODO : This gives subscribers access to the internal collection.
                    //         We should return either an IEnumerable or a copy.
                    
                    return true;
                }
                
                return false;
            }
        }
        
        public IEnumerator<TValue> GetEnumerator()
        {
            React();

            IOutcome<TCollection> currentOutcome = outcome; //- Make sure it's the same collection each time
            
            foreach (TValue element in currentOutcome.Value)
            {
                Observer.NotifyInvolved(currentOutcome);
                yield return element;
            }
        }

        //- TODO : Decide if we're just going to use the value comparer, or if we want to flesh this out.
        protected virtual bool AreCollectionsEqual(TCollection collection1, TCollection collection2)
        {
            return EqualityComparer<TCollection>.Default.Equals(collection1, collection2);
        }

        public void CopyTo(TValue[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);

        public bool Contains(TValue item)  => Collection.Contains(item);


        protected abstract TCollection CreateCollectionFromElements(IEnumerable<TValue> elements);

        #endregion

        
        #region Constructors

        protected ReactiveCollection([NotNull] Func<IEnumerable<TValue>> functionToGenerateItems, string nameToGive = null) : 
            this(FunctionalProcess.CreateFrom(functionToGenerateItems), nameToGive)
        {

        }
        
        protected ReactiveCollection([NotNull] IProcess<IEnumerable<TValue>> processToGenerateItems, string nameToGive = null) : 
            this(processToGenerateItems, EqualityComparer<TValue>.Default, nameToGive )
        {
            
        }
        
        public ReactiveCollection([NotNull] IProcess<IEnumerable<TValue>> processToGenerateItems, 
                                  IEqualityComparer<TValue> comparerForValues, string name = null) : 
            base(name)
        {
            itemComparer    = comparerForValues?? EqualityComparer<TValue>.Default;  
            reactionProcess = processToGenerateItems ?? throw new ArgumentNullException(
                                                            $"A {NameOf<ReactiveCollection<TCollection, TValue>>()} " +
                                                            "cannot be constructed with a null process, as it would never have a value. ");
        }

        #endregion
        

        #region Operators

        //- TODO : Do we want this?  How likely are people to use it on accident?
        //  We probably shouldn't since they'll be able to modify the collection
        // public static implicit operator TCollection(ReactiveCollection<TCollection, TValue> reactiveCollection) => 
        // reactiveCollection.outcome.Value;

        #endregion
        
        #region Explicit Implementations

        //- We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
        //  We can't guarantee it'll be the same object every time.  
        void        ICollection.CopyTo(Array array, int index) => ((ICollection)Collection).CopyTo(array, index);
        void        ICollection<TValue>.Add(TValue item)       => throw new CannotModifyReactiveValueException();
        bool        ICollection<TValue>.Remove(TValue item)    => throw new CannotModifyReactiveValueException();
        void        ICollection<TValue>.Clear()                => throw new CannotModifyReactiveValueException();
        bool        ICollection.IsSynchronized                 => false;
        bool        ICollection<TValue>.IsReadOnly             => true;
        IEnumerator IEnumerable.GetEnumerator()                => GetEnumerator();
        object      ICollection.SyncRoot                       => throw new NotSupportedException(
                                                                      $"{NameOf<ProactiveCollection<TCollection, TValue>>()} does not support SyncRoot. ");
        //^ We never know when the collection is going to be replaced so returning it's SyncRoot would be meaningless.
        //  We can't guarantee it'll be the same object every time.  
        //- TODO : Come back to this later and decide if it's worthwhile/possible to implement SyncRoot.
        
        #endregion
    }
}