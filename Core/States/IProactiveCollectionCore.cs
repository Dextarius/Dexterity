﻿using System;
using System.Collections;
using System.Collections.Generic;
using Core.Factors;

namespace Core.States
{
    public interface IProactiveCollectionCore<TValue> : IProactorCore, ICollection<TValue>, ICollectionCore<TValue> 
    {
        void AddRange(IEnumerable<TValue> itemsToAdd);
        void AddRange(params TValue[] itemsToAdd);
        void CopyTo(Array array, int index);
    }
}