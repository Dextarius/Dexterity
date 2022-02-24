namespace Tests.Tools.Factories
{
    public class ObservedStateCore_Int_Factory : ObservedStateCore_T_Factory<int>
    {
        public override int CreateRandomValueNotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public override int CreateRandomValue() => Tools.GenerateRandomInt();
    }
}