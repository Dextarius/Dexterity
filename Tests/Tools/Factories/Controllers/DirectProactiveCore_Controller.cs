using Factors;
using Factors.Cores.ProactiveCores;

namespace Tests.Tools.Factories.Controllers
{
    public class DirectProactiveCore_Controller : ProactiveCore_Controller<DirectProactiveCore<int>, int>
    {
        public override int ChangeValueToANonEqualValue()
        {
            var previousValue = ControlledInstance.Value;
            var newValue      = Tools.GenerateRandomIntNotEqualTo(previousValue);
            
            ControlledInstance.SetValueIfNotEqual(newValue);
            return newValue;
        }
        
        public override int SetValueToAnEqualValue()
        {
            var previousValue = ControlledInstance.Value;
            
            ControlledInstance.SetValueIfNotEqual(previousValue);

            return previousValue;
        }
        
        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public DirectProactiveCore_Controller() : base(new DirectProactiveCore<int>(Tools.GenerateRandomInt()))
        {
        }
    }
}