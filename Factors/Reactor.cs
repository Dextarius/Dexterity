using System;
using System.Diagnostics;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.Tools.Delegates;
using static Core.Tools.Types;


namespace Factors
{
    public abstract class Reactor<TCore> : Factor<TCore>, IReactor, IUpdateable 
        where TCore : IReactor
    {
        #region Instance Properties
        
        public bool HasTriggers      => core.HasTriggers;
        public int  NumberOfTriggers => core.NumberOfTriggers;
        public bool IsUnstable       => core.IsUnstable;
        public bool IsReacting       => core.IsReacting;
        public bool IsStabilizing    => core.IsStabilizing;
        public bool HasBeenTriggered => core.HasBeenTriggered;
        
        public WeakReference<IFactorSubscriber> WeakReference => core.WeakReference;

        public bool IsReflexive
        {
            get => core.IsReflexive;
            set => core.IsReflexive = value;
        }

        #endregion
        
        
        #region Static Methods

        protected static string CreateDefaultName<TReactor>(Delegate functionToCreateValue) => 
            $"{NameOf<TReactor>()} {GetClassAndMethodName(functionToCreateValue)}";

        protected static ArgumentNullException CannotConstructValueReactorWithNullProcess<T>()  
            where T : Reactor<TCore> =>
                new ArgumentNullException(
                    $"A {NameOf<T>()} cannot be constructed with a null process, as it would never have a value. ");

        protected static ArgumentNullException CannotConstructReactorWithNullProcess<T>()  
            where T : Reactor<TCore> =>
                new ArgumentNullException(
                    $"A {NameOf<T>()} cannot be constructed with a null process, as it would never do anything. ");

        #endregion
        

        #region Instance Methods

        
        //- Does not imply the caller will queue this Outcome to be updated.
        //  Only that the caller should be notified if this Outcome is Necessary
        //  and if not that it should mark itself and its dependents as Unstable
        public bool Destabilize()     => core.Destabilize();
        public bool ForceReaction()   => core.ForceReaction();
        public bool AttemptReaction() => core.AttemptReaction();
        public bool Trigger()         => core.Trigger();
        public bool Trigger(IFactor triggeringFactor, out bool removeSubscription) => 
            core.Trigger(triggeringFactor, out removeSubscription);

        #endregion

        
        #region Constructors

        protected Reactor(TCore reactorCore, string nameToGive) : base(reactorCore, nameToGive)
        {
            
        }

        #endregion
        
        
        #region Explicit Implementations
    
        void IUpdateable.Update() => AttemptReaction();
    
        #endregion

    }
}