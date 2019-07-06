using System;
using RP_Server_Scripts.Network;

namespace RP_Server_Scripts.Client
{
    internal sealed class ClientFactory : IClientFactory
    {
        private readonly IScriptMessageHandlerSelector _HandlerSelector;
        private readonly IPacketWriterPool _StreamPool;
        private readonly ClientList _ClientList;

        public ClientFactory(IScriptMessageHandlerSelector handlerSelector, IPacketWriterPool streamPool, ClientList clientList)
        {
            _HandlerSelector = handlerSelector ?? throw new ArgumentNullException(nameof(handlerSelector));
            _StreamPool = streamPool ?? throw new ArgumentNullException(nameof(streamPool));
            _ClientList = clientList ?? throw new ArgumentNullException(nameof(clientList));
        }

        public Client Create()
        {
            return new Client(_HandlerSelector, _StreamPool, _ClientList);
        }
    }
}
