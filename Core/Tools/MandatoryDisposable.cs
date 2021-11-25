using System;

namespace Core.Tools
{
    public class MandatoryDisposable : IDisposable
    {
        private bool wasDisposed;

        public virtual void Dispose()
        {
            wasDisposed = true;
        }

        public void ResetDisposal() => wasDisposed = false;


        ~MandatoryDisposable()
        {
            if (wasDisposed is false)
            {
                throw new InvalidOperationException(
                    $"A {GetType().Name} was not disposed of before being garbage collected");
            }
        }
    }
}