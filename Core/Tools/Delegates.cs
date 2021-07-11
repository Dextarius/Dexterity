using System;
using System.Linq.Expressions;
using System.Reflection;
using static Core.Tools.Types;

namespace Core.Tools
{
    public static class Delegates
    {
        /// <summary>  Creates a string in the form of DeclaringType.MethodName()  </summary>
        /// <returns>
        ///     A string equivalent to method.DeclaringType + "." + method.Name + "()"; " where 'method' is
        ///     the MethodInfo for <paramref name="delegateToGetInfoFor"/>
        /// </returns>
        public static string GetClassAndMethodName(Delegate delegateToGetInfoFor)
        {
            MethodInfo infoForDelegate = delegateToGetInfoFor.Method;
            ITheType   typeOfDeclarer  = TheType.RepresentedBy(infoForDelegate.DeclaringType);
            
            return $"{typeOfDeclarer.ReadableName}.{infoForDelegate.Name}";
        }

        public static Func<TInstance, TField> MakeFieldGetter<TInstance, TField>(FieldInfo fieldToGetValueOf)
        {
            ParameterExpression instanceToAccessFieldOf_Parameter = Expression.Parameter(typeof(TInstance));
            MemberExpression    fieldForThatInstance              = Expression.Field(instanceToAccessFieldOf_Parameter, 
                                                                                     fieldToGetValueOf);

            return Expression.Lambda<Func<TInstance, TField>>(fieldForThatInstance, instanceToAccessFieldOf_Parameter).Compile();
        }

        public static Func<TProperty> MakeFieldGetterForSpecificInstance<TInstance, TProperty>(FieldInfo fieldToGetValueOf, 
                                                                                               TInstance instanceWithField)
        {
            ConstantExpression instanceToAccessFieldOf  = Expression.Constant(instanceWithField);
            MemberExpression   fieldForProvidedInstance = Expression.Field(instanceToAccessFieldOf, fieldToGetValueOf);
            
            return Expression.Lambda< Func< TProperty>>(fieldForProvidedInstance).Compile();
        }

        public static Action<TInstance, TField> MakeFieldSetter<TInstance, TField>(FieldInfo fieldToSetValueOf)
        {
            ParameterExpression instanceToSetFieldFor_Parameter = Expression.Parameter(typeof(TInstance));
            ParameterExpression valueToSetFieldTo_Parameter     = Expression.Parameter(typeof(TField));
            MemberExpression    fieldForProvidedInstance        = Expression.Field(instanceToSetFieldFor_Parameter, 
                                                                                   fieldToSetValueOf);
            BinaryExpression    setFieldToProvidedValue         = Expression.Assign(fieldForProvidedInstance, 
                                                                                    valueToSetFieldTo_Parameter);
            
            return Expression.Lambda< Action< TInstance, TField>>(setFieldToProvidedValue, 
                                                                          instanceToSetFieldFor_Parameter, 
                                                                          valueToSetFieldTo_Parameter).
                                                                          Compile();
        }

        public static Action<TField> MakeFieldSetterForSpecificInstance<TInstance, TField>(FieldInfo fieldToSetValueOf, 
                                                                                           TInstance instanceWithField)
        {
            ParameterExpression valueProvided_Parameter  = Expression.Parameter(typeof(TField));
            ConstantExpression  instanceToReceiveValue   = Expression.Constant(instanceWithField);
            MemberExpression    fieldForProvidedInstance = Expression.Field(instanceToReceiveValue, fieldToSetValueOf);
            BinaryExpression    setFieldToProvidedValue  = Expression.Assign(fieldForProvidedInstance, valueProvided_Parameter);

            return Expression.Lambda< Action< TField>>(setFieldToProvidedValue, valueProvided_Parameter).Compile();
        }

        public static Func<TInstance, TProperty> MakePropertyGetter<TInstance, TProperty>(PropertyInfo propertyToGetValueOf)
        {
            MethodInfo getMethod           = propertyToGetValueOf.GetGetMethod();
            Type       returnType          = typeof(Func<TInstance, TProperty>);
            Delegate   getMethodAsDelegate = getMethod.CreateDelegate(returnType);
            
            return (Func<TInstance, TProperty>)getMethodAsDelegate;
        }

        public static Func<TProperty> MakePropertyGetterForSpecificInstance<TInstance, TProperty>(PropertyInfo propertyToGetValueOf,
                                                                                                  TInstance    instanceToGetValueFrom)
        {
            MethodInfo getMethod           = propertyToGetValueOf.GetGetMethod();
            Type       returnType          = typeof(Func<TProperty>);
            Delegate   getMethodAsDelegate = getMethod.CreateDelegate(returnType, instanceToGetValueFrom);
            
            return (Func<TProperty>)getMethodAsDelegate;
        }

        public static Action<TInstance, TProperty> MakePropertySetter<TInstance, TProperty>(PropertyInfo propertyToSetValueOf)
        {
            MethodInfo setMethod           = propertyToSetValueOf.GetSetMethod();
            Type       returnType          = typeof(Action<TInstance, TProperty>);
            Delegate   setMethodAsDelegate = setMethod.CreateDelegate(returnType);

            return (Action<TInstance, TProperty>) setMethodAsDelegate;
        }
        
        public static Action<TProperty> MakePropertySetterForInstance<TInstance, TProperty>(PropertyInfo propertyToSetValueOf,
                                                                                            TInstance    instanceToSetValueFor)
        {
            MethodInfo setMethod           = propertyToSetValueOf.GetSetMethod();
            Type       returnType          = typeof(Action<TInstance, TProperty>);
            Delegate   setMethodAsDelegate = setMethod.CreateDelegate(returnType, instanceToSetValueFor);

            return (Action<TProperty>)setMethodAsDelegate;
        }
        
        public static Action<TDeclaring, THandler> MakeAddEventHandlerDelegate<TDeclaring, THandler>(EventInfo eventToMakeDelegateFor) => 
            (Action<TDeclaring, THandler>)eventToMakeDelegateFor.AddMethod.CreateDelegate(typeof(Action<TDeclaring, THandler>));
            
        
        public static Action<TDeclaring, THandler> MakeRemoveEventHandlerDelegate<TDeclaring, THandler>(EventInfo eventToMakeDelegateFor) =>
            (Action<TDeclaring, THandler>)eventToMakeDelegateFor.RemoveMethod.CreateDelegate(typeof(Action<TDeclaring, THandler>));

        public static string CreateNullDelegateGivenForConstructionMessage<T>() => $"A {TheType<T>.ReadableName} cannot be " +
            $"constructed with a null delegate, as it would never have a value. ";
        
        public static Action<TInstance> MakeCommandExecuter<TInstance>(MethodInfo methodToExecute)
        {
            if (methodToExecute.ReturnType !=typeof(void))
            {
                throw new ArgumentException(CreateCannotMakeExecuterBecauseOfReturnTypeMessage< Action<TInstance>>(methodToExecute));
            }
            else if (methodToExecute.GetParameters().Length != 0)
            {
                throw new ArgumentException(CreateCannotMakeExecuterBecauseOfParametersMessage< Action<TInstance>>(methodToExecute, 
                                                                                                  false));
            }

            return (Action<TInstance>) (methodToExecute.CreateDelegate(typeof(Action<TInstance>)));
        }
        
        public static Action<TInstance, TParam> MakeParameterizedCommandExecuter<TInstance, TParam>(MethodInfo methodToExecute)
        {
            if (methodToExecute.ReturnType !=typeof(void))
            {
                throw new ArgumentException(CreateCannotMakeExecuterBecauseOfReturnTypeMessage< Action<TInstance>>(methodToExecute));
            }
            else if (methodToExecute.GetParameters().Length != 1)
            {
                throw new ArgumentException(CreateCannotMakeExecuterBecauseOfParametersMessage< Action<TInstance>>(methodToExecute, 
                                                                                                  true));
            }

            return (Action<TInstance, TParam>) (methodToExecute.CreateDelegate(typeof(Action<TInstance, TParam>)));
        }

        public static string CreateCannotMakeExecuterBecauseOfReturnTypeMessage<TDelegate>(MethodInfo methodProvided) =>
            $"A command execution delegate of the type {NameOf<TDelegate>()} cannot be made from the {nameof(MethodInfo)} " +
            $"for the method {methodProvided.DeclaringType}.{methodProvided.Name} because it does not return void.";
        
        public static string CreateCannotMakeExecuterBecauseOfParametersMessage<TDelegate>(MethodInfo methodProvided, bool shouldHaveParameter)
        {
            string problemWithParameter = (shouldHaveParameter) ? "does not have a parameter" : "has parameters";
            
           return $"A command execution delegate of the type {NameOf<TDelegate>()} cannot be made from the {nameof(MethodInfo)} " +
                  $"for the method {methodProvided.DeclaringType}.{methodProvided.Name} because it {problemWithParameter}.";
        }

    }
}