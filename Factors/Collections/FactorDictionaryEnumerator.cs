using System.Collections;
using Core.Causality;
using Core.Factors;
using Core.Redirection;

namespace Factors.Collections
{
    public class FactorDictionaryEnumerator : LinkedDictionaryEnumerator
    {
        #region Instance Fields

        private readonly IState dictionaryState;

        #endregion

        
        #region Instance Methods

        protected override void OnInteraction() => dictionaryState.NotifyInvolved() ;

        #endregion

        
        #region Constructors

        public FactorDictionaryEnumerator(IState stateBeingLinked, IDictionaryEnumerator enumeratorForDictionary) :
            base(enumeratorForDictionary)
        {
            dictionaryState = stateBeingLinked;
        }

        #endregion
    }
}