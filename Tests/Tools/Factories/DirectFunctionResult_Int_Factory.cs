using System;
using Core.Factors;
using Core.States;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class DirectFunctionResult_Int_Factory : Reactive_Int_Factory<DirectFunctionResult<int, int>>
    {
        public override DirectFunctionResult<int, int> CreateCore()
        {
            int            randomNumber  = Tools.GenerateRandomInt();
            var            valueSource   = new Proactive<int>(randomNumber);
            Func<int, int> valueFunction = (number) => number + Tools.GenerateRandomInt();

            return new DirectFunctionResult<int, int>(valueSource, valueFunction);
        }
    }
    
    public abstract class Reactive_Int_Factory<TCore> : IFactory<Reactive<int>>
        where TCore : IResult<int>
    {
        public abstract TCore CreateCore();

        public Reactive<int> CreateInstance()
        {
            var createdCore     = CreateCore();
            var createdInstance = new Reactive<int>(createdCore);

            return createdInstance;
        }
        
        public Reactive<int> CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}