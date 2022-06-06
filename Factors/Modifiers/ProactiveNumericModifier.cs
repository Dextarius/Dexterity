using Core.Factors;

namespace Factors.Modifiers
{
    public class ProactiveNumericModifier : Factor<IProactiveNumericModifierCore>, INumericMod
    {
        #region Properties

        public NumericModType ModType
        {
            get => core.ModType;
            set
            {
                if (core.ModType != value)
                {
                    core.ModType = value;
                    TriggerSubscribers();
                }
            }
        }

        public int ModPriority
        {
            get => core.ModPriority;
            set
            {
                if (core.ModPriority != value)
                {
                    core.ModPriority = value;
                    TriggerSubscribers();
                }
            }
        }

        public double Amount
        {
            get => core.Amount;
            set
            {
                if (core.SetAmount(value))
                {
                    TriggerSubscribers();
                }
            }
        }

        #endregion


        #region Constructors

        public ProactiveNumericModifier(IProactiveNumericModifierCore factorCore, string modifiersName = nameof(ProactiveNumericModifier)) : 
            base(factorCore, modifiersName)
        {
            
        }

        #endregion
    }
}