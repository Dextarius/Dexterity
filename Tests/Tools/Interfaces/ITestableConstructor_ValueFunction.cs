using Core.Factors;

namespace Tests.Tools.Interfaces
{
    public interface ITestableConstructor_ValueFunction<out TTested, in TValue> 
        where TTested : IFactor<TValue>
    {
        TTested[] CallAllConstructors_AndPassFunction(TValue valueToUse);
    }
}