using System;
using System.Threading.Tasks;

namespace MASACore.Core.LifeCycle
{
    public class ExclusiveScheduler
    {
        private readonly TaskFactory _exclusiveTaskFactory;

        public ExclusiveScheduler()
        {
            var concurrentExclusiveScheduler = new ConcurrentExclusiveSchedulerPair();
            _exclusiveTaskFactory = new TaskFactory(concurrentExclusiveScheduler.ExclusiveScheduler);
        }

        /// <summary>
        /// Tasks run on an ExclusiveScheduler instance will run serially
        /// </summary>
        /// <param name="action">Async Task without a return value</param>
        /// <returns></returns>
        public Task RunExclusivelyAsync(Func<Task> action)
        {
            return _exclusiveTaskFactory.StartNew(action).Unwrap();
        }

        /// <summary>
        /// Tasks run on an ExclusiveScheduler instance will run serially
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">Async Task with a return value</param>
        /// <returns></returns>
        public Task<T> RunExclusivelyAsync<T>(Func<Task<T>> action)
        {
            return _exclusiveTaskFactory.StartNew(action).Unwrap();
        }
    }
}
