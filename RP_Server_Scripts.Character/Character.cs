using GUC.Types;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.Character
{
    public abstract class Character
    {
        private readonly CharacterService _CharacterService;


        internal Character(int characterId, CharacterService characterService)
        {
            _CharacterService = characterService;
            CharacterId = characterId;
        }

        internal CharacterMapping CharacterMapping { get; set; }

        public int CharacterId { get; }

        public string Name { get; internal set; } = "Player";

        public Vec3f LastKnownPosition { get; internal set; }

        public Angles Rotation { get; internal set; }

        public bool IsMapped { get; internal set; }

        public bool IsValid { get; internal set; }

        public NpcDef Template { get; internal set; }

        public WorldInst World { get; internal set; }

        public CharacterMapping SpawnAndMap() => _CharacterService.SpawnAndMapCharacter(this);

        public void RemoveMapping() => _CharacterService.RemoveMapping(this);

        public void Save()
        {

        }

        public bool TryGetMapping(out CharacterMapping mapping)
        {
            mapping = CharacterMapping;
            return CharacterMapping != null;
        }

    }
}
