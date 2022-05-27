using System;
using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface ICollectionResult<TValue> : IReactorCore
    {
        int Count { get; }

        void                CopyTo(TValue[] array, int index);
        void                CopyTo(Array    array, int index);
        bool                Contains(TValue item);
        IEnumerator<TValue> GetEnumerator();
    }
}