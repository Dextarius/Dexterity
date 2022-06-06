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
    public class Proactive<T> : Factor<IProactiveCore<T>>, IState<T>
    {
        #region Properties
        
        public T Value
        {
            get => core.Value;
            set
            {
                if (core.ChangeValueTo(value))
                {
                    TriggerSubscribers();
                }
            }
        }

        #endregion


        #region Instance Methods

        public override string ToString() => $"{Name} => {Value}";
        

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
    //     public Proactive(IState<T> core, string name) : 
    //         base(name?? NameOf<Proactive<T>>())
    //     {
    //         state = core;
    //     }
    //
    //     public Proactive(IState<T> core) : this(core, null)
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
