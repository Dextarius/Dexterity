using System;
using System.Collections;
using Core.Factors;

namespace Core.States
{
    public interface ICollectionResult<TValue> : IReactorCore, ICollectionCore<TValue>
    {
        int Count { get; }

        void CopyTo(TValue[] array, int index);
        void CopyTo(Array array, int index);
        bool Contains(TValue item);
    }
}