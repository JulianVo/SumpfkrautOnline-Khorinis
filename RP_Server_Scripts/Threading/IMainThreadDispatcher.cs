using System;
using System.Threading;

namespace RP_Server_Scripts.Threading
{
    public interface IMainThreadDispatcher
    {
        /// <summary>
        /// Checks if the current <see cref="Thread"/> is the <see cref="Thread"/> that runs the server loop(which means that server APIs can be called directly).
        /// </summary>
        /// <returns>Return true if the current <see cref="Thread"/> is the <see cref="Thread"/> that runs the server loop.</returns>
        bool CheckAccess();


        /// <summary>
        /// Queues an action to be dispatched into the main <see cref="Thread"/> of the server as soon as possible.
        /// </summary>
        /// <param name="action">The action that should be dispatched.</param>
        void EnqueueAction(Action action);

        /// <summary>
        /// Checks if the current <see cref="Thread"/> is the main <see cref="Thread"/> of the server and either invokes the <see cref="Action"/> directly(if the <see cref="Thread"/>s match)
        /// or queues it to be dispatched into the main <see cref="Thread"/>(if they do not match).
        /// </summary>
        /// <param name="action"></param>
        void RunOrEnqueue(Action action);
    }
}
