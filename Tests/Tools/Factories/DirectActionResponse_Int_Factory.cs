using System;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class DirectActionResponse_Int_Factory : IFactory<DirectActionResponse<int>>
    {
        public DirectActionResponse<int> CreateInstance()
        {
            var         valueSource = new DirectProactiveCore<int>(1);
            Action<int> response    = (int input) => Tools.DoNothing();

            return new DirectActionResponse<int>(response, valueSource);
        }

        public DirectActionResponse<int> CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}