using System.Collections.Generic;
using Factors;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using static Tests.Class_Tests.Cores.Shared;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectProactorCores
{
    public class DirectStateCores : ITestableConstructor_Value<DirectProactiveCore<int>,  int> 
    {
        public DirectProactiveCore<int>[] CallAllConstructors_AndPassValue(int valueToUse)
        {
            return new[]
            {
                new DirectProactiveCore<int>(valueToUse),
                new DirectProactiveCore<int>(valueToUse, EqualityComparer<int>.Default)
            };
        }
        
        public int CreateRandomValue() => GenerateRandomInt();

        public int CreateRandomValueNotEqualTo(int valueToAvoid) => GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public DirectProactiveCore<int> CreateInstance() => new DirectProactiveCore<int>(1);
        
        public DirectProactiveCore<int> CreateStableInstance() => CreateInstance();
    }
}