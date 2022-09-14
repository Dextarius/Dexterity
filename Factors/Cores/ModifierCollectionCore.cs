using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;

namespace Factors.Cores
{
    public class ModifierCollectionCore<T> : ReactorCore, IEnumerable<IModifier<T>>
    {
        #region Instance Fields

        protected List<IModifier<T>> modifiers;
        private   int                updatePriority;

        #endregion


        #region Properties

        protected override IEnumerable<IFactor> Triggers         => modifiers;
        public override    bool                 HasTriggers      => modifiers.Count > 0;
        public override    int                  NumberOfTriggers => modifiers.Count;
        public             int                  Count            => modifiers.Count;
        public override    int                  UpdatePriority   => updatePriority;

        #endregion
        
        
        #region Instance Methods
        
        public void Add(IModifier<T> modifierToAdd)
        {
            if (modifierToAdd is null) { throw new ArgumentNullException(nameof(modifierToAdd)); }

            int indexForMod;

            if (modifiers is null)
            {
                modifiers   = new List<IModifier<T>>();
                indexForMod = 0;
            }
            else
            {
                indexForMod = FindIndexForMod(modifierToAdd);
            }
            
            modifiers.Insert(indexForMod, modifierToAdd);
            
            if (modifierToAdd.UpdatePriority > this.UpdatePriority)
            {
                updatePriority = modifierToAdd.UpdatePriority + 1;
            }
            
            AddTrigger(modifierToAdd, IsReflexive);
            Trigger();
        }
        
        
        protected int FindIndexForMod(IModifier<T> newMod)
        {
            int priorityForNewMod = newMod.ModPriority;
            
            for (int i = 0; i < modifiers.Count; i++)
            {
                int currentModsPriority = modifiers[i].ModPriority;
                
                if (currentModsPriority > priorityForNewMod)
                {
                    return i;
                }
            }
        
            return modifiers.Count;
        }
        
        public void Remove(IModifier<T> modifierToRemove)
        {
            if (modifiers != null        &&
                modifierToRemove != null && 
                modifiers.Remove(modifierToRemove))
            {
                RemoveTrigger(modifierToRemove);
                Trigger();
            }
        }
        
        public void Clear()
        {
            if (modifiers?.Count > 0)
            {
                foreach (var modifier in modifiers)
                {
                    RemoveTrigger(modifier);
                }
                
                modifiers.Clear();
                Trigger();
            }
        }
        
        public bool Contains(IModifier<T> modifierToFind) => modifiers.Contains(modifierToFind);
        
        public T Modify(T valueToModify)
        {
            T result = valueToModify;

            foreach (var modifier in modifiers)
            {
                result = modifier.Modify(result);
            }

            return result;
        }

        protected override long CreateOutcome() => TriggerFlags.Default;
        
        public IEnumerator<IModifier<T>> GetEnumerator()
        {
            if (modifiers != null) 
            {
                foreach (var modifier in modifiers)
                {
                    yield return modifier;
                }
            }
        }

        #endregion
        

        #region Explicit Implementations

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        
        //- Note : If a mod changes its mod priority while in our collection we won't know.
    }
}