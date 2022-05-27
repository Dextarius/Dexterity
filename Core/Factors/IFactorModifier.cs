using System;

namespace Core.Factors
{
    public interface IFactorModifier<T> : IFactor, IComparable<IFactorModifier<T>>
    {
        string Description { get; }
        int    ModPriority { get; }
        
        T      Modify(T valueToModify);

        //- Should we add a method so that modifiers can undo their changes?
        //  Since modifiers are executed in Priority order I don't think it would work right.
        //  T Unmodify(T modifiedValue);
    }
    
}