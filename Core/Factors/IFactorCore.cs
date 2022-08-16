using System;
using Core.States;

namespace Core.Factors
{
    public interface IFactorCore : IDisposable, IPrioritizedUpdate
    {
        bool Reconcile();
    }
}