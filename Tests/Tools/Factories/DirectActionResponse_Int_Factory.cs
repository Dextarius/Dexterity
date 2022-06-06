using System;
using Factors;
using Factors.Cores.DirectReactorCores;
using Factors.Cores.ProactiveCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class DirectActionResponse_Int_Factory : IFactory<Reaction>
    {
        public Reaction CreateInstance()
        {
            var valueSource = new Proactive<int>(1);
            var core        = new DirectActionResponse<int>(Response, valueSource);

            return new Reaction(core);
        }
        
        private void Response(int input) => Tools.DoNothing();

        public Reaction CreateStableInstance()
        {
            var createdInstance = CreateInstance();
            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}