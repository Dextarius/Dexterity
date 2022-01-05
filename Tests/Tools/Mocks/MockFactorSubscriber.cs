using System;
using Core.Factors;
using Core.States;

namespace Tests.Tools.Mocks
{
    public class MockFactorSubscriber : IFactorSubscriber
    {
        private WeakReference<IFactorSubscriber> weakReference;

        public WeakReference<IFactorSubscriber> WeakReference => weakReference ??= new WeakReference<IFactorSubscriber>(this);
        
        public bool IsNecessary { get; set; }
        public bool IsValid     { get; set; } 
        public bool IsStable    { get; set; }
        public bool WasUpdated  { get; private set; }

        
        public bool Trigger() => Trigger(null);
        public bool Trigger(IFactor triggeringFactor)
        {
            if (IsValid)
            {
                IsValid = false;
                return true;
            }
            else return false;
        }
        
        public bool Destabilize()
        {
            if (IsStable)
            {
                IsStable = false;
                return true;
            }
            else return false;
        }
        
        public void MakeNecessary()      => IsNecessary = true;
        public void MakeUnnecessary()    => IsNecessary = false;
        public void MakeValid()          => IsValid     = true;
        
        public void NotifyNecessary()    => throw new NotSupportedException();
        public void NotifyNotNecessary() => throw new NotSupportedException();
    }
}