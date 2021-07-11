using System;
using System.Collections;
using System.Collections.Generic;
using Causality.Processes;
using Core.Causality;
using Core.Factors;
using Factors.Exceptions;
using JetBrains.Annotations;

namespace Factors.Collections
{
    public class ReactiveList<T> : ReactiveCollection<List<T>, T>, IList<T>, IList
    {

        #region Static Properties

        public static IEqualityComparer<T> DefaultValueComparer = EqualityComparer<T>.Default;

        #endregion
        

        #region Instance Properties

        public T this[int index]
        {
            get
            {
                React();
                return outcome.Value[index];
            }
        }

        #endregion
        
        #region Instance Methods

        public int IndexOf(T item)
        {
            React();
            return outcome.Value.IndexOf(item);
        }
        
        public List<T> AsNormalList => new List<T>(outcome.Value); 
        //- We may want to pause the Observer if it seems like the user is calling this to make a collection that creates no dependencies

        protected override List<T> CreateCollectionFromElements(IEnumerable<T> elements) => new List<T>(elements);


        
        #endregion
        
        
        #region Explicit Implementations

        bool IList.IsFixedSize                     => true;
        bool IList.IsReadOnly                      => true;
        int  IList.Add(object value)               => throw new CannotModifyReactiveValueException();
        void IList.Clear()                         => throw new CannotModifyReactiveValueException();
        void IList.Insert(int index, object value) => throw new CannotModifyReactiveValueException();
        void IList.Remove(object value)            => throw new CannotModifyReactiveValueException();
        void IList.RemoveAt(int index)             => throw new CannotModifyReactiveValueException();
        int  IList.IndexOf(object value)           => (value is T valueOfCorrectType) ? IndexOf(valueOfCorrectType) : -1;

        object IList.this[int index]
        {
            get => this[index];
            set => throw new CannotModifyReactiveValueException();
        }

        void IList<T>.Insert(int index, T item) => throw new CannotModifyReactiveValueException();
        void IList<T>.RemoveAt(int index) => throw new CannotModifyReactiveValueException();

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new CannotModifyReactiveValueException();
        }
        
        bool IList.Contains(object value)
        {
            if (value is T valueOfCorrectType)
            {
                return Contains(valueOfCorrectType); //- If value is of type 'T' then passing it to the regular Contains() 
            }                                        //  method will trigger a reaction.
            else
            {
                return false;
                //- TODO : Revisit this.  Do we want to notify we're involved?  There should be no way
                //         for the collection to contain an object of the wrong type though, so I don't see why
                //         changing the elements of this list would affect the outcome of another factor.
            }
        }
        
        #endregion

        
        #region Constructors

        public ReactiveList([NotNull] Func<IEnumerable<T>> functionToGenerateItems, string nameToGive = null) : 
            this(FunctionalProcess.CreateFrom(functionToGenerateItems), DefaultValueComparer, nameToGive)
        {
        }
        
        public ReactiveList([NotNull] Func<IEnumerable<T>> functionToGenerateItems, IEqualityComparer<T> comparer, string name = null) : 
            this(FunctionalProcess.CreateFrom(functionToGenerateItems), comparer, name)
        {
        }
        
        public ReactiveList([NotNull] IProcess<IEnumerable<T>> processToGenerateItems, string nameToGive = null) : 
            this(processToGenerateItems, DefaultValueComparer, nameToGive)
        {
        }
        
        public ReactiveList([NotNull] IProcess<IEnumerable<T>> processToGenerateItems, IEqualityComparer<T> comparer, string name = null) : 
            base(processToGenerateItems, comparer, name)
        {
        }
        

        
        #endregion
    }
}