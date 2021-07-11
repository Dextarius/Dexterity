namespace Core.Factors
{
    public interface IFactor
    {
        string Name            { get; }
        bool   IsConsequential { get; }
    }
}