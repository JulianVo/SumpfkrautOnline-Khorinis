using System;
using System.Collections.Generic;
using System.Linq;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal class ScriptMessageHandlerSelector : IScriptMessageHandlerSelector
    {
        private readonly Dictionary<RP_Shared_Script.ScriptMessages, IScriptMessageHandler> _Handlers;

        public ScriptMessageHandlerSelector(IEnumerable<IScriptMessageHandler> handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            _Handlers = handlers.ToDictionary(v => v.SupportedMessage);
        }

        public bool TryGetMessageHandler(RP_Shared_Script.ScriptMessages message, out IScriptMessageHandler handler)
        {
            return _Handlers.TryGetValue(message, out handler);
        }
    }
}