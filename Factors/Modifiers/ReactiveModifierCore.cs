using Core.Factors;
using Factors.Cores;
using Factors.Cores.DirectReactorCores;

namespace Factors.Modifiers
{
    public abstract class ReactiveModifierCore : DirectReactorCore, IReactiveNumericModifierCore
    {
        public            bool           IsEnabled   { get; set; }
        public            NumericModType ModType     { get; set; }
        public            int            ModPriority { get; set; }
        public   abstract double         Amount      { get; }
    }
}