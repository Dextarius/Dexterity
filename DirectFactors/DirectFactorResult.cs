using Core.Factors;
using JetBrains.Annotations;

namespace DirectFactors
{
    public class DirectFactorResult<TInput, TOutput> : DirectFactorSubscriber<TOutput>, IFactorSubscriber<TInput>
    {
        #region Instance Fields

        [NotNull]
        private   readonly Func<TInput, TOutput>  valueFunction;
        private   readonly IFactor<TInput>        inputSource;
        protected readonly WeakSubscriber<TInput> weakSubscriber;


        #endregion


        #region Properties

        public bool IsUnstable {           get => weakSubscriber.IsUnstable;
            protected set => weakSubscriber.IsUnstable = value; }
        public bool IsNecessary {           get => weakSubscriber.IsNecessary;
            protected set => weakSubscriber.IsNecessary = value; }
        public bool HasBeenTriggered {           get => weakSubscriber.HasBeenTriggered;
            protected set => weakSubscriber.HasBeenTriggered = value; }

        #endregion


        #region Instance Methods

        public void ValueChanged(IDirectFactor<TInput> factor, TInput oldValue, TInput newValue, out bool removeSubscription)
        {
            if (IsNecessary || Owner.CoreTriggered())
            {
                CalculateResult(newValue);
            }
            else if (HasBeenTriggered is false)
            {
                HasBeenTriggered = true;
                //  InvalidateOutcome(triggeringFactor);
            }

            removeSubscription = false;
        }
        
        public virtual bool Destabilize(IDirectFactor factor)
        {
            return Owner.CoreDestabilized();
        }


        protected void CalculateResult(TInput input)
        {
         // SubscribeToInputs();
         // IsReacting       = true;
            IsUnstable       = false;
            HasBeenTriggered = false;
            
            try
            {
                var valueCreatedFromOldInput = currentValue;
                var valueCreatedFromNewInput = valueFunction(input);

                if (valueComparer.Equals(valueCreatedFromOldInput, valueCreatedFromNewInput) is false)
                {
                    currentValue = valueCreatedFromNewInput;
                    VersionNumber++;
                    NotifyOwnerValueChanged(valueCreatedFromOldInput, valueCreatedFromNewInput);
                }
            }
            catch (Exception e)
            {
                //- TODO : Consider storing exceptions as an accessible field,
                //         similar to some of the reactives available in other libraries.
                
                // InvalidateOutcome(null);
                throw;
            }
            finally
            {
                // IsReacting = false;
            }
        }
        
        public void ForceRecalculate() => CalculateResult(inputSource.Value);

        private void NotifyOwnerValueChanged(TOutput oldValue, TOutput newValue)
        {
            Owner.CoreValueChanged(oldValue, newValue);
        }

        #endregion
        

    }
}