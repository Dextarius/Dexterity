using Factors;
using Factors.Cores.ProactiveCores;

namespace Tests.Tools.Factories.Controllers
{
    public class ObservedState_Controller : Factor_T_Controller<Proactive<int>, int>
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


        public ObservedState_Controller()
        {
            var core = new ObservedProactiveCore<int>(Tools.GenerateRandomInt());
            
            ControlledInstance = new Proactive<int>(core);
            core.SetOwner(ControlledInstance);
        }
    }
}