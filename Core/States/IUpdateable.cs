namespace Core.States
{
    public interface IUpdateable
    {
        bool Update();
    }
    
    
    public interface IUpdateable<T>
    {
        bool Update(out T result);
    }
}