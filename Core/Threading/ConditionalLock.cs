using System;
using System.Threading;

namespace Core.Threading
{
    /// <summary>
    ///     A struct that provides lock semantics while allowing the choice of whether to take the lock to be
    ///     chosen at runtime.
    /// </summary>
    public struct ConditionalLock : IDisposable
    {
        #region Instance Fields

        private object lockObject;

        #endregion


        #region Static Methods

        public static ConditionalLock LockIf(bool condition, object objectToLockOn)
        {
            if (objectToLockOn == null)
            {
                throw new ArgumentNullException(nameof(objectToLockOn));
            }

            if (condition)
            {
                if (Monitor.TryEnter(objectToLockOn, Locks.MonitorEnterTimeout))
                {
                    throw new TimeoutException($"A {nameof(ConditionalLock)} exceeded the timeout specified " +
                                               $"in {nameof(Locks)}.{nameof(Locks.MonitorEnterTimeout)}. ");
                }

                return new ConditionalLock(objectToLockOn);
            }
            else
            {
                return default(ConditionalLock);
            }
        }

        #endregion


        #region Instance Methods

        public void Dispose()
        {
            if (lockObject != null)
            {
                Monitor.Exit(lockObject);
            }
        }

        #endregion


        #region Constructors

        internal ConditionalLock(object objectToLockOn)
        {
            lockObject = objectToLockOn;
        }

        #endregion
    }
}