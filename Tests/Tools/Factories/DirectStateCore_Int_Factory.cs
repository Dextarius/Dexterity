namespace Tests.Tools.Factories
{
    public class DirectStateCore_Int_Factory : DirectStateCore_T_Factory<int>
    {
        public override int CreateRandomValueNotEqualTo(int valueToAvoid) => 
            Tools.GenerateRandomIntNotEqualTo(valueToAvoid);
        
        public override int CreateRandomValue() => Tools.GenerateRandomInt();
    }
}