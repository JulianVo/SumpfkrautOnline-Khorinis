using System;
using System.Collections.Concurrent;
using GUC.Network;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Network
{
    public class PacketWriterPool : IPacketWriterPool
    {
        private readonly ConcurrentBag<ScriptPacketWriter> _Writers = new ConcurrentBag<ScriptPacketWriter>();

        private readonly ILogger _Log;

        public PacketWriterPool(ILoggerFactory loggerFactory)
        {
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public ScriptPacketWriter GetScriptMessageStream()
        {
            if (!_Writers.TryTake(out ScriptPacketWriter writer))
            {
                writer = CreateNewPacketWriter();
            }
            writer.Reset();
            writer.Write((byte) ServerMessages.ScriptMessage);
            writer.InUse = true;
            return writer;
        }

        public ScriptPacketWriter GetScriptMessageStream(RP_Shared_Script.ScriptMessages id)
        {
            var s = GetScriptMessageStream();
            s.Write((byte)id);
            return s;
        }

        private ScriptPacketWriter CreateNewPacketWriter()
        {
            _Log.Info($"New pooled {nameof(ScriptPacketWriter)} created.");
            return new ScriptPacketWriter(this);
        }

        public void ReturnObject(ScriptPacketWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (!writer.InUse)
            {
                throw new InvalidOperationException("The object was already returned to the pool(Do not dispose and return manually)!");
            }

            writer.InUse = false;
            _Writers.Add(writer);
        }
    }
}