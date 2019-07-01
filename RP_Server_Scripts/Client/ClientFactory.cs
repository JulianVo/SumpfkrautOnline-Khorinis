using System;
using RP_Server_Scripts.Network;

namespace RP_Server_Scripts.Client
{
    internal sealed class ClientFactory : IClientFactory
    {
        private readonly IScriptMessageHandlerSelector _HandlerSelector;
        private readonly IPacketWriterFactory _StreamFactory;
        private readonly ClientList _ClientList;

        public ClientFactory(IScriptMessageHandlerSelector handlerSelector, IPacketWriterFactory streamFactory, ClientList clientList)
        {
            _HandlerSelector = handlerSelector ?? throw new ArgumentNullException(nameof(handlerSelector));
            _StreamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
            _ClientList = clientList ?? throw new ArgumentNullException(nameof(clientList));
        }

        public Client Create()
        {
            return new Client(_HandlerSelector, _StreamFactory, _ClientList);
        }
    }
}
