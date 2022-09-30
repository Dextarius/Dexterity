using Core.Factors;
using Core.States;
using Factors.Cores;

namespace Factors.Modifiers
{
    public abstract class FactorModifier<T> : Factor<IFactorCore>, IFactorModifier<T>
    {
        #region Instance Fields

        private bool isEnabled;

        #endregion
        

        #region Properties

        public string Description { get; }
        public int    ModPriority { get; set; }
        
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    TriggerSubscribers();
                }
            }
        }

        #endregion

        
        #region Instance Methods

        public virtual int CompareTo(IFactorModifier<T> other)
        {
            if (this.ModPriority < other.ModPriority)
            {
                return -1;
            }
            else if (this.ModPriority == other.ModPriority)
            {
                return 0;
            }
            else return 1;
        }

        public T Modify(T valueToModify)
        {
            if (IsEnabled)
            {
                return ModifyValue(valueToModify);
            }
            else return valueToModify;
        }
        
        public abstract T ModifyValue(T valueToModify);

        #endregion


        #region Constructors

        protected FactorModifier(IFactorCore factorCore, string factorsName = nameof(FactorModifier<T>)) : 
            base(factorCore, factorsName)
        {
        }

        #endregion
    }

    public static class FactorModifier
    {
        public static int SortByModPriority<T>(this IFactorModifier<T> factor1, IFactorModifier<T> factor2)
        {
            if (factor1.ModPriority < factor2.ModPriority)
            {
                return -1;
            }
            else if (factor1.ModPriority == factor2.ModPriority)
            {
                return 0;
            }
            else return 1;
        }
    }
}