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

        private readonly IInvolved involvedDictionary;

        #endregion

        
        #region Instance Methods

        //- TODO : Calling NotifyInvolved() every time someone accesses a key and entry in the enumeration
        //         seems like it could be obnoxious.  When we've had a chance to use the library more come
        //         back here and decide if it's likely that people are going to use the enumerators in a 
        //         way where they are using the same enumerator during multiple reaction processes.
        //protected override void OnInteraction() => involvedDictionary.NotifyInvolved() ;

        protected override void OnInteraction() { }
        
        protected override void OnEnumerationStarted() => 
            involvedDictionary.NotifyInvolved(TriggerFlags.ItemAdded | TriggerFlags.ItemRemoved | TriggerFlags.ItemReplaced);

        #endregion

        
        #region Constructors

        public FactorDictionaryEnumerator(IInvolved collectionOwner, IDictionaryEnumerator enumeratorForDictionary) :
            base(enumeratorForDictionary)
        {
            involvedDictionary = collectionOwner;
        }

        #endregion


    }
}