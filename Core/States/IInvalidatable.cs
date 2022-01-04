using Core.Factors;

namespace Core.States
{
    public interface IInvalidatable
    {
        bool Invalidate();
        bool Invalidate(IFactor factorThatChanged);
    }
}