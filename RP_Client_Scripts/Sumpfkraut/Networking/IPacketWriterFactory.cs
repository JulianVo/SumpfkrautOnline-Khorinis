using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUC.Network;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    public interface IPacketWriterFactory
    {
        PacketWriter Create();

        PacketWriter Create(ScriptMessages messageId);
    }
}
