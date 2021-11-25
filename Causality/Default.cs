using System;
using System.Collections.Generic;
using Causality.States;

namespace Causality
{
    // public class Default<T>
    // {
    //     #region Properties
    //
    //     //- TODO : I get the feeling this may lead to unexpected behaviour if users set this and expect it
    //     //         to apply to child classes of <T> as well. Also the fact that Outcome<T> uses this, but 
    //     //         doe
    //     public static IEqualityComparer<T> EqualityComparer { get; set; }
    //
    //     #endregion
    //
    //
    //     #region Constructors
    //
    //     static Default()
    //     {
    //         T testValue = default;
    //
    //         if (testValue == null)
    //         {
    //             EqualityComparer = new ReferenceEqualityComparer<T>();
    //         }
    //         else if (testValue is ValueType)
    //         {
    //             EqualityComparer = EqualityComparer<T>.Default;
    //         }
    //         else
    //         {
    //             throw new NotSupportedException();
    //         }
    //     }
    //
    //     #endregion
    // }
}