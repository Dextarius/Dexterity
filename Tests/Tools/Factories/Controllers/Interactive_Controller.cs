using Factors.Cores;

namespace Tests.Tools.Factories.Controllers
{
    public class Interactive_Controller : Factor_T_Controller<InteractiveCore<int>, int>
    {
        public override int ChangeValueToANonEqualValue()
        {
            var previousValue = ControlledInstance.BaseValue;
            var newValue      = Tools.GenerateRandomIntNotEqualTo(previousValue);

            return ControlledInstance.BaseValue = newValue;
        }
        
        public override int SetValueToAnEqualValue() => ControlledInstance.BaseValue = ControlledInstance.BaseValue;

        public override int GetRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public Interactive_Controller()
        {
            ControlledInstance = new InteractiveCore<int>(Tools.GenerateRandomInt());
        }
    }
}