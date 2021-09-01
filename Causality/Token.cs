using System;

namespace Causality
{
    public partial class Observer
    {
        public readonly struct ConditionalToken : IDisposable
        {
            //private readonly CausalEvent temporaryEvent;

            public void Dispose()
            {
                //temporaryEvent.Conclude();
            }
        }
    }
}