using Core.Factors;

namespace Factors.Modifiers
{
    public interface IProactiveNumericModifierCore : INumericModCore
    {
        public bool SetAmount(double newAmount);
    }
    
    public interface IReactiveNumericModifierCore : INumericModCore, IReactorCore
    {
        new NumericModType ModType     { get; set; }
        new int            ModPriority { get; set; }
        
        
    }
}