using Core.Tools;
using static Core.Tools.Types;

namespace Tests
{
    public static class ErrorMessages
    {
        public const string
            FunctionExecutedBeforeRequested = "The Reactive's value function was run before the Reactive's value was requested",
            ReactorNotOutdated              = "The Reactor indicated that it was valid, even though its source value changed.";
        
        public static string ValueDidNotMatch<T>(string situation) =>
            $"The value for the {Types.NameOf<T>()} does not match the value {situation}. ";

        public static string HasDependents<T>(string situation) =>
            $"The {Types.NameOf<T>()} was marked as having dependents {situation}. ";
        
        public static string DependentsGreaterThanZero<T>(string situation, int numberOfDependents) =>
            $"The {Types.NameOf<T>()} was marked as having {numberOfDependents} dependents {situation}. ";
        
        public static string HasInfluences<T>(string situation) =>
            $"The {Types.NameOf<T>()} was marked as having influences {situation}. ";
        
        public static string InfluencesGreaterThanZero<T>(string situation, int numberOfInfluences) =>
            $"The {Types.NameOf<T>()} was marked as having {numberOfInfluences} influences {situation}. ";

        public static string FactorDidNotHaveDependents<T>(string situation) =>
            $"The {Types.NameOf<T>()} was marked as not having dependents {situation}. ";

        public static string ReactorWasNotValid<T>() =>
            $"The {Types.NameOf<T>()} indicated that it was not valid after its value was calculated.";
            
        public static string ValueFactorInvalidatedDependentsWhenGivenAnEquivalentValue<T>() =>
            $"Changing the value of a {NameOf<T>()} invalidated its dependents even though " +
             "the new value was equal to the old value. ";
    }
}