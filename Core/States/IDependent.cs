namespace Core.States
{
    public interface IDependent
    {
        bool Invalidate(IInfluence influenceThatChanged);
        bool Destabilize();
    }
}