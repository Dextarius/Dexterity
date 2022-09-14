using Core.Factors;
using Core.States;
using Factors.Cores;
using JetBrains.Annotations;

namespace Factors
{
    public class Modifiable<T> : ReactiveValue<T, ModifiableCore<T>>, IProactive<T>
    {
        #region Instance Fields

        protected IProactive<T> baseValueRedirector;

        #endregion


        #region Properties

        public T BaseValue
        {
            get => core.BaseValue;
            set => core.BaseValue = value;
        }

        #endregion


        #region Instance Methods

        public bool BaseValueEquals(T valueToCompare) => core.BaseValueEquals(valueToCompare);

        public IProactive<T> GetBaseValueAsFactor() => baseValueRedirector ??= new BaseValueRedirector(this);

        #endregion


        #region Constructors

        public Modifiable([NotNull] ModifiableCore<T> valueSource, string name = null) : base(valueSource, name)
        {
        }

        public Modifiable(T initialValue, string name = null) : this(new ModifiableCore<T>(initialValue), name)
        {
        }

        #endregion


        #region Explicit Implementations

        T IValue<T>.Value => Value;

        T IProactive<T>.Value
        {
            get => Value;
            set => BaseValue = value;
        }

        #endregion


        #region Nested Classes

        protected class BaseValueRedirector : IProactive<T>
        {
            private readonly Modifiable<T> modifiable;

            public T Peek() => modifiable.Peek();

            public string Name           => modifiable.Name;
            public int    UpdatePriority => modifiable.UpdatePriority;
            public uint   VersionNumber  => modifiable.VersionNumber;
            public T      Value          => modifiable.BaseValue;
            
            public bool Subscribe(IFactorSubscriber subscriberToAdd, bool isNecessary) => modifiable.Subscribe(subscriberToAdd, isNecessary);
            public void Unsubscribe(IFactorSubscriber subscriberToRemove)              => modifiable.Unsubscribe(subscriberToRemove);
            public void NotifyNecessary(IFactorSubscriber necessarySubscriber)         => modifiable.NotifyNecessary(necessarySubscriber);
            public void NotifyNotNecessary(IFactorSubscriber unnecessarySubscriber)    => modifiable.NotifyNotNecessary(unnecessarySubscriber);
            public bool Reconcile()                                                    => modifiable.Reconcile();
            public bool ValueEquals(T valueToCompare)                                  => modifiable.BaseValueEquals(valueToCompare);


            public BaseValueRedirector(Modifiable<T> modifiableToProxy)
            {
                modifiable = modifiableToProxy;

            }

            T IProactive<T>.Value
            {
                get => modifiable.BaseValue;
                set => modifiable.BaseValue = value;
            }
        }

        #endregion
    }
}