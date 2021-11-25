namespace Tests.Causality.Interfaces
{
    public interface IRandomGenerator<T>
    {
        public T CreateRandomValue();
        public T CreateRandomValueNotEqualTo(T valueToAvoid);
    }
}