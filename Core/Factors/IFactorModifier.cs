namespace Core.Factors
{
    public interface IFactorModifier<T>
    {
        IXXX ModifierChanged { get; }

        T Modify(T valueToModify);
    }
}