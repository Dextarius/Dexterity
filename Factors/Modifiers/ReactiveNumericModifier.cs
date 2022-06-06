using Core.Factors;

namespace Factors.Modifiers
{
    public class ReactiveNumericModifier : Reactor<IReactiveNumericModifierCore>, INumericMod
    {
        private bool isEnabled;

        #region Properties

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled != value)
                {
                    if (value is true)
                    {
                        
                    }
                    else
                    {
                        //- not necessary?
                    }
                    
                    isEnabled = value;
                    TriggerSubscribers();
                }
            }
        }


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

        public double Amount => core.Amount;

        public override bool IsNecessary => base.IsNecessary && IsEnabled;

        #endregion
        


        #region Constructors

        public ReactiveNumericModifier(IReactiveNumericModifierCore factorCore, string modifiersName = nameof(ProactiveNumericModifier)) : 
            base(factorCore, modifiersName)
        {
            
        }

        #endregion
    }
}