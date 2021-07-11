using System.Collections.Immutable;
using System.Threading;

namespace Core
{
    class ImmutableExtensions
    {

        public static ImmutableHashSet<T> AddUntilSuccessful<T>(ref ImmutableHashSet<T> destination, T valueToAdd) 
        {
            ImmutableHashSet<T> formerCollection = Volatile.Read(ref destination);
            bool                exchangeSucceeded;

            do
            {
                ImmutableHashSet<T> newCollection = (formerCollection != null) ? 
                                                       formerCollection.Add(valueToAdd)  :  
                                                       ImmutableHashSet<T>.Empty.Add(valueToAdd);

                ImmutableHashSet<T> exchangeResult = Interlocked.CompareExchange(ref destination, newCollection, formerCollection);

                exchangeSucceeded = ReferenceEquals(exchangeResult, formerCollection);
                formerCollection  = exchangeResult;
            }
            while (exchangeSucceeded == false);
           
            return formerCollection;
        }


        //- TODO : Do some more research into how the compiler handles a situation where a method uses an out variable to store a value, but the caller used a discard as the out argument.
        public static bool TryAddUntilSuccessful<T>(ref ImmutableHashSet<T> destination, T valueToAdd, out ImmutableHashSet<T> formerCollection)
        {
            bool exchangeSucceeded;
            
            formerCollection = Volatile.Read(ref destination);

            do
            {
                ImmutableHashSet<T> newCollection;

                if(formerCollection != null)
                {
                    newCollection = formerCollection.Add(valueToAdd); 

                    if (ReferenceEquals(formerCollection, newCollection))  //- If the value is already present Add() will just return the former collection.
                    {
                        return false;
                    }
                }
                else
                {
                    newCollection = ImmutableHashSet<T>.Empty.Add(valueToAdd);
                }

                ImmutableHashSet<T> exchangeResult = Interlocked.CompareExchange(ref destination, newCollection, formerCollection);

                exchangeSucceeded = ReferenceEquals(exchangeResult, formerCollection);
                formerCollection  = exchangeResult;
            }
            while (exchangeSucceeded == false);

            return true;
        }

        public static ImmutableHashSet<T> AddUntilSuccessfulOrNull<T>(ref ImmutableHashSet<T> destination, T valueToAdd) 
        {
            ImmutableHashSet<T> formerCollection  = Volatile.Read(ref destination);
            bool                exchangeSucceeded = false;

            while (formerCollection != null  &&  exchangeSucceeded == false) 
            {
                ImmutableHashSet<T> newCollection  = formerCollection.Add(valueToAdd);
                ImmutableHashSet<T> exchangeResult = Interlocked.CompareExchange(ref destination, newCollection, formerCollection);

                exchangeSucceeded = ReferenceEquals(exchangeResult, formerCollection);
                formerCollection = exchangeResult;
            }

            return formerCollection;
        }

        public static ImmutableHashSet<T> RemoveUntilSuccessful<T>(ref ImmutableHashSet<T> destination, T valueToRemove) 
        {
            ImmutableHashSet<T> formerCollection  = Volatile.Read(ref destination);
            bool                exchangeSucceeded = false;

            while (formerCollection?.Count > 0  &&  exchangeSucceeded == false)
            { 
                ImmutableHashSet<T> newCollection  = formerCollection.Remove(valueToRemove);
                ImmutableHashSet<T> exchangeResult = Interlocked.CompareExchange(ref destination, newCollection, formerCollection);

                exchangeSucceeded = ReferenceEquals(exchangeResult, formerCollection);
                formerCollection  = exchangeResult;
            }

            return formerCollection;
        }
        
        public static ImmutableHashSet<T> ClearUntilSuccessful<T>(ref ImmutableHashSet<T> destination)
        {
            ImmutableHashSet<T> formerCollection  = Volatile.Read(ref destination);
            bool                exchangeSucceeded = false;

            while (formerCollection?.IsEmpty == false  &&  exchangeSucceeded == false)
            {
                ImmutableHashSet<T> newCollection  = formerCollection.Clear();
                ImmutableHashSet<T> exchangeResult = Interlocked.CompareExchange(ref destination, newCollection, formerCollection);

                exchangeSucceeded = ReferenceEquals(exchangeResult, formerCollection);
                formerCollection  = exchangeResult;
            }

            return formerCollection;
        }
    }
}

