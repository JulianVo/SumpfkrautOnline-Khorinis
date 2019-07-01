using Gothic.Objects;
using WinApi;

namespace Gothic
{
    public class oCSpawnManager:zCObject
    {
        public void ClearList()
        {
            Process.THISCALL<NullReturnCall>(Address, 0x0777880);
        }
    }
}
