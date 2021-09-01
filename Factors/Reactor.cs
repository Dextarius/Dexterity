using System;
using System.Diagnostics;
using Causality;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;
using static Core.InterlockedUtils;
using static Core.Tools.Delegates;
using static Core.Tools.Types;



namespace Factors
{
    /// <summary>
    ///     * The Outcome maintains a strong reference to any Influences that it was affected by.
    ///     * Eventually, once the parent object is no longer referenced, it will naturally be collected.
    ///     * The Influence mentioned before will still have a direct reference to the Outcome, but the only thing keeping
    ///       the Influence alive is the Outcome itself.  Influences are not directly referenced by anything.
    ///     * This means they 
    /// </summary>
    public abstract class Reactor : Factor, IReactor, INotifiable
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

        protected readonly object syncLock = new object();
        protected          int    settings = defaultSettings;

        #endregion

        #region Properties

        public override bool IsConsequential => Outcome.IsConsequential;
        public          bool IsValid         => Outcome.IsValid;

        public bool IsUpdating
        {
                      get => (settings & Updating) == Updating;
            protected set
            {
                if (value) { CompareExchangeUntilMaskAdded(  ref settings, Updating); }
                else       { CompareExchangeUntilMaskRemoved(ref settings, Updating); }
            }
        }
        
        public bool IsReflexive
        {
            get => Outcome.HasCallback;
            set
            {
                if (value) { Outcome.SetCallback(this); }
                else       { Outcome.DisableCallback(); }
            }
        }

        [NotNull] protected abstract IOutcome Outcome { get; }

        #endregion

        #region Static Methods

        protected static string CreateDefaultName<TReactor>(Delegate functionToCreateValue) => 
            NameOf<TReactor>() + GetClassAndMethodName(functionToCreateValue);

        #endregion
        
        #region Instance Methods

        public void React()
        {
            if (Outcome.IsInvalid)
            {
                lock (syncLock)
                {
                    Debug.Assert(IsUpdating == false, "A reactor is in an update loop");

                    if (Outcome.IsInvalid)
                    {
                        IUpdateQueue updateQueue = UpdateHandler.RequestQueuing();

                        IsUpdating = true;
                        Act();
                        IsUpdating = false;
                        updateQueue?.EndQueue()?.Invoke();
                    }
                }
            }
        }
        
        public void Invalidate() => Outcome.Invalidate();

        protected abstract bool Act();

        #endregion


        #region Constructors

        protected Reactor(string nameToGive) : base(nameToGive)
        {
            
        }

        #endregion

        void INotifiable.Notify() => React();
    }
}