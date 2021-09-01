using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using Core.Mimicry;
using JetBrains.Annotations;
using static Core.Tools.Strings;

namespace Core.Tools
{
    public static class TheType<T>
    {
        #region Static Fields
        
        [NotNull] public static readonly Type           TypeInstance                = typeof(T);
        [NotNull] public static readonly string         ReadableName                = CreateReadableTypeName(TypeInstance); 
        [NotNull] public static readonly TypeInfo       Info                        = TypeInstance.GetTypeInfo();
        [NotNull] public static readonly FieldInfo[]    Fields                      = TypeInstance.GetFields();
        [NotNull] public static readonly PropertyInfo[] Properties                  = TypeInstance.GetProperties();
        [NotNull] public static readonly EventInfo[]    Events                      = TypeInstance.GetEvents();
                  public static readonly bool           IsEnumerable                = typeof(IEnumerable  ).IsAssignableFrom(typeof(T));
                  public static readonly bool           IsCollection                = typeof(ICollection<>).IsAssignableFrom(typeof(T)) || 
                                                                                      typeof(ICollection  ).IsAssignableFrom(typeof(T));
                  public static readonly bool           IsDisposable                = typeof(IDisposable  ).IsAssignableFrom(typeof(T));
                  public static readonly bool           IsEnumerableOtherThanString = IsEnumerable && typeof(T) != typeof(string);
                  public static readonly bool           IsNullable                  = default(T) == null;
                  public static readonly bool           IsNotNullable               = !IsNullable;


        //- We could use this to determine if a ReactiveList should use Recycling by default
        public static readonly bool IsImmutable = TypeInstance == typeof(string);
        
        #endregion


        #region Static Methods

        public static bool CanBeCastTo<TResult>() => And<TResult>.IsAValidCast;
        
        #endregion
        
        
        #region Nested Classes

        public static class And<TSecondType>
        {
            public static bool IsAValidCast = typeof(And<TSecondType>).IsAssignableFrom(typeof(TheType<T>));
        }
        

        public class Instance : ITheType
        {
            #region Explicit Implementations

            Type           ITheType.TypeInstance                => TypeInstance;
            string         ITheType.ReadableName                => ReadableName;
            TypeInfo       ITheType.Info                        => Info;
            FieldInfo[]    ITheType.Fields                      => Fields;
            PropertyInfo[] ITheType.Properties                  => Properties;
            EventInfo[]    ITheType.Events                      => Events;
            bool           ITheType.IsEnumerable                => IsEnumerable;
            bool           ITheType.IsEnumerableOtherThanString => IsEnumerableOtherThanString;

            bool           ITheType.IsCollection    => IsCollection;
            bool           ITheType.IsDisposable    => IsDisposable;
            bool           ITheType.IsNullable      => IsNullable;
            bool           ITheType.IsNotNullable   => IsNotNullable;
          //bool           ITheType.IsAMimicType    => IsAMimicType;
          //bool           ITheType.IsNotAMimicType => IsNotAMimicType;
            bool           ITheType.IsImmutable     => IsImmutable;

            bool ITheType.CanBeCastTo<TResult>() => CanBeCastTo<TResult>();

            #endregion
        }

        #endregion
    }

    
    public static class TheType
    {
        private static readonly Dictionary<Type, ITheType> instancesByType = new Dictionary<Type, ITheType>();

        
        public static ITheType RepresentedBy(Type typeToGet)
        {
            if (typeToGet != null)
            {
                return GetOrCreateTypeInstance(typeToGet);
            }
            else
            {
                return null;
            }
        }

        private static ITheType GetOrCreateTypeInstance([NotNull] Type typeToGet)
        {
            ITheType result;

            instancesByType.TryGetValue(typeToGet, out result);
            
            if (result == null)
            {
                instancesByType[typeToGet] = result = CreateTheTypeInstanceForType(typeToGet);
            }
            
            return result;
        }

        private static ITheType CreateTheTypeInstanceForType(Type typeToCreateInstanceFor)
        {
            Type            staticType             = typeof(TheType<>);//.MakeGenericType(typeToCreateInstanceFor);
            string          nameOfInstanceType     = nameof(TheType<object>.Instance);
            Type            instanceType           = staticType.GetNestedType(nameOfInstanceType);
            var             genericInstanceType    = instanceType.MakeGenericType(typeToCreateInstanceFor);
            ConstructorInfo instanceConstructor    = genericInstanceType.GetConstructor(Type.EmptyTypes)?? 
                                                        throw MissingConstructorException(nameOfInstanceType);
            ITheType        createdInstance        = (ITheType)(instanceConstructor.Invoke(new object[] {}));
            
            return createdInstance;
        }

       private static MissingMethodException MissingConstructorException(string nameOfType) =>
            new MissingMethodException($" The class {nameof(TheType)}.{nameOfType} was expected to have a default constructor, but none was found.");
    }

    public interface ITheType
    {
        #region Instance Fields
        
        [NotNull] Type           TypeInstance                { get; }
        [NotNull] string         ReadableName                { get; }
        [NotNull] TypeInfo       Info                        { get; }
        [NotNull] FieldInfo[]    Fields                      { get; }
        [NotNull] PropertyInfo[] Properties                  { get; }
        [NotNull] EventInfo[]    Events                      { get; }
                  bool           IsEnumerable                { get; }
                  bool           IsCollection                { get; }
                  bool           IsDisposable                { get; }
                  bool           IsNullable                  { get; }
                  bool           IsNotNullable               { get; }
                //bool           IsAMimicType                { get; }
                //bool           IsNotAMimicType             { get; }
                  bool           IsImmutable                 { get; }
                  bool           IsEnumerableOtherThanString { get; }

        #endregion

        bool CanBeCastTo<TResult>();
    }

    
    
    
    
}