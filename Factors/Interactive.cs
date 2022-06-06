using Core.Factors;
using Core.States;
using Factors.Cores;

namespace Factors
{
    public class Interactive<T> : Reactor<IInteractiveCore<T>>, IInteractive<T>
    {
        #region Properties
        
        public T Value
        {
            get
            {
                AttemptReaction();
                return core.Value;
            }
        }

        public T BaseValue
        {
            get => core.BaseValue;
            set
            {
                core.BaseValue = value;
                AttemptReaction();
            }
        }

        #endregion

        
        #region Instance Methods

        public bool ContainsModifier(IFactorModifier<T> modifierToFind) => core.ContainsModifier(modifierToFind);
        public void AddModifier(IFactorModifier<T> modifierToAdd)       => core.AddModifier(modifierToAdd);
        public void RemoveModifier(IFactorModifier<T> modifierToRemove) => core.RemoveModifier(modifierToRemove);

        public string PrintBaseValueAndModifiers() => core.PrintBaseValueAndModifiers();

        #endregion
        
        #region Operators

        public static implicit operator T(Interactive<T> interactive) => interactive.Value;

        #endregion
        
        
        //- We could add a mechanic to Factors that lets you add 'extensions'
        //  that will run at the end of the update process.
        //  We may be able to format some of the other mechanics as extensions, like Notifying the Observers
        //  when a Factor is involved/changed        
        
        
        #region Constructors

        public Interactive(IInteractiveCore<T> coreToUse, string nameToGive) : base(coreToUse, nameToGive)
        {
            
        }
        
        public Interactive(T initialBaseValue, string nameToGive = null) : 
            base(new InteractiveCore<T>(initialBaseValue), nameToGive)
        {
            
        }

        #endregion
    }
}