using System;
using System.Collections.Generic;
using System.Threading;

namespace GUC.Scripting
{
    /// <summary>
    /// A Dispatcher (comparable to the one used in  Winforms and WPF) allows events to be
    /// invoked from the worker <see cref="Thread"/>s into the main <see cref="Thread"/>
    /// that runs the server loop.
    /// <remarks>This is needed as long as the server is not completely adapted to multi threading(thread safety).</remarks>
    /// </summary>
    public static class GUCDispatcher
    { 
        /// <summary>
        /// Lock object for locking the <see cref="Queue{T}"/> of <see cref="Action"/>s.
        /// </summary>
        private static readonly object _Lock = new object();

        /// <summary>
        /// <see cref="Queue{T}"/> of <see cref="Action"/>s that should be dispatched into the main <see cref="Thread"/>.
        /// </summary>
        private static readonly Queue<Action> _QueuedActions = new Queue<Action>();

        /// <summary>
        /// Storing the id of the main <see cref="Thread"/> for comparison in the <see cref="CheckAccess"/> method.
        /// </summary>
        private static int _ThreadId = Int32.MinValue;

        /// <summary>
        /// Dispatches all <see cref="Action"/>s that have been queued since the last call to this method.
        /// </summary>
        internal static void Execute()
        {
            _ThreadId = Thread.CurrentThread.ManagedThreadId;
            Queue<Action> copiedQueue;
            lock (_Lock)
            {
                copiedQueue = new Queue<Action>(_QueuedActions);
                _QueuedActions.Clear();
            }

            while (copiedQueue.Count > 0)
            {
                copiedQueue.Dequeue().Invoke();
            }
        }

        /// <summary>
        /// Checks if the current <see cref="Thread"/> is the <see cref="Thread"/> that runs the server loop(which means that server APIs can be called directly).
        /// </summary>
        /// <returns>Return true if the current <see cref="Thread"/> is the <see cref="Thread"/> that runs the server loop.</returns>
        public static bool CheckAccess() => _ThreadId == Thread.CurrentThread.ManagedThreadId;


        /// <summary>
        /// Queues an action to be dispatched into the main <see cref="Thread"/> of the server as soon as possible.
        /// </summary>
        /// <param name="action">The action that should be dispatched.</param>
        public static void EnqueueAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            lock (_Lock)
            {
                _QueuedActions.Enqueue(action);
            }
        }

        /// <summary>
        /// Checks if the current <see cref="Thread"/> is the main <see cref="Thread"/> of the server and either invokes the <see cref="Action"/> directly(if the <see cref="Thread"/>s match)
        /// or queues it to be dispatched into the main <see cref="Thread"/>(if they do not match).
        /// </summary>
        /// <param name="action"></param>
        public static void RunOrEnqueue(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                EnqueueAction(action);
            }
        }
    }
}
