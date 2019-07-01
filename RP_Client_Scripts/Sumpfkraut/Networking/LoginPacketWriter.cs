using System;
using GUC.Network;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal sealed class LoginPacketWriter
    {
        private readonly IPacketWriterFactory _PacketWriterFactory;

        public LoginPacketWriter(IPacketWriterFactory packetWriterFactory)
        {
            _PacketWriterFactory = packetWriterFactory ?? throw new ArgumentNullException(nameof(packetWriterFactory));
        }

        public PacketWriter WriteLogin(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            PacketWriter writer = _PacketWriterFactory.Create(ScriptMessages.Login);
            writer.Write(userName);
            writer.Write(password);
            return writer;
        }

        public PacketWriter WriteLogout()
        {
            return _PacketWriterFactory.Create(ScriptMessages.Logout);
        }
    }
}
