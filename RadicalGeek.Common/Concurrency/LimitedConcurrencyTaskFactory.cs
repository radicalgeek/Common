using System.Threading.Tasks;

namespace RadicalGeek.Common.Concurrency
{
    public class LimitedConcurrencyTaskFactory : TaskFactory
    {
        /// <summary>
        /// Initializes a new instance of the LimitedConcurrencyTaskFactory class.
        /// </summary>
        public LimitedConcurrencyTaskFactory(int concurrencyLimit)
            : this(new LimitedConcurrencyLevelTaskScheduler(concurrencyLimit))
        {

        }
        private LimitedConcurrencyTaskFactory()
        {

        }
        private LimitedConcurrencyTaskFactory(System.Threading.CancellationToken cancellationToken)
            : base(cancellationToken)
        {

        }
        private LimitedConcurrencyTaskFactory(TaskScheduler scheduler)
            : base(scheduler)
        {

        }
        private LimitedConcurrencyTaskFactory(TaskCreationOptions creationOptions, TaskContinuationOptions continuationOptions)
            : base(creationOptions, continuationOptions)
        {

        }
        private LimitedConcurrencyTaskFactory(System.Threading.CancellationToken cancellationToken, TaskCreationOptions creationOptions, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
            : base(cancellationToken, creationOptions, continuationOptions, scheduler)
        {

        }

    }
}
