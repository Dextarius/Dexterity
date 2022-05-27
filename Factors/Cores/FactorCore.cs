using Core.Factors;

namespace Factors.Cores
{
    public abstract class FactorCore : IFactorCore
    {
        public virtual int  UpdatePriority => 1;
        public         uint VersionNumber  { get; protected set; }
        
        public virtual void Dispose() { }
    }
}