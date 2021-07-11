using System;
using Core.Causality;
using Core.Factors;
using JetBrains.Annotations;

namespace Causality.Processes
{
    public class FunctionalProcess<T> : IProcess<T>
    {
        #region Instance Fields

        [NotNull]
        private readonly Func<T> valueFunction;

        #endregion
        

        #region Instance Methods

        public T Execute() => valueFunction();

        #endregion

        
        #region Constructors

        public FunctionalProcess(Func<T> functionThatDeterminesValue)
        {
            valueFunction = functionThatDeterminesValue??  
                            throw new ArgumentNullException(nameof(functionThatDeterminesValue));
        }

        #endregion
    }

    public static class FunctionalProcess
    {
        #region Static Methods

        public static FunctionalProcess<TValue> CreateFrom<TValue>(Func<TValue> function) => new FunctionalProcess<TValue>(function);

        #endregion
    }
}