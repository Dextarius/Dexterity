using System.Collections.Generic;
using Core.Factors;
using Factors;
using Tests.Tools.Interfaces;

namespace Tests.Class_Tests.Cores.DirectProactorCores
{
    public class StateCore<TFactor, TCore, TFactory> : FactorCores<TFactor, TCore, TFactory> 
        where TFactor  : Factor<TCore> 
        where TCore    : IDeterminant
        where TFactory : IFactory<TFactor>, new()
    {

    }
}