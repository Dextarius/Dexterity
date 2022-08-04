using Core.Factors;
using Core.Tools;
using Factors.Cores;

namespace Factors.Modifiers
{
    public class ProactiveModifierCore : FactorCore, IProactiveNumericModifierCore
    {
        public bool           IsEnabled   { get; set; }
        public NumericModType ModType     { get; set; }
        public int            ModPriority { get; set; } = 0;
        public double         Amount      { get; protected set; }
        
        public bool SetAmount(double newAmount)
        {
            if (Numerics.DoublesAreNotEqual(Amount, newAmount))
            {
                Amount = newAmount;
                VersionNumber++;
                return true;
            }
            else return false;
        }
        
        public void SetOwner(IFactor owner) { }

        public ProactiveModifierCore(NumericModType modType)
        {
            ModType = modType;

        }
    }
    
    public class ModifierCore : FactorCore
    {
        public bool           IsEnabled   { get; set; }
        public NumericModType ModType     { get; set; }
        public int            ModPriority { get; set; } = 0;
        public double         Amount      { get; protected set; }
        
        public bool SetAmount(double newAmount)
        {
            if (Numerics.DoublesAreNotEqual(Amount, newAmount))
            {
                Amount = newAmount;
                VersionNumber++;
                return true;
            }
            else return false;
        }
        
        public void SetOwner(IFactor owner) { }

        public ModifierCore(NumericModType modType)
        {
            ModType = modType;

        }
    }
}