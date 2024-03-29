using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using Dextarius.Collections;

namespace Factors.Cores
{
    public class ModifierCollectionCore<T> : ReactorCore, ICollectionCore<IModifier<T>>,
        ICollection<IModifier<T>>
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

        public bool Remove(IModifier<T> modifierToRemove)
        {
            if (modifiers != null &&
                modifierToRemove != null &&
                modifiers.Remove(modifierToRemove))
            {
                RemoveTrigger(modifierToRemove);
                Trigger();
                return true;
            }
            else return false;
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

        public void CopyTo(IModifier<T>[] array, int arrayIndex) => modifiers.CopyTo(array, arrayIndex);

        public bool CollectionEquals(ICollection<IModifier<T>> collectionToCompare)
        {
            if (collectionToCompare.Count != this.Count)
            {
                return false;
            }
            else
            {
                foreach (var modifier in collectionToCompare)
                {
                    if (modifiers.Contains(modifier) is false)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        
        public bool CollectionEquals(IEnumerable<IModifier<T>> collectionToCompare)
        {
            var listToCompare = new List<IModifier<T>>(collectionToCompare);

            return modifiers.IsEquivalentTo(listToCompare, EqualityComparer<IModifier<T>>.Default);
        }

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

        bool ICollection<IModifier<T>>.IsReadOnly => false;
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        
        //- Note : If a mod changes its mod priority while in our collection we won't know.
    }
}