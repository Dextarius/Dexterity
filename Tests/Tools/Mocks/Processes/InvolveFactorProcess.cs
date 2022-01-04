using Core.Causality;
using Core.Factors;

namespace Tests.Tools.Mocks.Processes
{
    public class InvolveFactorProcess : IProcess 
    {
        private readonly IFactor factor;
        
        public void Execute()
        {
            factor.NotifyInvolved();
        }

        public InvolveFactorProcess(IFactor factorToInvolve)
        {
            factor = factorToInvolve;
        }
    }
    
    public class InvolveFactorProcess<T> : IProcess<T>
    {
        private readonly IFactor factor;
        
        public T Execute()
        {
            factor.NotifyInvolved();
            return default(T);
        }

        public InvolveFactorProcess(IFactor factorToInvolve)
        {
            factor = factorToInvolve;
        }
    }
}