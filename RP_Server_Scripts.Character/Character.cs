using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Character
{
    public abstract class Character
    {
        internal Character(int characterId)
        {
            CharacterId = characterId;
        }

        public int CharacterId { get; }

        public string Name { get; internal set; } = "Player";

        /// <summary>
        /// Indicates whether the given character is loaded into a game world.
        /// </summary>
        public bool IsLoaded { get; internal set; }

        public NpcDef Template { get; internal set; }
    }
}
