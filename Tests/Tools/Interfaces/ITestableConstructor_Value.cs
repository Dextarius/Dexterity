using Core.Factors;
using Core.Redirection;

namespace Tests.Tools.Interfaces
{
    public interface ITestableConstructor_Value<out TTested, TValue> : IFactory<TTested>, IRandomGenerator<TValue>
        where TTested : IValue<TValue>
    {
        TTested[] CallAllConstructors_AndPassValue(TValue valueToUse);
    }
}