using System.Collections;
using Core.Causality;
using Core.Factors;
using Core.Redirection;
using Core.States;

namespace Factors.Collections
{
    public class FactorDictionaryEnumerator : LinkedDictionaryEnumerator
    {
        #region Instance Fields

        private readonly IFactor dictionaryFactor;

        #endregion

        
        #region Instance Methods

        protected override void OnInteraction() => dictionaryFactor.NotifyInvolved() ;

        #endregion

        
        #region Constructors

        public FactorDictionaryEnumerator(IFactor factorBeingLinked, IDictionaryEnumerator enumeratorForDictionary) :
            base(enumeratorForDictionary)
        {
            dictionaryFactor = factorBeingLinked;
        }

        #endregion
    }
}