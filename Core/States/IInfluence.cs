namespace Core.States
{
    public interface IInfluence : IPrioritizable, INameable, INecessary
    {
        void NotifyNecessary();
        void NotifyNotNecessary();
        bool Reconcile();
        bool AddDependent(IDependency dependent);
        void ReleaseDependent(IDependency dependent);
    }
}