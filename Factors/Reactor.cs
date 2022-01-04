using System;
using System.Diagnostics;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.InterlockedUtils;
using static Core.Tools.Delegates;
using static Core.Tools.Types;


namespace Factors
{
    public abstract class Reactor : Factor, IReactor, IUpdateable
    {
        #region Instance Properties

        protected abstract IOutcome Outcome            { get; }
        public override    int      Priority           => Outcome.Priority;
        public             bool     IsUpdating         => Outcome.IsUpdating;
        public             bool     IsStable           => Outcome.IsStable;
        public             bool     IsStabilizing      => Outcome.IsStabilizing;
        public             bool     IsValid            => Outcome.IsValid;
        public             bool     IsUnstable         => Outcome.IsUnstable;
        public             bool     IsInvalid          => Outcome.IsInvalid;
        public override    bool     HasDependents      => Outcome.HasDependents;
        public override    int      NumberOfDependents => Outcome.NumberOfDependents;
        public override    bool     IsNecessary        => Outcome.IsNecessary;
        public             bool     IsBeingInfluenced  => Outcome.IsBeingInfluenced;
        public             int      NumberOfInfluences => Outcome.NumberOfInfluences;

        public bool IsReflexive
        {
            get => Outcome.IsReflexive;
            set => Outcome.IsReflexive = value;
        }

        #endregion
        
        
        #region Static Methods

        protected static string CreateDefaultName<TReactor>(Delegate functionToCreateValue) => 
            $"{NameOf<TReactor>()} {GetClassAndMethodName(functionToCreateValue)}";

        protected static ArgumentNullException CannotConstructValueReactorWithNullProcess<T>()  where T : Reactor =>
            new ArgumentNullException(
                $"A {NameOf<T>()} cannot be constructed with a null process, as it would never have a value. ");

        protected static ArgumentNullException CannotConstructReactorWithNullProcess<T>()  where T : Reactor =>
            new ArgumentNullException(
                $"A {NameOf<T>()} cannot be constructed with a null process, as it would never do anything. ");

        #endregion
        

        #region Instance Methods

        
        //- Does not imply the caller will queue this Outcome to be updated.
        //  Only that the caller should be notified if this Outcome is Necessary
        //  and if not that it should mark itself and its dependents as Unstable
        public          bool Destabilize()                                 => Outcome.Destabilize();
        public          bool React()                                       => Outcome.Generate();
        public          bool Stabilize()                                   => Outcome.Stabilize();
        public override bool AddDependent(IDependent dependentToAdd)       => Outcome.AddDependent(dependentToAdd);
        public override void RemoveDependent(IDependent dependentToRemove) => Outcome.RemoveDependent(dependentToRemove);
        public          bool Invalidate()                                  => Outcome.Invalidate();
        public          bool Invalidate(IFactor changedFactor)             => Outcome.Invalidate(changedFactor);
        public override void InvalidateDependents()                        => Outcome.InvalidateDependents();
        public override void NotifyNecessary()                             => Outcome.NotifyNecessary();
        public override void NotifyNotNecessary()                          => Outcome.NotifyNotNecessary();
        public override bool Reconcile()                                   => Outcome.Reconcile();

        #endregion

        
        #region Constructors

        protected Reactor(string nameToGive) : base(nameToGive)
        {
            
        }

        #endregion
        
        
        #region Explicit Implementations
    
        void IUpdateable.Update() => Stabilize();
    
        #endregion
    }
}