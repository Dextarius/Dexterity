using System;
using Core.Causality;
using Core.Factors;

namespace Causality.States
{
    public class Response : Result
    {
        #region Instance Fields

        protected readonly IProcess process;
        
        #endregion


        #region Properties
        

        #endregion


        #region Instance Methods

        protected override bool ExecuteProcess()
        {
            Observer.ObserveInteractions(process, this);

            return true;
        }

        #endregion

        
        #region Constructors
        

        public Response(object owner, IProcess processToDetermineOutcome) : base(owner)
        {
            process = processToDetermineOutcome;
        }
        
        public Response(IProcess processToDetermineOutcome) : this(null, processToDetermineOutcome)
        {
        }
        
        #endregion
    }
}