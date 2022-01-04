using System;
using static Core.Tools.Delegates;
using static Core.Tools.Types;

namespace Factors
{
    public static class Utilities
    {
        internal static string CreateDefaultReactorName<TReactor>(Delegate functionToCreateValue) => 
            $"{NameOf<TReactor>()} {GetClassAndMethodName(functionToCreateValue)}";
    }
}