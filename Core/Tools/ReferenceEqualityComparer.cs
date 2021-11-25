using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Tools
{
   public class ReferenceEqualityComparer<T> : IEqualityComparer<T>  
    {
        public bool Equals(T object1, T object2)
        {
            return ReferenceEquals(object1, object2);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        public ReferenceEqualityComparer()
        {
            Debug.Assert(default(T) is ValueType is false, 
                "The ReferenceEqualityComparer should not be used on value types, " +
                "since testing them using reference equality will only compare boxed versions of the values.");
        }
    }
}