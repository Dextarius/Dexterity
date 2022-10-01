using System;
using System.Linq.Expressions;
using System.Reflection;
using Dextarius.Utilities;
using static Dextarius.Utilities.Types;

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

        public static string CreateStringShowingArgumentBeingPassedToAction<TArg>(TArg argument, Action<TArg> function) => 
            CreateStringShowingArgumentBeingPassedToDelegate(argument, function);
        public static string CreateStringShowingArgumentBeingPassedToAction<TArg1, TArg2>(
            TArg1 argument1, TArg2 argument2, Action<TArg1, TArg2> action) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument1, argument2, action);
        
        public static string CreateStringShowingArgumentBeingPassedToAction<TArg1, TArg2, TArg3>(
            TArg1 argument1, TArg2 argument2, TArg3 argument3, Action<TArg1, TArg2, TArg3> action) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument1, argument2, argument3, action);
        
        public static string CreateStringShowingArgumentBeingPassedToAction<TArg1, TArg2, TArg3, TArg4>(
            TArg1 argument1, TArg2 argument2, TArg3 argument3, TArg4 argument4, 
            Action<TArg1, TArg2, TArg3, TArg4> action) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument1, argument2, argument3, argument4, action);

        public static string CreateStringShowingArgumentBeingPassedToFunction<TArg, TReturn>(
            TArg argument, Func<TArg, TReturn> function) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument, function);
        
        public static string CreateStringShowingArgumentBeingPassedToFunction<TArg1, TArg2, TReturn>(
            TArg1 argument1, TArg2 argument2, Func<TArg1, TArg2, TReturn> function) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument1, argument2, function);
        
        public static string CreateStringShowingArgumentBeingPassedToFunction<TArg1, TArg2, TArg3, TReturn>(
            TArg1 argument1, TArg2 argument2, TArg3 argument3, Func<TArg1, TArg2, TArg3, TReturn> function) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument1, argument2, argument3, function);
        
        public static string CreateStringShowingArgumentBeingPassedToFunction<TArg1, TArg2, TArg3, TArg4, TReturn>(
            TArg1 argument1, TArg2 argument2, TArg3 argument3, TArg4 argument4, 
            Func<TArg1, TArg2, TArg3, TArg4, TReturn> function) => 
                CreateStringShowingArgumentBeingPassedToDelegate(argument1, argument2, argument3, argument4, function);
        
        public static string CreateStringShowingArgumentBeingPassedToDelegate<T>(T argument, Delegate delegateToUse) => 
            $"{GetClassAndMethodName(delegateToUse)}({argument})";
        
        public static string CreateStringShowingArgumentBeingPassedToDelegate<T1, T2>(
            T1 argument1, T2 argument2, Delegate delegateToUse) => 
                $"{GetClassAndMethodName(delegateToUse)}({argument1}, {argument2})";
        
        public static string CreateStringShowingArgumentBeingPassedToDelegate<T1, T2, T3>(
            T1 argument1, T2 argument2, T3 argument3, Delegate delegateToUse) =>
                $"{GetClassAndMethodName(delegateToUse)}({argument1}, {argument2}, {argument3})";
        
        public static string CreateStringShowingArgumentBeingPassedToDelegate<T1, T2, T3, T4>(
            T1 argument1, T2 argument2, T3 argument3, T4 argument4, Delegate delegateToUse) =>
                $"{GetClassAndMethodName(delegateToUse)}({argument1}, {argument2}, {argument3}, {argument4})";
        
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