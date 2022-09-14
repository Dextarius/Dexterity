using System.Collections.Generic;
using Core.Factors;

namespace Factors.Cores.DirectReactorCores.CollectionResults
{
    // public abstract class ExpListResult<TValue> : ICollectionFactorSubscriber<TValue>
    // {
    //     protected List<TValue> collection;
    //
    //     public abstract void Added(TValue valueAdded, int index);
    //     public abstract void Moved(TValue valueAdded, int index);
    //     public abstract void Removed(TValue valueAdded, int index);
    //     public abstract void Replaced(TValue valueAdded, int index);
    //     
    //     
    //     public abstract void     AddValue(TValue valueAdded, int index);
    //     public abstract void    MoveValue(TValue valueAdded, int index);
    //     public abstract void  RemoveValue(TValue valueAdded, int index);
    //     public abstract void ReplaceValue(TValue valueAdded, int index);
    // }
    //
    //
    // public abstract class ConverterListResult<TOriginal, TConverted> : ExpListResult<TOriginal>
    // {
    //     public override void Added(TOriginal valueAdded, int index)
    //     {
    //         var convertedValue = Convert(valueAdded);
    //         
    //         collection.Add();
    //     }
    //     
    //     public override void Moved(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     
    //     public override void Removed(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     
    //     public override void Replaced(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     
    //     public override void AddValue(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     
    //     public override void MoveValue(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     
    //     public override void RemoveValue(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     
    //     public override void ReplaceValue(TOriginal valueAdded, int index)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //
    //     protected abstract TConverted Convert(TOriginal valueToConvert);
    // }
}