using System;
using System.Threading.Tasks;
using GUC.Network;
using RP_Server_Scripts.Client;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Shared_Script;

namespace RP_Server_Scripts.Authentication.MessageHandler
{
    /// <summary>
    /// A decorator implementation of <see cref="IScriptMessageHandler"/> that blocks script messages from <see cref="Client"/>s that are not logged in.
    /// </summary>
    internal class AuthenticatedMessageHandlerDecorator : IScriptMessageHandler
    {
        private readonly IScriptMessageHandler _ScriptMessageHandlerImplementation;
        private readonly AuthenticationService _AuthenticationService;
        private readonly ILogger _Log;

        public AuthenticatedMessageHandlerDecorator(IScriptMessageHandler scriptMessageHandlerImplementation, AuthenticationService authenticationService,ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _ScriptMessageHandlerImplementation = scriptMessageHandlerImplementation ?? throw new ArgumentNullException(nameof(scriptMessageHandlerImplementation));
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _Log = loggerFactory.GetLogger(GetType());
        }

        public void HandleMessage(Client.Client sender, PacketReader stream)
        {
            //Only accept messages of logged in clients.
            if (_AuthenticationService.IsLoggedIn(sender))
            {
                _ScriptMessageHandlerImplementation.HandleMessage(sender, stream);
            }
            else
            {
                _Log.Warn($"Client '{sender.SystemAddress}' send a '{SupportedMessage}' packet while not being logged in.");
                sender.Disconnect();
            }
        }

        public ScriptMessages SupportedMessage => _ScriptMessageHandlerImplementation.SupportedMessage;
    }
}

