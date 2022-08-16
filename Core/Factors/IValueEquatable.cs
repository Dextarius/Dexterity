namespace Core.Factors
{
    public interface IValueEquatable<in T>
    {
        bool ValueEquals(T valueToCompare);
    }
}