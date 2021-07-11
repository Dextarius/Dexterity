using Core.Causality;

namespace Core
{
    public static class Config
    {
        //- This originally existed in the testing section for whatever reason, but we need it to compile the updater code.
        //   Perhaps find the right place for it later.
        public static IUpdateExecutionProvider ActiveExecutionProvider { get; set; } = new DummyExecutionProvider();

    }
}