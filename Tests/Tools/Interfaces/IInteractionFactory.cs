using Core.Causality;

namespace Tests.Tools.Interfaces
{
    public interface IInteractionFactory<out T> : IFactory<T>
       where T : IInteraction
    {
        T CreateInstance_WhoseUpdateCalls(IProcess processTo);
    }
}