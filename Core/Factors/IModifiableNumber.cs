using System;

namespace Core.Factors
{
    public interface IModifiableNumber : IReactor, IModifiableDouble, IFactor<double>
    {
        
    }
}