using Core.States;

namespace Tests.Tools.Interfaces
{
    
    public interface ITestableConstructor_Name<out TTested> 
        where TTested : INameable
    {
        TTested[] CallAllConstructors_AndPassName(string nameToUse);
    }
}