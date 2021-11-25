using Core.Factors;

namespace Core.States
{
    public interface IResult : IDependent, IFactor
    {
        bool IsStable           { get;      }
        bool IsValid            { get;      }
        bool IsBeingInfluenced  { get;      }
        int  NumberOfInfluences { get;      }
        bool IsUpdating         { get;      }
        bool IsReflexive        { get; set; }
        //  bool AllowRecursion     { get; set; }
        
        
        bool React();
    }
    
    
    public interface IResult<out T> : IResult, IState<T>
    {
        
    }
}