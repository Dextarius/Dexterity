using Core.Factors;
using Core.States;

namespace Factors.Cores.DirectReactorCores
{
    public abstract class ValueChangedResponse<T> : DirectReactorCore
    {
        protected readonly IFactor<T> factorToWatch;
        private            T          previousValue;
        
        
        protected override long CreateOutcome()
        {
            T    currentValue  = factorToWatch.Peek();
            long triggerFlags = ExecuteResponse(currentValue, previousValue);

            previousValue = currentValue;

            return triggerFlags;
        }

        protected abstract long ExecuteResponse(T newValue, T oldValue);
        
        
        protected ValueChangedResponse(IFactor<T> valueFactor)
        {
            factorToWatch = valueFactor;
        }
    }
    
    
    
    // public abstract class ValueLimiterResponse<T> : ValueChangedResponse<T>
    // {
    //     protected readonly IProactive<T> factorToLimit;
    //     
    //
    //     protected override long ExecuteResponse(T newValue, T oldValue)
    //     {
    //         if (IsLessThan(newValue, factorToLimit.Value))
    //         {
    //             factorToLimit.Value = newValue;
    //         }
    //         else
    //         {
    //             var changeAmount = newValue - oldValue;
    //             
    //             
    //         }
    //         
    //     }
    //
    //     protected abstract bool IsLessThan(T value1, T value2);
    //     
    //     protected ValueLimiterResponse(IFactor<T> valueFactor, IProactive<T> factorBeingLimited) : base(valueFactor)
    //     {
    //         factorToLimit = factorBeingLimited;
    //     }
    // }
}