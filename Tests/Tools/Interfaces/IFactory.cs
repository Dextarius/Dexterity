namespace Tests.Tools.Interfaces
{
    public interface IFactory<out TClass, TValue> : IFactory<TClass>
    {
        TValue CreateRandomInstanceOfValuesType();
        TValue CreateRandomInstanceOfValuesType_NotEqualTo(TValue valueToAvoid);
    }
    
    public interface IFactory<out TClass>
    {
        TClass CreateInstance();
       // TClass CreateStableValidInstance();
    }
}