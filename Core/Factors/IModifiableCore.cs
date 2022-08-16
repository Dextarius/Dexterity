namespace Core.Factors
{
    public interface IModifiableCore<T> : IReactorCore<T>, IModifiableBase<T>
    {
       new T BaseValue { get; set; }
    }

    //- TODO : Come up with a better name for this.  We have too many interfaces with similar sounding names.
}