using System;
using System.Collections.Generic;
using GUC.Network;

namespace GUC.Scripts.Arena
{
    public sealed class PlayerInfo
    {
        static readonly Dictionary<int, PlayerInfo> _Players = new Dictionary<int, PlayerInfo>();
        public static IEnumerable<PlayerInfo> GetInfos() { return _Players.Values; }

        public static PlayerInfo HeroInfo { get; } = new PlayerInfo();

        public static bool TryGetInfo(int id, out PlayerInfo info)
        {
            return _Players.TryGetValue(id, out info);
        }

        public int Id { get; private set; } = -1;

        public string Name { get; private set; }


        public static void ReadHeroInfo(PacketReader stream)
        {
            HeroInfo.Id = stream.ReadByte();
        }

        public static event Action OnPlayerListChange;
        public static void ReadPlayerInfoMessage(PacketReader stream)
        {
            int id = stream.ReadByte();
            if (!_Players.TryGetValue(id, out PlayerInfo pi))
            {
                pi = id == HeroInfo.Id ? HeroInfo : new PlayerInfo { Id = id };
                _Players.Add(id, pi);
            }

            pi.Name = stream.ReadString();

            OnPlayerListChange?.Invoke();
        }

        public static void ReadPlayerQuitMessage(PacketReader stream)
        {
            _Players.Remove(stream.ReadByte());
        }

        public static void ReadPlayerInfoTeam(PacketReader stream)
        {
            int id = stream.ReadByte();
            if (!_Players.TryGetValue(id, out PlayerInfo pi))
                return;

            OnPlayerListChange?.Invoke();

        }
    }
}
