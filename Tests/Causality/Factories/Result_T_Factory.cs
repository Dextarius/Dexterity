using System;
using Causality;
using Causality.States;
using Core.Causality;
using Core.Factors;
using Core.States;
using Tests.Causality.Interfaces;
using Tests.Causality.Mocks;

namespace Tests.Causality.Factories
{
    public abstract class Result_T_Factory<TResult, TValue> : Result_Factory<TResult>, IResultFactory<TResult, TValue>
        where TResult : IResult<TValue>
    {
        public abstract TValue  CreateRandomInstanceOfValuesType();
        public abstract TValue  CreateRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
        public abstract TResult CreateInstance_WithValue(TValue value);
        public abstract TResult CreateInstance_WhoseUpdateCalls(IProcess<TValue> processToCall);
    }


    public abstract class Result_T_Factory<TValue> : Result_T_Factory<Result<TValue>, TValue>, IRandomGenerator<TValue>
    {
        #region Instance Methods

        public override Result<TValue> CreateInstance()
        {
            TValue                     valueForInstance = CreateRandomInstanceOfValuesType();
            StoredValueProcess<TValue> valueProcess     = new StoredValueProcess<TValue>(valueForInstance);
            Result<TValue>             createdOutcome   = new Result<TValue>(valueProcess);

            return createdOutcome;
        }
        
        public override Result<TValue> CreateInstance_WhoseUpdateCalls(IProcess<TValue> processToCall) =>
            new Result<TValue>(null, processToCall);

        public override Result<TValue> CreateInstance_WithValue(TValue value)
        {
            var valueProcess = new StoredValueProcess<TValue>(value);

            return CreateInstance_WhoseUpdateCalls(valueProcess);
        }
        
        public override Result<TValue> CreateInstance_WhoseUpdateCalls(IProcess processToCall)
        {
            var valueProcess = new RandomValueProcess<TValue>(this, processToCall);
            return new Result<TValue>(valueProcess);
        }
        public override Result<TValue> CreateInstance_WhoseUpdateInvolves(IFactor factorToInvolve)
        {
            var valueProcess = new InvolveFactorProcess<TValue>(factorToInvolve);

            return new Result<TValue>(valueProcess);
        }

        #endregion
        
        
        #region Explicit Implementations

        TValue IRandomGenerator<TValue>.CreateRandomValue() => CreateRandomInstanceOfValuesType();
        
        TValue IRandomGenerator<TValue>.CreateRandomValueNotEqualTo(TValue valueToAvoid) => 
            CreateRandomInstanceOfValuesType_NotEqualTo(valueToAvoid);

        #endregion
    }
}