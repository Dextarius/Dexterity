namespace Tests.Tools.Interfaces
{
    public interface IFactory<out TClass>
    {
        TClass CreateInstance();
        TClass CreateStableInstance();
    }
}