using System.Collections;
using System.Collections.Generic;
using Core.Factors;
using Core.States;
using static Core.Tools.Types;

namespace Factors.Cores
{
    public class ModifierCollection<T> : Reactor<ModifierCollectionCore<T>>, IModifierCollection<T>
    {
        public int Count => core.Count;

        public void      Add(IModifier<T> modifierToAdd)    => core.Add(modifierToAdd);
        public void   Remove(IModifier<T> modifierToRemove) => core.Remove(modifierToRemove);
        public bool Contains(IModifier<T> modifierToFind)   => core.Contains(modifierToFind);

        public T Modify(T valueToModify) => core.Modify(valueToModify);
        
        public IEnumerator<IModifier<T>> GetEnumerator() => core.GetEnumerator();

        public ModifierCollection(string nameToGive = null) : 
            this(new ModifierCollectionCore<T>(), nameToGive)
        {
        }
        
        public ModifierCollection(ModifierCollectionCore<T> reactorCore, string nameToGive = null) : 
            base(reactorCore, nameToGive ?? NameOf<ModifierCollection<T>>())
        {
        }

        IEnumerator  IEnumerable.GetEnumerator() => GetEnumerator();
    }
}