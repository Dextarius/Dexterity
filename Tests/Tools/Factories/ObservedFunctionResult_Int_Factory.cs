using System;
using System.ComponentModel.DataAnnotations;
using Factors;
using Factors.Cores.ObservedReactorCores;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class ObservedFunctionResult_Int_Factory : IFactory<Reactive<int>>
    {
        public Reactive<int> CreateInstance()
        {
            var       sourceCore    = new ObservedProactiveCore<int>(Tools.GenerateRandomInt());
            var       valueSource   = new Proactive<int>(sourceCore);
            Func<int> valueFunction = () => valueSource.Value + Tools.GenerateRandomInt();
            var       resultCore    = new ObservedFunctionResult<int>(valueFunction);

            sourceCore.SetOwner(valueSource);

            return new Reactive<int>(resultCore);
        }

        public Reactive<int> CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}