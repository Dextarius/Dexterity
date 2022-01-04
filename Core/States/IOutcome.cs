using System;
using Core.Factors;

namespace Core.States
{ 
    public interface IOutcome : IFactor, IDependent, IStabilizable
    {
        #region Properties

        bool IsReflexive        { get; set; }
        bool IsUpdating         { get; }
        bool IsValid            { get; }
        bool IsInvalid          { get; }
        bool IsStable           { get; }
        bool IsUnstable         { get; }
        bool IsStabilizing      { get; }
        bool IsBeingInfluenced  { get; }
        int  NumberOfInfluences { get; }

        #endregion

        
        #region Instance Methods

        bool Generate();

        #endregion
    }
    
    
    
    
}