namespace Tests.Tools.Factories
{
    public class State_Int_Factory : State_T_Factory<int>
    {
        public override int CreateRandomValueNotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public override int CreateRandomValue() => Tools.GenerateRandomInt();
    }
}