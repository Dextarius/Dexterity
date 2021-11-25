using System;
using Causality.States;
using Core.Causality;
using Core.Factors;

namespace Tests.Causality.Factories
{
    public class Result_Int_Factory : Result_T_Factory<int>
    {
        private Random numberGenerator = new Random();
        
        public override int CreateRandomInstanceOfValuesType() => numberGenerator.Next();
        public override int CreateRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) 
        {
            int number = numberGenerator.Next();

            while (number == valueToAvoid)
            {
                number = numberGenerator.Next();
            }

            return number;
        }



    }
}