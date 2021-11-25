using System;
using Causality;
using Causality.Processes;
using Causality.States;
using Core.Causality;
using Core.States;

namespace Tests.Causality.Mocks
{
    public class MockInteraction : IInteraction
    {
        #region Instance Fields

        #endregion
        

        #region Properties

        public WeakReference<IInteraction> WeakReference { get; }

        public bool                 WasUpdated          { get; private set; }
        public bool                 WasInfluenced       { get; private set; }
        public bool                 IsStable            { get; private set; }
        public bool                 IsValid             { get; private set; }
        public bool                 IsNecessary         { get; private set; }
        public int                  Priority            { get; private set; }
        public IProcess             UpdateProcess       { get; private set; }
        public IInfluence MostRecentInfluence { get; private set; }
        
        

        #endregion

        // public void RetrieveValueOf<TState, TValue>(TState state) where TState : IState<TValue>
        // {
        //     CausalObserver.ForThread
        // }


        public bool Invalidate(IInfluence influenceThatChanged)
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
        
        public void  Notify_InfluencedBy(IInfluence influence)
        {
            WasInfluenced = true;
            MostRecentInfluence = influence;

            influence.AddDependent(this);
        }

        public void SetPriority(int newPriority)
        {
            Priority = newPriority;
        }

        public void MakeNecessary()   => IsNecessary = true;
        public void MakeUnnecessary() => IsNecessary = false;
        public void MakeValid()       => IsValid     = true;

        public MockInteraction(IProcess updateProcess)
        {
            UpdateProcess = updateProcess;
            WeakReference = new WeakReference<IInteraction>(this);
            IsValid = true;
        }
        
        public MockInteraction(Action updateProcess) : this(ActionProcess.CreateFrom(updateProcess))
        {
        }

        public MockInteraction() : this(ActionProcess.CreateFrom(Tools.DoNothing))
        {
            
        }
    }
}