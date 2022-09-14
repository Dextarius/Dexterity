using Core.States;

namespace Core.Factors
{
    public interface IModifiable<T> 
    {
        IModifierCollection<T> Modifiers { get; }
    }
}