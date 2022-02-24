namespace Tests.Tools.Interfaces
{
    public interface IMultiConstructorFactory<out TClass>
    {
        TClass[] CallAllConstructors();
    }
}