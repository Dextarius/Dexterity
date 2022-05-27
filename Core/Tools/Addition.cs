using System;

namespace Core.Tools
{
    public static class Addition<T1, T2>
    {
        private static readonly IAdditionHandler<T1, T2> additionHandler = AdditionHandlers.GetHandler<T1,T2>();

        public static T1 Add(T1 first, T2 second) => additionHandler.Add(first, second);
    }

    public interface IAdditionHandler<T1, T2>
    {
        T1 Add(T1 first, T2 second);
    }

    public static class AdditionHandlers
    {
        public static IAdditionHandler<T1, T2> GetHandler<T1, T2>()
        {
            //- TODO : Finish this.
            
            // switch (typeof(T1))
            // {
            //     case typeof(int) : 
            // }
            
            return null;
        }
    }
}