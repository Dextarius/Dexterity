using System;
using System.Collections.Immutable;
using System.Threading;

namespace Core
{
    public static class InterlockedUtils
    {
        [Flags]
        public enum ExchangeResult { Failed = 0, Successful, Exchanged, Equal,  }

        public static T CompareExchangeUntilSuccessful<T>(ref T destination, T newValue) where T : class
        {
            T    formerValue = destination;
            bool exchangeSucceeded;

            if (ReferenceEquals(formerValue, newValue) == false)
            {
                do
                {
                    T exchangeResult = Interlocked.CompareExchange(ref destination, newValue, formerValue);
                
                    exchangeSucceeded = ReferenceEquals(exchangeResult, formerValue);
                    formerValue       = exchangeResult;
                } 
                while (!exchangeSucceeded);
            }
            
            return formerValue;
        }

        public static bool TryCompareExchange<T>(ref T destination, T newValue, T formerValue) where T : class
        {
            return Interlocked.CompareExchange(ref destination, newValue, formerValue)  ==  formerValue;
        }

        public static bool TryCompareExchange<T>(ref T destination, T newValue, T valueToReplace, out T returnedValue) where T : class
        {
            returnedValue = Interlocked.CompareExchange(ref destination, newValue, valueToReplace);

            return returnedValue == valueToReplace;
        }
        
        //- This one uses a second ref parameter and sets that variable to whatever the exchange result was while still being able
        //  to check if it matched, mostly to avoid having to write the swap in every method that wants to do so.
        public static bool TryCompareExchangeOrSet<T>(ref T destination, T newValue, ref T variableToSet) where T : class
        {
           var returnedValue  = Interlocked.CompareExchange(ref destination, newValue, variableToSet);

           if (ReferenceEquals(returnedValue, variableToSet))
           {
               return true;
           }
           else
           {
               variableToSet = returnedValue;
               return false;
           }
        }
        
        public static bool TryCompareExchangeOrSet(ref int destination, int newValue, ref int variableToSet)
        {
            var returnedValue  = Interlocked.CompareExchange(ref destination, newValue, variableToSet);
            var copyOfVariable = variableToSet;
           
            variableToSet = returnedValue;
           
            return returnedValue == copyOfVariable;
        }
        
        public static bool TryCompareExchange(ref int destination, int newValue, int valueToReplace, out int returnedValue) 
        {
            returnedValue = Interlocked.CompareExchange(ref destination, newValue, valueToReplace);

            return returnedValue == valueToReplace;
        }

        public static bool TryCompareExchangeIfNotEqual<T>(ref T destination, T newValue, T valueToReplace) where T : class
        {
            if (ReferenceEquals(valueToReplace, newValue))
            {
                return true;  //- Should this be true?  The destination has the desired value sure, 
                              //- but this execution isn't the one that set it.
            }
            else
            {
               T returnedValue = Interlocked.CompareExchange(ref destination, newValue, valueToReplace);

                return returnedValue == valueToReplace;
            }
        }

        public static bool TryCompareExchangeIfNotEqual<T>(ref T destination, T newValue, T valueToReplace, out T returnedValue) where T : class
        {
            if (ReferenceEquals(valueToReplace, newValue))
            {
                returnedValue = newValue;

                return true;  //- Should this be true?  The destination has the desired value sure, 
                              //- but this thread isn't the one that set it.
            }
            else
            {
                returnedValue = Interlocked.CompareExchange(ref destination, newValue, valueToReplace);

                return returnedValue == valueToReplace;
            }
        }

        public static void CompareExchangeUntilMaskAdded(ref int variableToAddMaskTo, int maskToAdd)
        {
            int formerState;
            int newState;
            int exchangeResult = variableToAddMaskTo;
            
            do
            {
                formerState = exchangeResult;
                newState    = formerState | maskToAdd;

                if (formerState != newState)
                {
                    exchangeResult =
                        Interlocked.CompareExchange(ref variableToAddMaskTo, newState, formerState);
                }
            } 
            while (exchangeResult != formerState);
        }
        
        //- TODO : Test this to make sure it works properly.
        public static bool TryCompareExchangeUntilMaskIsPresent(ref int variableToAddMaskTo, int maskToAdd)
        {
            int formerState;
            int newState;
            int exchangeResult = variableToAddMaskTo;
            
            do
            {
                formerState = exchangeResult;
                newState    = formerState | maskToAdd;

                if (newState != formerState)
                {
                    exchangeResult =
                        Interlocked.CompareExchange(ref variableToAddMaskTo, newState, formerState);
                    
                    if (exchangeResult == formerState)
                    {
                        return true;
                    }
                }
            } 
            while (exchangeResult != formerState);

            return false;
        }

        //- Is there a reason we have this and CompareExchangeUntilMaskAdded()?
        public static void CompareExchangeUntilMaskApplied(ref int variableToApplyMaskTo, int maskToApply)
        {
            int formerState;
            int newState;
            int exchangeResult = variableToApplyMaskTo;
            
            do
            {
                formerState = exchangeResult;
                newState    = formerState & maskToApply;

                if (formerState != newState)
                {
                    exchangeResult =
                        Interlocked.CompareExchange(ref variableToApplyMaskTo, newState, formerState);
                }
            } 
            while (exchangeResult != formerState);
        }

        public static void CompareExchangeUntilMaskRemoved(ref int variableToRemoveMaskFrom, int maskToRemove) =>
            CompareExchangeUntilMaskApplied(ref variableToRemoveMaskFrom, ~maskToRemove);

        public static void SetFlagStateEqualTo(bool desiredState, ref int fieldToSet, int maskToUse)
        {
            bool maskIsAlreadySet = (fieldToSet & maskToUse)  ==  maskToUse;
            bool changeIsNeeded   = maskIsAlreadySet != desiredState;
                
            if (changeIsNeeded)
            {
                if (desiredState)
                {
                    CompareExchangeUntilMaskAdded(ref fieldToSet, maskToUse);
                }
                else
                {
                    CompareExchangeUntilMaskRemoved(ref fieldToSet, maskToUse);
                }
            }
        }

        public static void RemoveAndExchangeUntilSuccessful<T>(ref ImmutableHashSet<T> setToRemoveFrom, T elementToRemove)
        {
            var oldSet = setToRemoveFrom;

            while (oldSet == ImmutableHashSet<T>.Empty)
            {
                var newSet = oldSet.Remove(elementToRemove);

                if (newSet == oldSet)
                {
                    return;
                }
                else
                {
                    var exchangeResult =
                        Interlocked.CompareExchange(ref setToRemoveFrom, newSet, oldSet);

                    if (exchangeResult == oldSet)
                    {
                        return;
                    }
                    else
                    {
                        oldSet = exchangeResult;
                    }
                }
            }
        }
        
        public static bool AddAndExchangeUntilPresent<T>(ref ImmutableHashSet<T> setToAddTo, T elementToAdd)
        {
            var oldSet = setToAddTo;
            var newSet = oldSet.Add(elementToAdd);
            
            while (newSet != oldSet)
            {
                var exchangeResult = Interlocked.CompareExchange(ref setToAddTo, newSet, oldSet);
                
                if (exchangeResult == oldSet)
                {
                    return true;
                }
                else
                {
                    oldSet = exchangeResult;
                    newSet = oldSet.Add(elementToAdd);
                }
            }
            
            return false;
        }
        
        public static bool TryAddAndExchange<T>(ref ImmutableHashSet<T> setToAddTo, T elementToAdd)
        {
            var oldSet = setToAddTo;
            var newSet = oldSet.Add(elementToAdd);
            
            while (newSet != oldSet)
            {
                var exchangeResult = Interlocked.CompareExchange(ref setToAddTo, newSet, oldSet);
                
                if (exchangeResult == oldSet)
                {
                    return true;
                }
                else
                {
                    oldSet = exchangeResult;
                    newSet = oldSet.Add(elementToAdd);
                }
            }
            
            return false;
        }
        
        public static bool TryExchangeUntilSetIsEmpty<T>(ref ImmutableHashSet<T> setToReplace)
        {
            var oldSet = setToReplace;

            while(oldSet != ImmutableHashSet<T>.Empty)
            {
                var exchangeResult=
                    Interlocked.CompareExchange(ref setToReplace, ImmutableHashSet<T>.Empty, oldSet);

                if (exchangeResult == oldSet)
                {
                    return true;
                }
                else
                {
                    oldSet = exchangeResult;
                }
            }

            return false;
        }
    }
}