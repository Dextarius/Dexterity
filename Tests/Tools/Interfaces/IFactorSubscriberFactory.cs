using Core.Factors;
using Core.States;

namespace Tests.Tools.Interfaces
{
    public interface IFactorSubscriberFactory<out T> : IFactory<T> 
        where T : IFactorSubscriber
    {
        T CreateStableUntriggeredInstance();
    }
    
    public interface IFactorController<out T> : IController<T>
        where T : IFactor
    {
        
    }
    
    public interface IFactorSubscriberController<out T> : IController<T>
        where T : IFactorSubscriber
    {
        public bool CheckIfTriggered();
        public void MakeStableAndUntriggered();
        public void MakeNecessary();
    }

    public interface IController<out T>
    {
        T ControlledInstance { get; }
    }
}