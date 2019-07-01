using System;
using System.Collections.Generic;
using System.Linq;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Properties;

namespace RP_Server_Scripts.Chat
{
    internal class ChatCommandController
    {
        private readonly Dictionary<string, IChatCommand> _ChatCommands = new Dictionary<string, IChatCommand>();

        public ChatCommandController(Chat chat, IEnumerable<IChatCommand> chatCommands, ILoggerFactory loggerFactory)
        {
            if (chat == null)
            {
                throw new ArgumentNullException(nameof(chat));
            }

            if (chatCommands == null)
            {
                throw new ArgumentNullException(nameof(chatCommands));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            chat.ChatCommandReceived += ChatOnChatCommandReceived;

            //Register all chat commands.
            foreach (var chatCommand in chatCommands)
            {
                //Register all identifiers of the command e.g. login, l
                //Skip commands without identifiers.
                foreach (var chatCommandIdentifier in chatCommand.Identifiers ?? new string[0])
                {
                    if (_ChatCommands.ContainsKey(chatCommandIdentifier.ToUpperInvariant()))
                    {
                        throw new ArgumentException($"More than one command with the identifier '{chatCommand.Identifiers}' was registered.");
                    }

                    _ChatCommands.Add(chatCommandIdentifier.ToUpperInvariant(), chatCommand);
                }
            }

            //Log the amount of registered chat commands.
            loggerFactory.GetLogger(GetType()).Info(string.Format(Resources.Msg_ChatCommandsRegistered, _ChatCommands.Count));
        }

        private void ChatOnChatCommandReceived(Chat sender, ChatCommandReceivedEventArgs e)
        {
            //The command contains a space. Split the identifier from the arguments.
            string[] parts = e.Command.Split(' ');
            string identifier = parts[0].Substring(1, parts[0].Length - 1);
            string[] args = parts.Skip(1).ToArray();

            if (_ChatCommands.TryGetValue(identifier.ToUpperInvariant(), out IChatCommand chatCommand))
            {
                chatCommand.HandleCommand(e.Sender, args);
            }
            else
            {
                e.Sender.SendChatMessage(string.Format(Resources.Msg_CommandNotExist, identifier));
            }
        }

        public int RegisteredCommands => _ChatCommands.Count();
    }
}
