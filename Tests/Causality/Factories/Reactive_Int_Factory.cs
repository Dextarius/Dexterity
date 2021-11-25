using System;

namespace Tests.Causality.Factories
{
    public class Reactive_Int_Factory : Reactive_Factory<int>
    {
        private readonly Random numberGenerator = new Random();
        
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