using System;
using GUC.Scripting;

namespace RP_Server_Scripts.Threading
{
    /// <summary>
    /// Implementation of <see cref="IMainThreadDispatcher"/> that uses the <see cref="GUCDispatcher"/>.
    /// </summary>
    internal sealed class MainThreadDispatcher : IMainThreadDispatcher
    {
        /// <inheritdoc />
        public bool CheckAccess() => GUCDispatcher.CheckAccess();


        /// <inheritdoc />
        public void EnqueueAction(Action action) => GUCDispatcher.EnqueueAction(action);


        /// <inheritdoc />
        public void RunOrEnqueue(Action action) => GUCDispatcher.RunOrEnqueue(action);

    }
}