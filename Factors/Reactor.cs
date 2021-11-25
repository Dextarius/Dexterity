using System;
using System.Diagnostics;
using Causality;
using Core.Causality;
using Core.Factors;
using Core.States;
using JetBrains.Annotations;
using static Core.InterlockedUtils;
using static Core.Tools.Delegates;
using static Core.Tools.Types;



namespace Factors
{
    public abstract class Reactor : Factor, IResult
    {
        #region Static Fields

        protected static int defaultSettings = 0;

        protected const int Updating            = 0b0000_0010;
        protected const int Reflexive           = 0b0000_0100;
        protected const int PotentiallyInvalid  = 0b0000_0000;
        protected const int Executing           = 0b0000_0000;
        protected const int Exclusive           = 0b0000_0000;
        protected const int Disposed            = 0b0000_0000;
        protected const int Patient             = 0b0000_0000;
        protected const int Recursive           = 0b0000_0000;
        protected const int ThreadSafe          = 0b0000_0000;

        #endregion


        #region Instance Fields

        protected int settings = defaultSettings;

        #endregion

        #region Properties

        [NotNull] 
        protected abstract IResult Result { get; }

        public          bool IsStable           => Result.IsStable;
        public          bool IsValid            => Result.IsValid;
        public          bool IsBeingInfluenced  => Result.IsBeingInfluenced;
        public          int  NumberOfInfluences => Result.NumberOfInfluences;
        public          bool IsUpdating         => Result.IsUpdating;
        public override bool HasDependents      => Result.HasDependents;

        public bool IsReflexive
        {
            get => Result.IsReflexive;
            set => Result.IsReflexive = value;
        }

        #endregion

        #region Static Methods

        protected static string CreateDefaultName<TReactor>(Delegate functionToCreateValue) => 
            NameOf<TReactor>() + GetClassAndMethodName(functionToCreateValue);

        protected static ArgumentNullException CannotConstructValueReactorWithNullProcess<T>()  where T : Reactor =>
            new ArgumentNullException(
                $"A {NameOf<T>()} cannot be constructed with a null process, as it would never have a value. ");

        protected static ArgumentNullException CannotConstructReactorWithNullProcess<T>()  where T : Reactor =>
            new ArgumentNullException(
                $"A {NameOf<T>()} cannot be constructed with a null process, as it would never do anything. ");

        #endregion
        
        
        #region Instance Methods

        public bool Invalidate(IInfluence influenceThatChanged) => Result.Invalidate(influenceThatChanged);
        public void Invalidate()                                => Result.Invalidate(null);
        public bool Destabilize()                               => Result.Destabilize();
        public bool React()                                     => Result.React();
        
        protected override IFactor GetFactorImplementation() => Result;
        //protected abstract IInteraction GetInteractionImplementation();


        #endregion


        #region Constructors

        protected Reactor(string nameToGive) : base(nameToGive)
        {
        }

        #endregion

        
        #region Explicit Implementations

       //WeakReference<IInteraction> IInteraction.WeakReference => GetInteractionImplementation().WeakReference;

       //void IInfluenceable.Notify_InfluencedBy(IInfluence influence) => 
       //    GetInteractionImplementation().Notify_InfluencedBy(influence);
       //
       //bool IUpdateable.Update() => GetInteractionImplementation().Update();

        #endregion
    }
}