using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Factors;
using static Core.Tools.Threading;

namespace Tests.Tools
{
    public class ReactiveManipulator<T>
    {
        private readonly ManualResetEvent startEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent endEvent   = new ManualResetEvent(false);
        private readonly Proactive<T>     linkedProactive;
        
        public Reactive<T> Reactive { get; }
        
        
        public void InvalidateReactive() => Reactive.Trigger();
        public void UpdateSourceValue(T newValue) => linkedProactive.Value = newValue;

        public void TriggerReaction()
        {
           var triggerReaction = Reactive.Value;
        }

        public UpdateToken PutReactiveIntoUpdatingState()
        {
            StartNewThreadThatRuns(UpdateReactive);
            startEvent.WaitOne();

            return new UpdateToken(this);
        }

        public void EndReactiveUpdatingState()
        {
            endEvent.Set();
            startEvent.Reset();
            endEvent.Reset();
        }
        
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private void UpdateReactive()
        {
            T valueToTest = Reactive.Value;
        }
        
        private T WaitAndReturnSourceValue()
        {
            startEvent.Set();
            endEvent.WaitOne();

            return linkedProactive.Value;
        }

        public ReactiveManipulator(T valueToUse)
        {
            linkedProactive = new Proactive<T>(valueToUse);
            Reactive        = new Reactive<T>(WaitAndReturnSourceValue);
        }
        
        
        public readonly struct UpdateToken : IDisposable
        {
            private readonly ReactiveManipulator<T> creator;
            
            public void Dispose()
            {
                creator.EndReactiveUpdatingState();
            }
            

            public UpdateToken(ReactiveManipulator<T> reactiveManipulator)
            {
                creator = reactiveManipulator;
            }
        }
    }
}