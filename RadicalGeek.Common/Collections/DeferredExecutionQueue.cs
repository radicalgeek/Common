using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace RadicalGeek.Common.Collections
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification="It behaves like a Queue.")]
    public sealed class DeferredExecutionQueue : IDisposable
    {
        readonly TimeSpan doNotPoll = TimeSpan.FromMilliseconds(-1);

        private readonly Queue<Task> privateQueue = new Queue<Task>();
        private Timer executionTimer;
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(10);
        private bool executingNow;
        
        private static readonly CacheList<string, DeferredExecutionQueue> namedQueues = new CacheList<string, DeferredExecutionQueue>(s => new DeferredExecutionQueue());

        public static DeferredExecutionQueue GetNamedQueue(string queueName)
        {
            return namedQueues[queueName];
        }

        public DeferredExecutionQueue()
        {
        }

        public DeferredExecutionQueue(TimeSpan timeout)
        {
            this.timeout = timeout;
        }

        private void ExecuteActions(object state)
        {
            if (!executingNow)
            {
                executingNow = true;
                try
                {
                    Queue<Task> queue = state as Queue<Task>;
                    if (queue != null)
                        while (queue.Count > 0)
                            queue.Dequeue().Start();
                }
                finally
                {
                    executingNow = false;
                }
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Task's lifetime extends far beyond the declaring scope")]
        public void Add(Action<object> method, object args)
        {
            if (executionTimer == null)
                executionTimer = new Timer(ExecuteActions, privateQueue, timeout, doNotPoll);
            else
                executionTimer.Change(timeout, doNotPoll);
            privateQueue.Enqueue(new Task(method, args));
        }

        public void Dispose()
        {
            executionTimer.Change(TimeSpan.MaxValue, doNotPoll);
            executionTimer.Dispose();
            if (!executingNow)
                ExecuteActions(privateQueue);
        }
    }
}
