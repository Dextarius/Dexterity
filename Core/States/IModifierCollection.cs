using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface IModifierCollection<T> : IEnumerable<IModifier<T>>
    {
        int Count { get; }
        
        void      Add(IModifier<T> modifierToAdd);
        void   Remove(IModifier<T> modifierToRemove);
        bool Contains(IModifier<T> modifierToFind);
        T      Modify(T             valueToModify);
    }
}