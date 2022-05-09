using System.Collections.Generic;
using Factors;
using Factors.Cores.ProactiveCores;
using NUnit.Framework;
using Tests.Tools.Interfaces;
using static Tests.Class_Tests.Cores.Shared;
using static Tests.Tools.Tools;

namespace Tests.Class_Tests.Cores.DirectProactorCores
{
    public class DirectStateCores : ITestableConstructor_Name<DirectStateCore<int>>,
                                    ITestableConstructor_Value<DirectStateCore<int>,  int> 
    {
        public DirectStateCore<int>[] CallAllConstructors_AndPassName(string nameToUse)
        {
            return new[]
            {
                new DirectStateCore<int>(0, nameToUse)
            };
        }
        public DirectStateCore<int>[] CallAllConstructors_AndPassValue(int valueToUse)
        {
            return new[]
            {
                new DirectStateCore<int>(valueToUse),
                new DirectStateCore<int>(valueToUse, EqualityComparer<int>.Default)
            };
        }
        
        public int CreateRandomValue() => GenerateRandomInt();

        public int CreateRandomValueNotEqualTo(int valueToAvoid) => GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public DirectStateCore<int> CreateInstance() => new DirectStateCore<int>(1);
        
        public DirectStateCore<int> CreateStableInstance() => CreateInstance();
    }
}