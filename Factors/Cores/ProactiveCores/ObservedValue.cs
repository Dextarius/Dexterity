using Core.Factors;

namespace Factors.Cores.ProactiveCores
{
    public readonly ref struct ObservedValue<T>
    {
        private readonly IInvolved factor;
        private readonly long      flags;
        private readonly T         value;
        
        public  readonly T Value
        {
            get
            {
                factor.NotifyInvolved(flags);
                return value;
            }
        }

        public static implicit operator T(ObservedValue<T> value) => value.Value;
    }
}