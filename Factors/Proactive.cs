using System.Collections.Generic;
using Core.Causality;
using Core.States;
using Factors.Cores.ProactiveCores;
using JetBrains.Annotations;
using static Core.Tools.Types;

namespace Factors
{
    public class Proactive<T> : Proactor<IProactiveCore<T>>, IProactive<T>
    {
        #region Properties
        
        public T Value
        {
            get => core.Value;
            set => core.SetValueIfNotEqual(value);
        }

        #endregion


        #region Instance Methods

        public bool ValueEquals(T valueToCompare) => core.ValueEquals(valueToCompare);

        public T Peek() => core.Peek();

        public override string ToString() => $"{Name} => {Value}";

        public override bool CoresAreNotEqual(IProactiveCore<T> oldCore, IProactiveCore<T> newCore) => 
            newCore.ValueEquals(oldCore.Value) is false;

        #endregion
        

        #region Operators

        public static implicit operator T(Proactive<T> proactive) => proactive.Value;

        #endregion


        #region Constructors

        public Proactive(IProactiveCore<T> core, string name = null) : base(core, name?? NameOf<Proactive<T>>())
        {
        }

        public Proactive(T initialValue, IEqualityComparer<T> comparer = null, string name = null) : 
            this(new DirectProactiveCore<T>(initialValue, comparer), name?? NameOf<Proactive<T>>())
        {
        }
        
        public Proactive(T initialValue, string name) : this(initialValue, null, name)
        {
        }

        #endregion
    }
}
