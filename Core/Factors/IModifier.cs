namespace Core.Factors
{
    public interface IModifier<T> : IModifierBase<T>, IFactor
    {
        
    }
    
    public interface IModifierBase<T> 
    {
        int ModPriority { get; set; }

        T Modify(T valueToModify);
    }
}