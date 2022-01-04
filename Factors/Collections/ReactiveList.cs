using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Factors.Exceptions;
using JetBrains.Annotations;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public class ReactiveList<T> : ReactiveCollection<IListResult<T>, T>, IList<T>, IList, IReadOnlyListMembers<T>
    {
        #region Static Fields

        private static readonly string DefaultName = NameOf<ReactiveList<T>>();

        #endregion
        
        #region Instance Properties

        public T this[int index] => result[index];

        #endregion
        
        #region Instance Methods

        public List<T>       AsNormalList()                                       => result.AsNormalList();
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) => result.ConvertAll(converter);
        public bool          Exists(Predicate<T> predicate)                       => result.Exists(predicate);
        public T             Find(Predicate<T> predicate)                         => result.Find(predicate);
        public List<T>       FindAll(Predicate<T> predicate)                      => result.FindAll(predicate);
        public T             FindLast(Predicate<T> predicate)                     => result.FindLast(predicate);
        public void          ForEach(Action<T>    action)                         => result.ForEach(action);
        public List<T>       GetRange(int startIndex, int count)                  => result.GetRange(startIndex, count);
        public int           IndexOf(T item)                                      => result.IndexOf(item);
        public T[]           ToArray()                                            => result.ToArray();
        public bool          TrueForAll(Predicate<T> predicate)                   => result.TrueForAll(predicate);

        public int LastIndexOf(T item, int startIndex, int count) => result.LastIndexOf(item, startIndex, count);
        public int LastIndexOf(T item, int startIndex)            => result.LastIndexOf(item, startIndex);
        public int LastIndexOf(T item)                            => result.LastIndexOf(item);
        
        public int FindIndex(Predicate<T> predicate, int startIndex, int count) => result.FindIndex(predicate, startIndex, count);
        public int FindIndex(Predicate<T> predicate, int startIndex)            => result.FindIndex(predicate, startIndex);
        public int FindIndex(Predicate<T> predicate)                            => result.FindIndex(predicate);
        
        public int FindLastIndex(Predicate<T> predicate, int startIndex, int count) => result.FindLastIndex(predicate, startIndex, count);
        public int FindLastIndex(Predicate<T> predicate, int startIndex)            => result.FindLastIndex(predicate, startIndex);
        public int FindLastIndex(Predicate<T> predicate)                            => result.FindLastIndex(predicate);
        
        public int BinarySearch(T item, IComparer<T> comparer, int startIndex, int count) => result.BinarySearch(item, comparer, startIndex, count);
        public int BinarySearch(T item, IComparer<T> comparer)                            => result.BinarySearch(item, comparer);
        public int BinarySearch(T item)                                                   => result.BinarySearch(item);
        
        #endregion
        
        
        #region Constructors

        public ReactiveList([NotNull] IListResult<T> collectionSource, string name = null) : 
            base(collectionSource, name ?? DefaultName)
        {
            
        }

        #endregion
        
        
        #region Explicit Implementations

        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly  => true;

        object IList.this[int index]
        {
            get => this[index];
            set => throw new CannotModifyReactiveValueException();
        }
        
        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new CannotModifyReactiveValueException();
        }


        void IList.Insert(int index, object value) => throw new CannotModifyReactiveValueException();
        int  IList.Add(object value)               => throw new CannotModifyReactiveValueException();
        void IList.Clear()                         => throw new CannotModifyReactiveValueException();
        void IList.Remove(object value)            => throw new CannotModifyReactiveValueException();
        void IList.RemoveAt(int index)             => throw new CannotModifyReactiveValueException();
        void IList<T>.Insert(int index, T item)    => throw new CannotModifyReactiveValueException();
        void IList<T>.RemoveAt(int index)          => throw new CannotModifyReactiveValueException();
        
        bool IList.Contains(object value)
        {
            if (value is T valueOfCorrectType)
            {
                return Contains(valueOfCorrectType);  
            }                                        
            else if (default(T) is null  &&  value is null)
            {
                return Contains(default(T));
            }
            else
            {
                return false;
                //- TODO : Revisit this.  Do we want to notify we're involved?  There should be no way
                //         for the collection to contain an object of the wrong type though, so I don't see why
                //         changing the elements of this list would affect the outcome of another factor.
            }
        }
        
        int  IList.IndexOf(object value)
        {
            if (value is T valueOfCorrectType)
            {
                return IndexOf(valueOfCorrectType);
            }
            else return -1;
        }

        #endregion
    }



    // public static class ReactiveList
    // {
    //     public ReactiveList([NotNull] Func<IEnumerable<T>> functionToGenerateElements, 
    //                         IEqualityComparer<T> comparerForElements, 
    //                         string               name = null) : 
    //         base(name ?? DefaultName)
    //     {
    //         if (functionToGenerateElements is null) { throw new ArgumentNullException(nameof(functionToGenerateElements)); }
    //         
    //         result = ListFunctionResult.CreateFrom(functionToGenerateElements, this, comparerForElements);
    //     }
    //     
    //     public ReactiveList([NotNull] Func<IEnumerable<T>> functionToGenerateElements, string nameToGive = null) : 
    //         this(functionToGenerateElements, EqualityComparer<T>.Default, nameToGive)
    //     {
    //     }
    // }
}