namespace Core.Factors
{
    public interface IProactorCore : IFactorCore 
    {
        void SetCallback(IFactorCoreCallback callback);
    }
}