using Core.Factors;

namespace DirectFactors
{
    public class DirectFactorSubscriber<TValue> : DirectSubscriberReactorCore
    {
        #region Instance Fields

        protected readonly IEqualityComparer<TValue> valueComparer;
        protected          TValue                    currentValue;

        #endregion
        
        
        #region Properties

        public TValue Value
        {
            get
            {
                // NotifyInvolved();
                return currentValue;
            }
        }
        
        protected IReactiveCoreOwner<TValue> Owner { get; }

        #endregion


        #region Instance Methods

        protected bool CreateResult()
        {
            TValue oldValue = currentValue;
            TValue newValue = GenerateValue();

            SubscribeToInputs();
            //- TODO : What if the input is somehow invalidated/changed during GenerateValue()?

            if (valueComparer.Equals(oldValue, newValue))
            {
                return false;
            }
            else
            {
                currentValue = newValue;
                return true;
            }
        }
        
        

        #endregion
    }

    public abstract class DirectSubscriberReactorCore
    {
        
        
        public uint VersionNumber { get; protected set; }

    }

    
    
}