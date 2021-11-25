using Core.Causality;
using Core.States;

namespace Tests.Causality.Interfaces
{
    public interface IInteractionFactory<out T> : IFactory<T>
        where T : IInteraction
    {
        T CreateInstance_WhoseUpdateCalls(IProcess processTo);
    }
}