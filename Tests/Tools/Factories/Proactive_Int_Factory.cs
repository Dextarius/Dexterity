namespace Tests.Tools.Factories
{
    public class Proactive_Int_Factory : Proactive_T_Factory<int>
    {
        public override int CreateRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tests.Tools.Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public override int CreateRandomInstanceOfValuesType() => Tests.Tools.Tools.GenerateRandomInt();
        
        // public override void ChangeValueTo(int newValue)
        // {
        //     manipulatedInstance.Value = newValue;
        // }
    }
}