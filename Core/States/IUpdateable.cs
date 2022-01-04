namespace Core.States
{
    public interface IUpdateable
    {
        void Update();
    }
    
    
    public interface IUpdateable<T>
    {
        bool Update(out T result);
    }
}