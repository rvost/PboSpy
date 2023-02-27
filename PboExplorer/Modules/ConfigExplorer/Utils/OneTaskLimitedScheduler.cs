namespace PboExplorer.Modules.ConfigExplorer.Utils;

/// <summary>
/// Task Scheduler is based on LimitedConcurrencyLevelTaskScheduler from
/// Source:
/// https://code.msdn.microsoft.com/windowsdesktop/Samples-for-Parallel-b4b76364
/// 
/// This scheduler ensures that only task is being executed at any time and
/// will clear the previously scheduled tasks (if any) before queing a new task.
/// 
/// This ensures that only the most recently queued task is processed at any time.
/// </summary>
public class OneTaskLimitedScheduler : TaskScheduler
{
    /// <summary>Whether the current thread is processing work items.</summary>
    [ThreadStatic]
    private static bool _currentThreadIsProcessingItems;
    /// <summary>The list of tasks to be executed.</summary>
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)
    /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
    private readonly int _maxDegreeOfParallelism;
    /// <summary>Whether the scheduler is currently processing work items.</summary>
    private int _delegatesQueuedOrRunning = 0; // protected by lock(_tasks)

    /// <summary>
    /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
    /// specified degree of parallelism.
    /// </summary>
    public OneTaskLimitedScheduler()
    {
        _maxDegreeOfParallelism = 1;
    }

    /// <summary>Queues a task to the scheduler.</summary>
    /// <param name="task">The task to be queued.</param>
    protected sealed override void QueueTask(Task task)
    {
        // Add the task to the list of tasks to be processed.  If there aren't enough
        // delegates currently queued or running to process tasks, schedule another.
        lock (_tasks)
        {
            if (_tasks.Count > 0)  // Always empty the queue since we want only 1 task
                _tasks.Clear();  // and the last task to be processed at any time...

            _tasks.AddLast(task);
            if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
            {
                ++_delegatesQueuedOrRunning;
                NotifyThreadPoolOfPendingWork();
            }
        }
    }

    /// <summary>
    /// Informs the ThreadPool that there's work to be executed for this scheduler.
    /// </summary>
    private void NotifyThreadPoolOfPendingWork()
    {
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            // Note that the current thread is now processing work items.
            // This is necessary to enable inlining of tasks into this thread.
            _currentThreadIsProcessingItems = true;
            try
            {
                // Process all available items in the queue.
                while (true)
                {
                    Task item;
                    lock (_tasks)
                    {
                        // When there are no more items to be processed,
                        // note that we're done processing, and get out.
                        if (_tasks.Count == 0)
                        {
                            --_delegatesQueuedOrRunning;
                            break;
                        }

                        // Get the next item from the queue
                        item = _tasks.First.Value;
                        _tasks.RemoveFirst();
                    }

                    // Execute the task we pulled out of the queue
                    base.TryExecuteTask(item);
                }
            }
            // We're done processing items on the current thread
            finally { _currentThreadIsProcessingItems = false; }
        }, null);
    }

    /// <summary>Attempts to execute the specified task on the current thread.</summary>
    /// <param name="task">The task to be executed.</param>
    /// <param name="taskWasPreviouslyQueued"></param>
    /// <returns>Whether the task could be executed on the current thread.</returns>
    protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        // If this thread isn't already processing a task, we don't support inlining
        if (!_currentThreadIsProcessingItems) return false;

        // If the task was previously queued, remove it from the queue
        if (taskWasPreviouslyQueued) TryDequeue(task);

        // Try to run the task.
        return base.TryExecuteTask(task);
    }

    /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
    /// <param name="task">The task to be removed.</param>
    /// <returns>Whether the task could be found and removed.</returns>
    protected sealed override bool TryDequeue(Task task)
    {
        lock (_tasks) return _tasks.Remove(task);
    }

    /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
    public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

    /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
    /// <returns>An enumerable of the tasks currently scheduled.</returns>
    protected sealed override IEnumerable<Task> GetScheduledTasks()
    {
        bool lockTaken = false;
        try
        {
            Monitor.TryEnter(_tasks, ref lockTaken);
            if (lockTaken) return _tasks.ToArray();
            else throw new NotSupportedException();
        }
        finally
        {
            if (lockTaken) Monitor.Exit(_tasks);
        }
    }
}

/// <summary>
/// Implements a background thread processor that will process
/// ONLY ONE task at any time.
/// </summary>
internal class OneTaskProcessor : IDisposable
{
    #region fields
    private readonly OneTaskLimitedScheduler _myTaskScheduler;
    private readonly List<TaskItem> _myTaskList;
    private readonly SemaphoreSlim _Semaphore;

    private bool _Disposed;
    #endregion fields

    #region constructors
    /// <summary>
    /// Class constructor
    /// </summary>
    public OneTaskProcessor()
    {
        _myTaskScheduler = new OneTaskLimitedScheduler();
        _myTaskList = new List<TaskItem>();

        _Semaphore = new SemaphoreSlim(1, 1);
        _Disposed = false;
    }
    #endregion constructors

    #region methods
    /// <summary>
    /// Implements the <see cref="IDisposable"/> interface.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Schedules a function that returns an int value for execution in a
    /// one task at a time background thread framework.
    /// 
    /// All previously scheduled tasks are cancelled (if any).
    /// Their is only one task being executed at any time (via custom <see cref="TaskScheduler"/>.
    /// </summary>
    /// <param name="funcToExecute"></param>
    /// <param name="tokenSource"></param>
    /// <returns></returns>
    internal async Task<int> ExecuteOneTask(Func<int> funcToExecute,
                                            CancellationTokenSource tokenSource)
    {
        try
        {
            for (int i = 0; i < _myTaskList.Count; i++)
            {
                if (_myTaskList[i].Cancellation != null)
                {
                    _myTaskList[i].Cancellation.Cancel();
                    _myTaskList[i].Cancellation.Dispose();
                }
            }
        }
        catch (AggregateException e)
        {
            Console.WriteLine("\nAggregateException thrown with the following inner exceptions:");
            // Display information about each exception. 
            foreach (var v in e.InnerExceptions)
            {
                if (v is TaskCanceledException)
                    Console.WriteLine("   TaskCanceledException: Task {0}",
                                      ((TaskCanceledException)v).Task.Id);
                else
                    Console.WriteLine("   Exception: {0}", v.GetType().Name);
            }
            Console.WriteLine();
        }
        finally
        {
            _myTaskList.Clear();
        }

        await _Semaphore.WaitAsync();
        try
        {
            // Do the search and return number of results as int
            var t = Task.Factory.StartNew<int>(funcToExecute,
                                                tokenSource.Token,
                                                TaskCreationOptions.LongRunning,
                                                _myTaskScheduler);

            _myTaskList.Add(new TaskItem(t, tokenSource));

            await t;

            return t.Result;
        }
        finally
        {
            _Semaphore.Release();
        }
    }

    /// <summary>
    /// The bulk of the clean-up code is implemented here.
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed == false)
        {
            if (disposing == true)
            {
                try
                {
                    for (int i = 0; i < _myTaskList.Count; i++)
                    {
                        if (_myTaskList[i].Cancellation != null)
                        {
                            _myTaskList[i].Cancellation.Cancel();
                            _myTaskList[i].Cancellation.Dispose();
                        }
                    }

                    _Semaphore.Dispose();
                }
                catch { }
            }

            _Disposed = true;
        }
    }
    #endregion methods

    #region private classes
    /// <summary>
    /// Implements a taskitem wich consists of a task and its <see cref="CancellationTokenSource"/>.
    /// </summary>
    private class TaskItem
    {
        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="taskToProcess"></param>
        /// <param name="cancellation"></param>
        public TaskItem(Task taskToProcess,
                        CancellationTokenSource cancellation)
            : this()
        {
            TaskToProcess = taskToProcess;
            Cancellation = cancellation;
        }

        /// <summary>
        /// Class constructor.
        /// </summary>
        protected TaskItem()
        {
            TaskToProcess = null;
            Cancellation = null;
        }

        /// <summary>
        /// Gets the task that shoulf be processed.
        /// </summary>
        public Task TaskToProcess { get; }

        /// <summary>
        /// Gets the <seealso cref="CancellationTokenSource"/> that can
        /// be used to cancel this task.
        /// </summary>
        public CancellationTokenSource Cancellation { get; }
    }
    #endregion private Classes
}
