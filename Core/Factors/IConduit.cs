using JetBrains.Annotations;

namespace Core.Factors
{
    //- TODO : We need to flesh these out more, they should be doing more than just providing a value.
    
    public interface IConduit
    {
       [CanBeNull] object Value { get; set; }
    }
        
    public interface IConduit<T>
    {
        [CanBeNull] T Value { get; set; }
    }
}