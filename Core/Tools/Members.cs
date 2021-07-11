using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Tools
{
    public static class Members
    {
        public static IEnumerable<T> GetCustomAttributesWithType<T>(this PropertyInfo property) where T : Attribute =>
            property.GetCustomAttributes(typeof(T), false).OfType<T>();
    }
}