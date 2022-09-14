using System;

namespace Factors
{
    public class CollectionInvolver<TValue>
    {

    }
    
    public class CollectionTrigger<TValue>
    {
        [Flags]
        public enum Operation { Added, Removed, Moved }


        protected readonly IChannel<TValue> valueAdded;
        protected readonly IChannel<TValue> valueRemoved;
        protected readonly IChannel<TValue> valueMoved;
        protected          Operation        relevantOperations;

        private bool IsActive { get; set; }
        
        
    }
    
    
    public abstract class CollectionFilter<TValue> : CollectionTrigger<TValue>
    {

        protected void OnValueAdded(TValue value)
        {
            if (MeetsFilterCriteria(value))
            {
                // Add value to filtered collection.
            }
        }
        
        protected abstract bool MeetsFilterCriteria(TValue value);

        protected CollectionFilter()
        {
            relevantOperations |= Operation.Added;


        }
    }
}