namespace Tests.Tools.Interfaces
{
    public interface INameableConstructorTestingFactory<out TClass>
    {
        TClass[] CallAllConstructorsUsing_Name(string name);
    }
}