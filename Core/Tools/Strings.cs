using System;
using System.Collections.Generic;

namespace Core.Tools
{
    public static class Strings
    {
        #region Constants

        private const char ReflectedTypeNameDelimiter = '`';

        #endregion

        public static string CreateReadableTypeName(Type typeToMakeNameFor) => (typeToMakeNameFor.IsGenericType)  ? 
                                                                                   CreateReadableGenericName(typeToMakeNameFor) :
                                                                                   typeToMakeNameFor.Name;

        //- I assume we're using this to get name for the TheType<T> class, or that we aren't using that class for some reason.
        //-    Otherwise you should just use NameOf<T>() or the 'ReadableName' field of that class, both use a static copy of what this would give you.
        private static string CreateReadableGenericName(Type typeToMakeNameFor)
        {
            string            reflectedVersionOfName   = typeToMakeNameFor.Name;
            int               indexofLastTypeCharacter = reflectedVersionOfName.LastIndexOf(ReflectedTypeNameDelimiter);
            string            typeNameWithoutBrackets  = reflectedVersionOfName.Substring(0, indexofLastTypeCharacter);
            IEnumerable<Type> genericArgs              = typeToMakeNameFor.GetGenericArguments();
            string            memoizedName             = $"{typeNameWithoutBrackets}<";


            foreach (Type currentArg in genericArgs)
            {
                memoizedName += $"{CreateReadableTypeName(currentArg)}, ";
            }

            memoizedName =  memoizedName.TrimEnd(' ', ',');
            memoizedName += ">";

            return memoizedName;
        }
        
        public static string CreateArgumentMemberNullMessage(string paramName, string memberName) => 
            $"The argument for {paramName} must have a {memberName} that is not null. ";
    }
}