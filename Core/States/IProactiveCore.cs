using System;
using Core.Factors;
using Core.Redirection;

namespace Core.States
{
    public interface IProactiveCore<T> : IProactorCore, IValueCore<T>
    {
        bool SetValueIfNotEqual(T newValue);
    }
}