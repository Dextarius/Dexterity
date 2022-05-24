using Core.Factors;
using Core.States;

namespace Core
{
    public interface IFactorLogging
    {
        bool IsEnabled { get; set; }

        void Notify_ReactorTriggeredWhileUpdating<TReactor>(TReactor triggeredReactor, IFactor triggeringFactor);
        void Notify_ReactorHasRecursiveDependency<TReactor>(TReactor reactor, IFactorSubscriber subscriber);
    }
}