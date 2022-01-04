using Core.Causality;
using Core.Factors;

namespace Tests.Tools.Mocks.Processes
{
    public class IncrementingProcess : IProcess
    {
        private IFactor involvedFactor;
        
        public int NumberOfTimesExecuted { get; private set; }
        
        public void Execute()
        {
            NumberOfTimesExecuted++;
            involvedFactor?.NotifyInvolved();
        }

        public void ResetCount() => NumberOfTimesExecuted = 0;

        public IncrementingProcess(IFactor factorToInvolve = null)
        {
            involvedFactor = factorToInvolve;
        }
    }
}