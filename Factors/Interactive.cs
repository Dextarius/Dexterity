using System;
using Core.Factors;
using Core.States;
using Factors.Cores;

namespace Factors
{
    public class Interactive<T> : Reactor<ValueModifier<T>>, IInteractive<T>
    {
        #region Properties
        
        public T Value => core.Value;

        public T BaseValue
        {
            get => core.BaseValue;
            set => core.BaseValue = value;
        }

        #endregion

        
        #region Instance Methods

        public void AddModifier(T modifierToAdd)
        {
            
        }
        
        public void RemoveModifier(T modifierToRemove)
        {
            
        }
        
        public bool ContainsModifier(T modifierToFind)
        {
            
        }

        #endregion
        
        
        //- We could add a mechanic to Factors that lets you add 'extensions'
        //  that will run at the end of the update process.
        //  We may be able to format some of the other mechanics as extensions, like Notifying the Observers
        //  when a Factor is involved/changed        
        
        
        #region Constructors

        protected Interactive(IResult<T> reactorCore, string nameToGive) : base(reactorCore, nameToGive)
        {
        }

        #endregion


        public T Peek()
        {
            
        }
    }

    public interface IInteractive<T> : IReactor<T>
    {
        void AddModifier(T modifierToAdd);
        void RemoveModifier(T modifierToRemove);
        bool ContainsModifier(T modifierToFind);
    }

    public interface IFactorModifier<T>
    {
        IXXX    ModifierChanged { get; }
        int         Priority        { get; }
        
        T Modify(T valueToModify);
    }


}