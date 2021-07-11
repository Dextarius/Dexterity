using System;
using System.Linq;
using JetBrains.Annotations;

namespace Core.Tools
{
    public static class Types
    {
        public static Type GetInheritedGenericType(Type genericInheritedType, Type typeToSearch)
        {
            Type result     = null;
            Type parentType = typeToSearch;

            while (parentType != typeof(object) &&
                   parentType != null           &&
                   result     == null)
            {
                if (parentType.IsGenericType &&  
                    parentType.GetGenericTypeDefinition() == genericInheritedType)
                {
                    result = parentType;
                }
                else
                {
                    parentType = parentType.BaseType;
                }
            }

            return result;
        }

        public static Type GetTypeArgumentIfInheritsGeneric(Type genericInheritedType, Type typeToSearch)
        {
            Type genericType = GetInheritedGenericType(genericInheritedType, typeToSearch);

            return genericType?.GetGenericArguments().First();
        }

        public static Type GetFirstTypeArgument(Type typeToSearch)
        {
            return typeToSearch?.GetGenericArguments().First();
        }

        public static Type CreateTypeGenericTypeUsingArgs<TArg>(Type baseGenericType)
        {
            _ = baseGenericType ?? throw new ArgumentNullException(nameof(baseGenericType));
           
            Type unconstructedGenericType = GetUnconstructedGenericTypeFrom(baseGenericType);

            return unconstructedGenericType.MakeGenericType(typeof(TArg));
        }
        
        public static Type CreateTypeGenericTypeUsingArgs<TArg1, TArg2>(Type baseGenericType)
        {
            _ = baseGenericType ?? throw new ArgumentNullException(nameof(baseGenericType));
           
            Type unconstructedGenericType = GetUnconstructedGenericTypeFrom(baseGenericType);

            return unconstructedGenericType.MakeGenericType(typeof(TArg1), typeof(TArg1));
        }
        

        [NotNull]
        private static Type GetUnconstructedGenericTypeFrom([NotNull] Type genericType)
        {
            if (genericType.IsGenericTypeDefinition)
            {
                return genericType;
            }
            else if(genericType.IsConstructedGenericType)
            {
                return genericType.GetGenericTypeDefinition();
            }
            else
            {
                throw new ArgumentException($"The argument provided to {nameof(genericType)} is neither a generic type " +
                                            "definition, nor a constructed generic type. ");
            }
        }
        
        public static string NameOf<T>() => TheType<T>.ReadableName;
        public static string NameOf([CanBeNull] Type typeToGetNameOf) => 
            (typeToGetNameOf != null)  ?  TheType.RepresentedBy(typeToGetNameOf)?.ReadableName  :  "null";
        
        
    }
}