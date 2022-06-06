using Factors;
using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class ObservedActionResponse_Factory : IFactory<Reaction>
    {
        public Reaction CreateInstance()
        {
            var core = new ObservedActionResponse(Tools.DoNothing);

            return new Reaction(core);
        }

        public Reaction CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}