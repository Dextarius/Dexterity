using Core.Factors;

namespace Tests.Tools.Mocks
{
    internal class MockReactorCallback : MockFactor, IReactorCoreCallback
    {
        public int                  CoreUpdatedCount               { get; protected set; }
        public int                  ReactorTriggeredCount          { get; protected set; }
        public int                  ReactorDestabilizedCount       { get; protected set; }
        public bool                 ReactorTriggeredReturnValue    { get; set; }
        public bool                 ReactorDestabilizedReturnValue { get; set; }
        public IReactorCoreCallback CallbackOverride               { get; protected set; }


        public void SetCallback(IReactorCoreCallback callback) => CallbackOverride = callback;

        public void CoreUpdated(IFactorCore triggeredCore, long triggerFlags)
        {
            CoreUpdatedCount++;
            CallbackOverride?.CoreUpdated(triggeredCore, triggerFlags);
        }
        
        public bool ReactorTriggered(IReactorCore triggeredCore)
        {
            ReactorTriggeredCount++;

            if (CallbackOverride != null) { return CallbackOverride.ReactorTriggered(triggeredCore); }
            else                          { return ReactorTriggeredReturnValue; }
        }
        
        public bool ReactorDestabilized(IReactorCore destabilizedCore)
        {
            ReactorDestabilizedCount++;
            
            if (CallbackOverride != null) { return CallbackOverride.ReactorDestabilized(destabilizedCore); }
            else                          { return ReactorDestabilizedReturnValue; }
        }
    }
}