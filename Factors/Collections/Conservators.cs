using System;
using System.Collections.Generic;
using Core.Redirection;
using Factors.Exceptions;
using static Core.Tools.Types;

namespace Factors.Collections
{
    public partial class ProactiveDictionary<TKey, TValue> 
    {
        //- TODO : Most of this is redundant between the two classes. There's probably a cleaner way to implement these.
        //         See if the Proactor class has any contents by the time we come back to this,
        //         if so we can probably copy the Reactive versions.
        internal class ProactiveKeyConservator : Conservator<TKey>
        {
            #region Instance Fields

            protected readonly ProactiveDictionary<TKey, TValue> LinkedProactive;

            #endregion
            
            #region Properties
            
            public override ICollection<TKey> ManagedCollection => LinkedProactive.Collection.Keys;

            #endregion

            
            #region Instance Methods

            protected override void OnAccessed() {}
            protected override void OnModified() => 
                throw new InvalidOperationException($"A {NameOf<ProactiveDictionary<TKey, TValue>>()}'s key" +
                                                     "collection cannot be modified directly. ");

            #endregion
            
            #region Constructors

            internal ProactiveKeyConservator(ProactiveDictionary<TKey, TValue> creator) : base(Operations.CopyTo)
            {
                LinkedProactive = creator;
            }

            #endregion
        }
        
        internal class ProactiveValueConservator : Conservator<TValue>
        {
            
            #region Instance Fields

            protected readonly ProactiveDictionary<TKey, TValue> LinkedProactive;

            #endregion
            
            #region Properties
            
            public override ICollection<TValue> ManagedCollection => LinkedProactive.Collection.Values;

            #endregion

            
            #region Instance Methods

            protected override void OnAccessed() {}
            protected override void OnModified() => 
                throw new InvalidOperationException($"A {NameOf<ProactiveDictionary<TKey, TValue>>()}'s values" +
                                                    $"collection cannot be modified directly. ");

            #endregion
            
            #region Constructors

            internal ProactiveValueConservator(ProactiveDictionary<TKey, TValue> creator) : base(Operations.CopyTo)
            {
                LinkedProactive = creator;
            }

            #endregion
        }
    }
    
    internal abstract class ReactiveConservator<TReactive, TValue> : Conservator<TValue> where TReactive : Reactor
    {
        #region Constants

        private const string WriteAttemptErrorMessage = 
            "A process attempted to modify a collection whose elements are determined by a ReactiveCollection.";

        #endregion
        
        
        #region Instance Fields

        protected readonly TReactive LinkedReactor;

        #endregion


        #region Instance Methods

        protected override void OnAccessed() { }
        protected override void OnModified() => throw new CannotModifyReactiveValueException(message:WriteAttemptErrorMessage);  
        //- TODO : Consider if there is any reason a collection belonging to a Reactor would be mutable.
        
        //- TODO : We need to test the workings of GetEnumerator() to make sure that the reactive collection's
        //         outcome notifies the observer it's involved each time it accesses an element of the collection.

        #endregion


        #region Constructors

        protected ReactiveConservator(TReactive creator, Operations operationsSupported) : base(operationsSupported)
        {
            LinkedReactor = creator;
        }

        #endregion
    }

    public partial class ReactiveDictionary<TKey, TValue>
    {
        internal class ReactiveKeyConservator : ReactiveConservator<ReactiveDictionary<TKey, TValue>, TKey>
        {
            #region Properties

            public override ICollection<TKey> ManagedCollection => LinkedReactor.Collection.Keys;

            #endregion


            #region Constructors

            internal ReactiveKeyConservator(ReactiveDictionary<TKey, TValue> creator) :
                base(creator, Operations.CopyTo)
            {
            }

            #endregion
        }
        
        internal class ReactiveValueConservator : ReactiveConservator< ReactiveDictionary<TKey, TValue>, TValue>
        {
            #region Properties

            public override ICollection<TValue> ManagedCollection => LinkedReactor.Collection.Values;

            #endregion


            #region Constructors

            internal ReactiveValueConservator(ReactiveDictionary<TKey, TValue> creator) :
                base(creator, Operations.CopyTo)
            {
            }

            #endregion
        }
    }
}