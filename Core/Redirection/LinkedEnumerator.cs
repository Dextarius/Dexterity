using System.Collections;
using System.Collections.Generic;

namespace Core.Redirection
{
    public abstract class LinkedEnumerator : IEnumerator
    {
        #region Instance Fields

        protected bool enumerationStarted = false;

        #endregion
        
        
        #region Properties

        protected virtual IEnumerator EnumeratorForLinkedCollection { get; }

        public object Current
        {
            get
            {
                OnInteraction();
                return EnumeratorForLinkedCollection.Current;
            }
        }

        #endregion


        #region Instance Methods


        //- Keep in mind this is called before the enumerator takes whatever specific action uses it
        protected abstract void OnInteraction();
        protected abstract void OnEnumerationStarted();

        public bool MoveNext()
        {
            if (enumerationStarted is false)
            {
                OnEnumerationStarted();
                enumerationStarted = true;
            }
            
            OnInteraction();
            return EnumeratorForLinkedCollection.MoveNext();
        }

        public void Reset()
        {
            OnInteraction();
            EnumeratorForLinkedCollection.Reset();
            enumerationStarted = false;
        }

        #endregion


        #region Constructors

        protected LinkedEnumerator()  //- TODO : Does this need to set EnumeratorForLinkedCollection?
        {
            
        }

        protected LinkedEnumerator(IEnumerator enumeratorForLinked)
        {
            EnumeratorForLinkedCollection = enumeratorForLinked;
        }

        #endregion
    }
    
    public abstract class LinkedEnumerator<T> : LinkedEnumerator, IEnumerator<T>
    {
        #region Instance Fields

        private readonly IEnumerator<T> enumeratorForParentCollection;

        #endregion
        

        #region Properties

        protected override IEnumerator EnumeratorForLinkedCollection => enumeratorForParentCollection;

        public new T Current
        {
            get
            {
                OnInteraction();
                return enumeratorForParentCollection.Current;
            }
        }

        #endregion

        
        #region Instance Methods

        public void Dispose()
        {
            //- TODO : Do we want this to call OnInteraction()?
            enumeratorForParentCollection.Dispose();
        }

        #endregion


        #region Constructors

        protected LinkedEnumerator(IEnumerator<T> enumeratorForParent)
        {
            enumeratorForParentCollection = enumeratorForParent;
        }

        #endregion
    }
    
    
    public abstract class LinkedDictionaryEnumerator : LinkedEnumerator, IDictionaryEnumerator
    {
        private readonly IDictionaryEnumerator enumeratorForParentsDictionary;

        public DictionaryEntry Entry
        {
            get
            {
                OnInteraction();
                return enumeratorForParentsDictionary.Entry;
            }
        }

        public object Key
        {
            get
            {
                OnInteraction();
                return enumeratorForParentsDictionary.Key;
            }
        }

        public object Value
        {
            get
            {
                OnInteraction();
                return enumeratorForParentsDictionary.Value;
            }
        }


        protected LinkedDictionaryEnumerator(IDictionaryEnumerator enumeratorForDictionary) : base(enumeratorForDictionary)
        {
            enumeratorForParentsDictionary = enumeratorForDictionary;
        }
    }
}