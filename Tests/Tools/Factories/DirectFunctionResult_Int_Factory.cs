using System;
using Core.Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class DirectFunctionResult_Int_Factory : IFactory<DirectFunctionResult<int, int>>
    {
        public DirectFunctionResult<int, int> CreateInstance(IFactor<int> valueSource)
        {
            Func<int, int> valueFunction = (number) => number;
            
            return new DirectFunctionResult<int, int>(valueFunction, valueSource);
        }

        public DirectFunctionResult<int, int> CreateInstance()
        {
            int            randomNumber  = Tools.GenerateRandomInt();
            var            valueSource   = new DirectStateCore<int>(randomNumber);
            Func<int, int> valueFunction = (number) => number + Tools.GenerateRandomInt();

            return new DirectFunctionResult<int, int>(valueFunction, valueSource);
        }
        
        public DirectFunctionResult<int, int> CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}