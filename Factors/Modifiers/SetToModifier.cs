using Core.Factors;

namespace Factors.Modifiers
{
    public class SetToModifier<T> : FactorModifier<T>
    {
        public T ValueToSet { get; set; }

        public override T ModifyValue(T valueToModify) => ValueToSet;

        public override bool CoresAreNotEqual(IFactorCore oldCore, IFactorCore newCore) => false;

        public SetToModifier(T valueToSet, IFactorCore coreToUse) : base(coreToUse, nameof(SetToModifier<T> ))
        {
            ValueToSet = valueToSet;
        }
    }
}