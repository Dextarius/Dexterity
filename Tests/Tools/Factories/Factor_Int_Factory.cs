namespace Tests.Tools.Factories
{
    public class Factor_Int_Factory : Factor_T_Factory<int>
    {
        public override int CreateRandomInstanceOfValuesType_NotEqualTo(int valueToAvoid) => 
            Tests.Tools.Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public override int CreateRandomInstanceOfValuesType() => Tests.Tools.Tools.GenerateRandomInt();

        //public override void ChangeValueTo(int newValue)
        //{
        //    manipulatedInstance.Value = newValue;
        //}
    }
}