namespace Core.States
{ 
    public interface IDependable<in TDependent>
    {
        bool HasDependents { get; }

        bool     AddDependent(TDependent dependent);
        void ReleaseDependent(TDependent dependent);
    }
}