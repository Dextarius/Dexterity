using Core.States;

namespace Core.Factors
{
    public interface IReactor : IFactor, IStabilizable, IDestabilizable, IInvalidatable
    {
        bool IsReflexive        { get; set; }
        bool IsUpdating         { get; }
        bool IsStable           { get; }
        bool IsUnstable         { get; }
        bool IsValid            { get; }
        bool IsInvalid          { get; }
        bool IsStabilizing       { get; }
        bool IsBeingInfluenced  { get; }
        int  NumberOfInfluences { get; }

        
        bool React();
    }
    
    
}