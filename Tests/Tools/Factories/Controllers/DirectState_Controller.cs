using Factors.Cores.ProactiveCores;

namespace Tests.Tools.Factories.Controllers
{
    public class DirectState_Controller : Factor_T_Controller<DirectProactiveCore<int>, int>
    {
        public override int ChangeValueToANonEqualValue()
        {
            var previousValue = ControlledInstance.Value;
            var newValue      = Tools.GenerateRandomIntNotEqualTo(previousValue);

            return ControlledInstance.Value = newValue;
        }
        
        public override int SetValueToAnEqualValue()
        {
            var previousValue = ControlledInstance.Value;
            
            return ControlledInstance.Value = previousValue;
        }
        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);


        public DirectState_Controller()
        {
            ControlledInstance = new DirectProactiveCore<int>(Tools.GenerateRandomInt());
        }
    }
}