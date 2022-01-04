using Core.States;

namespace Core.Factors
{
    public interface IFactor : IPrioritizable, INameable, INecessary 
    {
        bool HasDependents      { get; }
        int  NumberOfDependents { get; }
        
        void InvalidateDependents();
        bool AddDependent(IDependent dependent);
        void RemoveDependent(IDependent dependentToRemove);
        bool Reconcile();
    }

    public interface IFactor<out T> : IFactor
    {
        T Value { get; }

        T Peek();   
    }
}