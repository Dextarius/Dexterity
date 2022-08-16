using System;
using Core.Factors;

namespace Factors.Cores.ProactiveCores
{
    public class ProactorCore : FactorCore, IProactorCore
    {
        public IFactorCoreCallback Callback { get; protected set; }

        public void SetCallback(IFactorCoreCallback newCallback)
        {
            Callback = newCallback ?? throw new ArgumentNullException(nameof(newCallback));
        }
        
        public override void Dispose()
        {
            Callback = null;
        }
    }
}