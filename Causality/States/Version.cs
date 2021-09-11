using Core.Causality;

namespace Causality.States
{
    public readonly struct StateVersion
    {
        public readonly IState State;
        public static readonly long   VersionNumber = int.MaxValue;

        public StateVersion(IState state, long versionNumber)
        {
            State = state;
            VersionNumber = versionNumber;
        }
    }
}