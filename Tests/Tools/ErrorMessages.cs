using Core.Tools;
using static Core.Tools.Types;

namespace Tests.Tools
{
    public static class ErrorMessages
    {
        public const string
            FunctionExecutedBeforeRequested = "The Reactive's value function was run before the Reactive's value was requested",
            ReactorNotOutdated              = "The Reactor indicated that it was valid, even though its source value changed.";
        
        public static string ValueDidNotMatch<T>(string situation) =>
            $"The value for the {NameOf<T>()} does not match the value {situation}. ";

        public static string HasSubscribers<T>(string situation) =>
            $"The {NameOf<T>()} was marked as having subscribers {situation}. ";
        
        public static string SubscribersGreaterThanZero<T>(string situation, int numberOfSubscribers) =>
            $"The {NameOf<T>()} was marked as having {numberOfSubscribers} subscribers {situation}. ";
        
        public static string HasInfluences<T>(string situation) =>
            $"The {NameOf<T>()} was marked as having influences {situation}. ";
        
        public static string InfluencesGreaterThanZero<T>(string situation, int numberOfInfluences) =>
            $"The {NameOf<T>()} was marked as having {numberOfInfluences} influences {situation}. ";

        public static string FactorDidNotHaveSubscribers<T>(string situation) =>
            $"The {NameOf<T>()} was marked as not having subscribers {situation}. ";

        public static string ReactorWasNotValid<T>() =>
            $"The {NameOf<T>()} indicated that it was not valid after its value was calculated.";
            
        public static string ValueFactorInvalidatedSubscribersWhenGivenAnEquivalentValue<T>() =>
            $"Changing the value of a {NameOf<T>()} invalidated its subscribers even though " +
             "the new value was equal to the old value. ";
    }
}