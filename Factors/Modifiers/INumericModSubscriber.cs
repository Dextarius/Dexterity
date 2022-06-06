using Core.Factors;

namespace Factors.Modifiers
{
    public interface INumericModSubscriber
    {
        void    ValueChanged(INumericMod modifier, double oldValue, double newValue);
        void  ModTypeChanged(INumericMod modifier, NumericModType oldType, NumericModType newType);
        void PriorityChanged(INumericMod modifier, int oldPriority, int newPriority);
    }
    
    //- If we decide the NumericModifiers aren't performing well we can always use this.
    //  Having the mod directly communicate its changes should simplify the ModifiableNumber's
    //  changes.  We'll have to consider that regular factors won't be able subscribe to
    //  mods that implement this though.
}