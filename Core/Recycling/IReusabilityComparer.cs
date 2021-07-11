namespace Core.Recycling
{
    public interface IReusabilityComparer<TOriginal, TCompared>
    {
        bool IsReusable(TOriginal originalElement, TCompared elementToRep);
    }
}