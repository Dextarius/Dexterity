using Core.Tools;

namespace Tests
{
    public static class ErrorMessages
    {
        public const string
            FunctionExecutedBeforeRequested = "The Reactive's value function was run before the Reactive's value was requested",
            ReactorNotOutdated              = "The Reactor indicated that it was valid, even though its source value changed.";
        
        public static string ValueDidNotMatch<T>(string situation) =>
            $"The value for the {Types.NameOf<T>()} does not match the value {situation}. ";

        public static string FactorWasConsequential<T>(string situation) =>
            $"The {Types.NameOf<T>()} was marked as consequential {situation}. ";

        public static string FactorWasNotConsequential<T>(string situation) =>
            $"The {Types.NameOf<T>()} was marked as not being consequential {situation}. ";

        public static string ReactorWasNotValid<T>() =>
            $"The {Types.NameOf<T>()} indicated that it was not valid after its value was calculated.";
    }
}