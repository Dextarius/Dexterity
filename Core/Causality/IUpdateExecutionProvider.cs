using System;

namespace Core.Causality
{
    public interface IUpdateExecutionProvider
    {
        Action<Action> GetUpdateExecutionProcess();
    }
}