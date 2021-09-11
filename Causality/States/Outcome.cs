using System;
using Core.Causality;
using Core.Factors;

namespace Causality.States
{
    public class Outcome : State, IOutcome
    {
        #region Instance Fields

        protected IProcess process;
        
        #endregion


        #region Properties
        

        #endregion

        
        #region Constructors
        
        public Outcome(INotifiable objectToNotify) 
        {
            callbackReference = objectToNotify != null?  new WeakReference<INotifiable>(objectToNotify) :
                                                         default;
        }

        public Outcome() : this(null)
        {
        }
        
        #endregion
    }



    
    
    
}