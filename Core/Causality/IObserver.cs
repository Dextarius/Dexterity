using Core.Factors;
using Core.States;
using Core.Tools;

namespace Core.Causality
{
    public interface IObserver<in TInfluence, in TObserved>  
        where TInfluence : IFactor  
        where TObserved  : IObserved
    {
        bool IsCurrentlyObserving { get; }
        
        void       NotifyInvolved(TInfluence involvedObject, long triggerFlags);
        void       NotifyChanged(TInfluence changedObject);
        void       ObserveInteractions<TInteraction>(TInteraction outcomeToObserve) where TInteraction : TObserved, IProcess;
        TValue     ObserveInteractions<TInteraction, TValue>(TInteraction outcomeToObserve) where TInteraction : TObserved, IProcess<TValue>;
        void       ObserveInteractions(IProcess processToObserve, TObserved outcomeForProcess);
        T          ObserveInteractions<T>(IProcess<T> processToObserve, TObserved outcomeForProcess);
        PauseToken PauseObservation();
        void       ResumeObservation();
    }
}