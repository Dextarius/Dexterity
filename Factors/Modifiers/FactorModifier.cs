using Core.Factors;
using Core.States;
using Factors.Cores;

namespace Factors.Modifiers
{
    public abstract class FactorModifier<T> : FactorCore, IFactorModifier<T>
    {
        private bool isEnabled;

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