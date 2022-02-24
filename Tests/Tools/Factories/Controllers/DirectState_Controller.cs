using Factors.Cores.ProactiveCores;

namespace Tests.Tools.Factories.Controllers
{
    public class DirectState_Controller : Factor_T_Controller<DirectStateCore<int>, int>
    {
        public override int ChangeValueToANonEqualValue()
        {
            var previousValue = ControlledInstance.Value;
            var newValue      = Tools.GenerateRandomIntNotEqualTo(previousValue);

            return ControlledInstance.Value = newValue;
        }
        
        public override int ChangeValueToAnEqualValue()
        {
            var previousValue = ControlledInstance.Value;
            
            return ControlledInstance.Value = previousValue;
        }


        public DirectState_Controller()
        {
            ControlledInstance = new DirectStateCore<int>(Tools.GenerateRandomInt());
        }
    }
}