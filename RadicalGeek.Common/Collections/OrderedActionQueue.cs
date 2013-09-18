using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace RadicalGeek.Common.Collections
{
    public class OrderedActionQueue : IDisposable
    {
        private readonly Timer actionPollTimer;
        private readonly string logPath = ConfigurationManager.AppSettings["LogPath"];
        public Action<string> WriteLog; 
        private readonly List<List<RetryableAction>> actionQueue = new List<List<RetryableAction>>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (actionPollTimer != null)
                {
                    ExecuteWaitingActions();
                    actionPollTimer.Dispose();
                }
        }

        ~OrderedActionQueue()
        {
            Dispose(false);
        }

        public OrderedActionQueue()
        {
            actionPollTimer = new Timer(ExecuteWaitingActions, null, 100, 100);
        }

        private void ExecuteWaitingActions(object state = null)
        {
            actionPollTimer.Change(-1, -1);
            List<RetryableAction>[] currentBlock;
            lock (actionQueue)
            {
                currentBlock = new List<RetryableAction>[actionQueue.Count];
                actionQueue.CopyTo(0, currentBlock, 0, actionQueue.Count);
                actionQueue.Clear();
            }

            foreach (List<RetryableAction> expressions in currentBlock)
            {
                foreach (RetryableAction expression in expressions.Where(action => !action.Completed))
                {
                    try
                    {
                        if (expression.TryCount == 5)
                        {
                            // record that this has been attempetd too many times
                            Console.WriteLine("Too many attempts.");
                            foreach (Exception exception in expression.Exceptions)
                                Console.WriteLine(exception.Message);
                            break;
                        }
                        expression.TryCount++;
                        expression.Action();
                        expression.Completed = true;
                    }
                    catch (Exception ex)
                    {
                        expression.Exceptions.Add(ex);
                        Add(expressions);
                        break;
                    }
                }
            }

            actionPollTimer.Change(100, 100);
        }

        public void Add(List<Action> expressionList)
        {
            lock (actionQueue)
                actionQueue.Add(expressionList.Select(a => new RetryableAction { Action = a }).ToList());
        }

        private void Add(List<RetryableAction> expressionList)
        {
            lock (actionQueue)
                actionQueue.Add(expressionList);
        }

        public void Add(Action expressionList)
        {
            lock (actionQueue)
                actionQueue.Add(new List<RetryableAction> { new RetryableAction { Action = expressionList } });
        }

        private class RetryableAction
        {
            public Action Action { get; set; }
            public int TryCount { get; set; }
            public readonly List<Exception> Exceptions = new List<Exception>();
            public bool Completed;
        }
    }
}