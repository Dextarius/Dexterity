using System.ComponentModel.DataAnnotations;
using Factors.Cores.ObservedReactorCores;
using Tests.Tools.Interfaces;

namespace Tests.Tools.Factories
{
    public class ObservedFunctionResult_Int_Factory : IFactory<ObservedFunctionResult<int>>
    {
        public ObservedFunctionResult<int> CreateInstance() => 
            new ObservedFunctionResult<int>(Tools.GenerateRandomInt);
    }
}