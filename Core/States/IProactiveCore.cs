using System;
using Core.Factors;
using Core.Redirection;

namespace Core.States
{
    public interface IProactiveCore<T> : IFactorCore, IValueCore<T>
    {
        bool ChangeValueTo(T newValue);
    }
}