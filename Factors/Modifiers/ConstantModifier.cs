using Core.Factors;

namespace Factors.Modifiers
{
    public abstract class ConstantModifier<T> : ConstantFactor, IFactorModifier<T>
    {
        public abstract string Description { get; }
        public          int    ModPriority { get; set; }


        public virtual int CompareTo(IFactorModifier<T> other) => this.SortByModPriority(other);

        public abstract T Modify(T valueToModify);


        protected ConstantModifier(string name) : base(name)
        {
        }
    }
    
    public abstract class AddConstantModifier<T> : ConstantModifier<T>, IFactorModifier<T>
    {
        public readonly T valueToAdd;

        
        public override string Description => $"{Name} (Add {valueToAdd}) ";


        protected AddConstantModifier(string name, T valueToAdd) : base(name)
        {
            this.valueToAdd = valueToAdd;

        }
    }

    
    public class AddConstantIntModifier : AddConstantModifier<int>
    {
        public override int Modify(int valueToModify) => valueToModify + valueToAdd;
        
        public AddConstantIntModifier(int valueToAdd, string name = "AddConstantInt") : base(name, valueToAdd)
        {
        }
    }
    
    
    public class AddConstantUIntModifier : AddConstantModifier<uint>
    {
        public override uint Modify(uint valueToModify) => valueToModify + valueToAdd;
        
        public AddConstantUIntModifier(uint valueToAdd, string name = "AddConstantUInt") : base(name, valueToAdd)
        {
        }
    }
    
    
    public class AddConstantDoubleModifier : AddConstantModifier<double>
    {
        public override double Modify(double valueToModify) => valueToModify + valueToAdd;
        
        public AddConstantDoubleModifier(double valueToAdd, string name = "AddConstantDouble") : base(name, valueToAdd)
        {
        }
    }
}