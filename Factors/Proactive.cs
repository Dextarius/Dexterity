using System.Collections.Generic;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Cores.ProactiveCores;
using JetBrains.Annotations;
using static Core.InterlockedUtils;
using static Core.Tools.Types;

namespace Factors
{
    public class Proactive<T> : Factor<IState<T>>, IState<T>
    {
        #region Properties
        
        public T Value
        {
            get => core.Value;
            set => core.Value = value;
        }
        
        public T Peek() => core.Peek();

        #endregion


        #region Instance Methods

        public override string ToString() => $"{Name} => {Value}";
        

        #endregion
        

        #region Operators

        public static implicit operator T(Proactive<T> proactive) => proactive.Value;

        #endregion


        #region Constructors

        public Proactive(IState<T> valueState, string name = null) : 
            base(valueState, name?? NameOf<Proactive<T>>())
        {

        }

        public Proactive(T initialValue, IEqualityComparer<T> comparer = null, string name = null) : 
            this(new ObservedStateCore<T>(initialValue, comparer), name?? NameOf<Proactive<T>>())
        {
        }
        
        public Proactive(T initialValue, string name) : this(initialValue, null, name)
        {
        }

        #endregion
    }
    
    
    // public class Proactive<T> : Proactor, IState<T>
    // {
    //     #region Instance Fields
    //
    //     protected readonly IState<T> state;
    //
    //     #endregion
    //
    //
    //     #region Properties
    //     
    //     public T Value
    //     {
    //         get => state.Value;
    //         set => state.Value = value;
    //     }
    //     
    //     public T Peek() => state.Peek();
    //
    //     protected override IFactor Influence => state;
    //
    //     #endregion
    //
    //
    //     #region Instance Methods
    //
    //     public override string ToString() => $"{Name} => {Value}";
    //     
    //
    //     #endregion
    //     
    //
    //     #region Operators
    //
    //     public static implicit operator T(Proactive<T> proactive) => proactive.Value;
    //
    //     #endregion
    //
    //
    //     #region Constructors
    //
    //     public Proactive(IState<T> valueState, string name) : 
    //         base(name?? NameOf<Proactive<T>>())
    //     {
    //         state = valueState;
    //     }
    //
    //     public Proactive(IState<T> valueState) : this(valueState, null)
    //     {
    //         
    //     }
    //     
    //     public Proactive(T initialValue, IEqualityComparer<T> comparer = null, string name = null) : 
    //         this(new ObservedState<T>(initialValue, comparer), name?? NameOf<Proactive<T>>())
    //     {
    //     }
    //     
    //     public Proactive(T initialValue, string name) : this(initialValue, null, name)
    //     {
    //     }
    //
    //     #endregion
    // }
}
