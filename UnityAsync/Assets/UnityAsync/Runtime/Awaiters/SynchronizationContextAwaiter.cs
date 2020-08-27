using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace UnityAsync.Awaiters
{
    public struct SynchronizationContextAwaiter : INotifyCompletion
    {
        static readonly SendOrPostCallback postCallback = state => ((Action)state)();

        readonly SynchronizationContext context;

        public SynchronizationContextAwaiter(SynchronizationContext context)
        {
            this.context = context;
        }

        public bool IsCompleted => context == SynchronizationContext.Current;
        public void OnCompleted(Action continuation) => context.Post(postCallback, continuation);
        public void GetResult() { }
    }
}