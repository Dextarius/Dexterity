using Core.States;

namespace Causality.Scratch
{
    public interface IOwner
    {
        void NotifyInvalidated();
        
    }
}