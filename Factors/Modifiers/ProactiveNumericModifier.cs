using Core.Factors;
using static Core.Tools.Numerics;

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
                    OnUpdated();
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
    
    public class NumericModifier : Factor<IProactiveNumericModifierCore>, INumericMod
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
            get
            {
                if (IsEnabled)
                {
                    return core.Amount;
                }
                else return 0;
            }
            set
            {
                if (core.SetAmount(value))
                {
                    TriggerSubscribers();
                }
            }
        }

        public bool IsEnabled
        {
            get => core.IsEnabled;
            set
            {
                if (core.IsEnabled != value)
                {
                    core.IsEnabled = value;

                    if (DoublesAreNotEqual(core.Amount, 0))
                    {
                        TriggerSubscribers();
                    }
                }
            }
        }

        #endregion


        #region Constructors

        public NumericModifier(
            IProactiveNumericModifierCore factorCore, string modifiersName = nameof(ProactiveNumericModifier)) : 
                base(factorCore, modifiersName)
        {
            
        }

        #endregion
    }
}