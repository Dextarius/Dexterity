using System;
using Factors.Cores.DirectReactorCores;
using Tests.Tools.Interfaces;
using static Tests.Tools.Tools;

namespace Tests.Tools.Factories.Controllers
{
    public class DirectActionResponse_Controller : 
        DirectActionResponse_ControllerBase<DirectActionResponse<int>>
    {
        private static readonly Action<int> defaultAction = (input) => DoNothing();
        
        
        public DirectActionResponse_Controller(IFactor_T_Controller<int> inputSourceController) : 
            base(new [] { inputSourceController }, 
                 new DirectActionResponse<int>(defaultAction, inputSourceController.ControlledInstance))
        {
            
        }

        public DirectActionResponse_Controller() : this(new Proactive_Controller<DirectProactiveCore_Controller, int>())
        {
        }
    }
}