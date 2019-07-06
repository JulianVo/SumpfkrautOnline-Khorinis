using System;
using GUC.Network;

namespace RP_Server_Scripts.Network
{
    public class ScriptPacketWriter : PacketWriter, IDisposable
    {
        private readonly PacketWriterPool _PacketWriterPool;

        public ScriptPacketWriter(int capacity, PacketWriterPool packetWriterPool) : base(capacity)
        {
            _PacketWriterPool = packetWriterPool;
        }

        public ScriptPacketWriter(PacketWriterPool packetWriterPool)
        {
            _PacketWriterPool = packetWriterPool ?? throw new ArgumentNullException(nameof(packetWriterPool));
        }

        internal bool InUse { get; set; }

        void IDisposable.Dispose()
        {
            _PacketWriterPool.ReturnObject(this);
        }
    }
}