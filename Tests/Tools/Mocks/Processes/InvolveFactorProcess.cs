using Core.Causality;
using Core.Factors;

namespace Tests.Tools.Mocks.Processes
{
    public class InvolveFactorProcess<TFactor> : IProcess 
        where TFactor : IInvolved
    {
        private readonly TFactor factor;
        
        public void Execute()
        {
            factor.NotifyInvolved();
        }

        public InvolveFactorProcess(TFactor factorToInvolve)
        {
            factor = factorToInvolve;
        }
    }
    
    public class InvolveFactorProcess<TFactor, TValue> : IProcess<TValue>
        where TFactor : IInvolved
    {
        private readonly TFactor factor;
        
        public TValue Execute()
        {
            factor.NotifyInvolved();
            return default(TValue);
        }

        public InvolveFactorProcess(TFactor factorToInvolve)
        {
            factor = factorToInvolve;
        }
    }
}