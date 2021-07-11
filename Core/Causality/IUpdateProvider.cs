using JetBrains.Annotations;

namespace Core.Causality
{
    public interface IUpdateProvider
    {
        [NotNull] IUpdateHandler GetUpdateHandlerForThread();
        [NotNull] IUpdateHandler CreateNewUpdateHandler();
    }
}