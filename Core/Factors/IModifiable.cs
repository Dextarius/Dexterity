﻿namespace Core.Factors
{
    public interface IModifiable<T> : IFactor<T>
    {
        void   AddModifier(IFactorModifier<T> modifierToAdd);
        void   RemoveModifier(IFactorModifier<T> modifierToRemove);
        bool   ContainsModifier(IFactorModifier<T> modifierToFind);
        string PrintBaseValueAndModifiers();
    }
}