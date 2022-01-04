using System;
using Core.Causality;
using Core.Factors;
using Core.States;
using Factors.Outcomes.ObservedOutcomes;

namespace Tests.Tools.Mocks
{
    public class MockXXXX : IDependent
    {
        #region Properties
    
        public WeakReference<IDependent> WeakReference { get; } 
    
        public bool     WasInfluenced         { get; private set; }
        public bool     IsStable              { get; private set; }
        public bool     IsValid               { get; private set; }
        public bool     IsNecessary           { get; private set; }
        public int      Priority              { get; private set; }
        public IProcess UpdateProcess         { get; private set; }
        public IFactor  MostRecentDeterminant { get; private set; }
        
        
    
        #endregion
    
        // public void RetrieveValueOf<TState, TValue>(TState state) where TState : IState<TValue>
        // {
        //     CausalObserver.ForThread
        // }

        public bool Invalidate() => Invalidate(null);

    
        public bool Invalidate(IFactor factorThatChanged)
        {
            if (IsValid)
            {
                IsValid = false;
    
                if (IsNecessary)
                {
                    CausalFactor.Update(this);
                }
                
                return true;
            }
            else return false;
        }
        
        public bool Destabilize()
        {
            IsStable = false;
    
            return IsNecessary;
        }
        
        public bool Update()
        {
            WasUpdated = true;
            IsValid  = true;
            IsStable = true;
    
            CausalFactor.Observe(UpdateProcess, this);
            
            return true;
        }
        
        public void  Notify_InfluencedBy(IFactor determinant)
        {
            WasInfluenced = true;
            MostRecentDeterminant = determinant;
    
            determinant.AddDependent(this);
        }
    
        public void SetPriority(int newPriority)
        {
            Priority = newPriority;
        }

        public MockDependent(IProcess updateProcess)
        {
            UpdateProcess = updateProcess;
            WeakReference = new WeakReference<IDependent>(this);
            IsValid = true;
        }
        
        public MockDependent(Action updateProcess) : this(ObservedActionResponse.CreateFrom(updateProcess, this))
        {
            UpdateProcess = updateProcess;
            WeakReference = new WeakReference<IDependent>(this);
            IsValid = true;
        }
    
        public MockDependent() : this(ActionProcess.CreateFrom(Tests.Tools.Tools.DoNothing))
        {
            UpdateProcess = updateProcess;
            WeakReference = new WeakReference<IDependent>(this);
            IsValid = true;
        }
    }
}