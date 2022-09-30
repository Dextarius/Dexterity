using System;
using System.Runtime.CompilerServices;

namespace Factors.Exceptions
{
    public class CannotModifyReactiveValueException : Exception
    {
        #region Constants

        private const string ErrorMessageStart = "A process attempted to call the method '";

        private const string ErrorMessageEnd = 
            "' on a Reactive factor.  The value of a Reactive factor is determined using a " +
            "process specified at creation, and cannot be modified externally. ";

        #endregion
        

        #region Static Methods

        private static string CreateErrorMessage(string methodName) => $"{ErrorMessageStart}{methodName}{ErrorMessageEnd}";

        #endregion


        #region Constructors

        public CannotModifyReactiveValueException()
        {
        }

        public CannotModifyReactiveValueException([CallerMemberName] string methodName = "")
            : base(CreateErrorMessage(methodName))
        {
        }
        
        public CannotModifyReactiveValueException(string message, [CallerMemberName] string methodName = "")
            : base(CreateErrorMessage(methodName) + message)
        {
        }

        public CannotModifyReactiveValueException(Exception inner, [CallerMemberName] string methodName = "")
            : base(CreateErrorMessage(methodName), inner)
        {
        }
        
        public CannotModifyReactiveValueException(string message, Exception inner,  [CallerMemberName] string methodName = "")
            : base(CreateErrorMessage(methodName) + message, inner)
        {
        }

        #endregion
    }
}