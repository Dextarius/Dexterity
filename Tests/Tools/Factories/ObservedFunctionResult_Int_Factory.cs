using System;
using System.ComponentModel.DataAnnotations;
using Factors.Cores.ObservedReactorCores;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class ObservedFunctionResult_Int_Factory : IFactory<ObservedFunctionResult<int>>
    {
        public ObservedFunctionResult<int> CreateInstance()
        {
            var       valueSource   = new ObservedStateCore<int>(Tools.GenerateRandomInt());
            Func<int> valueFunction = () => valueSource.Value + Tools.GenerateRandomInt();
            
            return new ObservedFunctionResult<int>(valueFunction);
        }

        public ObservedFunctionResult<int> CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}