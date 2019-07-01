using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RP_Server_Scripts.Properties;

namespace RP_Server_Scripts.Chat
{
    internal class HelpChatCommand : IChatCommand
    {
        private readonly Dictionary<string, IChatCommand> _ChatCommands = new Dictionary<string, IChatCommand>();


        public IEnumerable<IChatCommand> ChatCommands
        {
            //Use property injection to allow circular references.
            get => _ChatCommands.Values.Distinct();
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_ChatCommands.Count > 0)
                {
                    throw new InvalidOperationException("The property must not be set after startup");
                }

                //Register all chat commands.
                foreach (var chatCommand in value)
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
            }
        }


        public string[] Identifiers => new[] { "help", "h" };
        public void HandleCommand(Client.Client sender, string[] parameter)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            //It is the help command, handle it.
            if (parameter.Length > 0)
            {
                // Send the description for the given command to the client.
                if (_ChatCommands.TryGetValue(parameter[0].ToUpperInvariant(), out IChatCommand helpedCommand))
                {
                    sender.SendServerNotification(string.Format(Resources.Msg_CommandHelp,
                        helpedCommand.UsageText ?? Resources.Msg_MissingText,
                        helpedCommand.DescriptionText ?? Resources.Msg_MissingText));
                }
                else
                {
                    sender.SendChatMessage(string.Format(Resources.Msg_CommandNotExist, parameter[0]));
                }
            }
            else
            {
                //No specific command identifier was given. List all registered commands.
                var sb = new StringBuilder("All available commands: ");
                foreach (var identifier in _ChatCommands.Keys)
                {
                    sb.Append(identifier);
                    sb.Append(";");
                }
                sender.SendServerNotification(sb.ToString());
            }
        }

        public string DescriptionText => Resources.Msg_HelpCommand_Description;
        public string UsageText => "/help <command>";
    }
}
