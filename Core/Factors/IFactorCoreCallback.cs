namespace Core.Factors
{
    public interface IFactorCoreCallback : IFactor
    {
        void CoreUpdated(IFactorCore triggeredCore, long triggerFlags);
    }
}