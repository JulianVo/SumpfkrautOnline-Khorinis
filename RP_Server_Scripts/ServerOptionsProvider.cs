using GUC.Options;

namespace RP_Server_Scripts
{
    public sealed class ServerOptionsProvider
    {
        public int Slot => ServerOptions.Slots;
    }
}
