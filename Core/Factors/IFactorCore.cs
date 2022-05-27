using System;

namespace Core.Factors
{
    public interface IFactorCore : IDisposable
    {
        int  UpdatePriority { get; }
        uint VersionNumber  { get; }

    }
}