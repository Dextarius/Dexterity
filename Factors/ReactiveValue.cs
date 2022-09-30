using System;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;

namespace Factors
{
    public abstract class ReactiveValue<TValue, TCore> : Reactor<TCore>, IReactive<TValue> 
        where TCore : IResult<TValue>
    {
        #region Constants

        protected string CannotUseNullFunction =
            "A Reactive cannot be constructed with a null delegate, as it would never have a value. ";

        #endregion
        
        
        #region Instance Properties

        public TValue Value
        {
            get
            {
                AttemptReaction();
                return core.Value;
            }
        }
        
        public IModifierCollection<TValue> Modifiers => core.Modifiers;

        #endregion


        #region Instance Methods

        public TValue Peek() => core.Peek();

        public bool ValueEquals(TValue valueToCompare) => core.ValueEquals(valueToCompare);
        
        public override string ToString() => $"{Name} => {Value}";
        
        public override bool CoresAreNotEqual(TCore oldCore, TCore newCore) => newCore.ValueEquals(oldCore.Value) is false;

        public override void SwapCore(TCore newCore) { }

        #endregion

        
        #region Operators

        public static implicit operator TValue(ReactiveValue<TValue, TCore> reactive) => reactive.Value;

        #endregion
        
        
        #region Constructors

        protected ReactiveValue([NotNull] TCore valueSource, string name = null) : base(valueSource, name)
        {
        }

        #endregion
    }
}