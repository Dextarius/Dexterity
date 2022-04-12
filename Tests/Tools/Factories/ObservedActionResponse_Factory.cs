using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class ObservedActionResponse_Factory : IFactory<ObservedActionResponse>
    {
        public ObservedActionResponse CreateInstance() => new ObservedActionResponse(Tools.DoNothing);

        public ObservedActionResponse CreateStableInstance()
        {
            var createdInstance = CreateInstance();

            createdInstance.ForceReaction();

            return createdInstance;
        }
    }
}