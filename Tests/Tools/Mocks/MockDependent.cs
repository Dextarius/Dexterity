using System;
using Core.Factors;
using Core.States;

namespace Tests.Tools.Mocks
{
    public class MockDependent : IDependent
    {
        private WeakReference<IDependent> weakReference;

        public WeakReference<IDependent> WeakReference => weakReference ??= new WeakReference<IDependent>(this);
        
        public bool IsNecessary { get; set; }
        public bool IsValid     { get; set; } 
        public bool IsStable    { get; set; }
        public bool WasUpdated  { get; private set; }

        
        public bool Invalidate() => Invalidate(null);
        public bool Invalidate(IFactor factorThatChanged)
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