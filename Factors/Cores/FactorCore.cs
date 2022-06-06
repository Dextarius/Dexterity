using Core.Factors;

namespace Factors.Cores
{
    public abstract class FactorCore : IFactorCore
    {
        public virtual int  UpdatePriority => 0;
        public         uint VersionNumber  { get; protected set; }
        
        public virtual  void Dispose() { }
    }
}