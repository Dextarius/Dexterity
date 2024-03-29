﻿using Core.Causality;

namespace Tests.Tools.Mocks.Processes
{
    public class StoredValueProcess<T> : IProcess<T>
    {
        #region Properties

        public T Value { get; set; }
        
        #endregion
        

        #region Instance Methods

        public T Execute() => Value;

        #endregion

        
        #region Constructors

        public StoredValueProcess(T initialValue)
        {
            Value = initialValue;
        }

        #endregion
    }
}