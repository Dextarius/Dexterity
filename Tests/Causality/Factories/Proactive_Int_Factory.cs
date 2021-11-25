using System;

namespace Tests.Causality.Factories
{
    public class Proactive_Int_Factory : Proactive_T_Factory<int>
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
        
        public override void ChangeValueTo(int newValue)
        {
            manipulatedInstance.Value = newValue;
        }
    }
}