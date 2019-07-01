using System;
using GUC.Network;
using GUC.Scripts.Sumpfkraut.Networking;
using RP_Shared_Script;

namespace GUC.Scripts.Account
{
    internal sealed class AccountCreationMessageWriter
    {
        private readonly IPacketWriterFactory _PacketWriterFactory;

        public AccountCreationMessageWriter(IPacketWriterFactory packetWriterFactory)
        {
            _PacketWriterFactory = packetWriterFactory ?? throw new ArgumentNullException(nameof(packetWriterFactory));
        }

        public PacketWriter Write(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            var packet = _PacketWriterFactory.Create(ScriptMessages.CreateAccount);
            packet.Write(userName);
            packet.Write(password);
            return packet;
        }
    }
}
