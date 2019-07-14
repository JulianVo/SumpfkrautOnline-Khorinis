using System.Diagnostics;
using GUC.Types;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.Character
{
    [DebuggerDisplay("Character: {" + nameof(Name) + "}")]
    public abstract class Character
    {
        private readonly CharacterService _CharacterService;
        private Vec3f _LastKnownPosition;
        private Angles _Rotation;
        private NpcDef _Template;
        private WorldInst _World;


        internal Character(int characterId, CharacterService characterService)
        {
            _CharacterService = characterService;
            CharacterId = characterId;
        }

        internal CharacterMapping CharacterMapping { get; set; }

        public int CharacterId { get; }

        public string Name { get; internal set; } = "Player";

        public Vec3f LastKnownPosition
        {
            get
            {
                if (IsMapped)
                {
                    _LastKnownPosition = CharacterMapping.CharacterNpc.GetPosition();
                }
                return _LastKnownPosition;
            }
            internal set => _LastKnownPosition = value;
        }

        public Angles Rotation
        {
            get
            {
                if (IsMapped)
                {
                    _Rotation = CharacterMapping.CharacterNpc.GetAngles();
                }
                return _Rotation;
            }
            internal set => _Rotation = value;
        }

        public bool IsMapped { get; internal set; }

        public bool IsValid { get; internal set; }

        public NpcDef Template
        {
            get
            {
                if (IsMapped)
                {
                    _Template = CharacterMapping.CharacterNpc.Template;
                }
                return _Template;
            }
            internal set => _Template = value;
        }

        public WorldInst World
        {
            get
            {
                if (IsMapped)
                {
                    _World = CharacterMapping.CharacterNpc.World;
                }
                return _World;
            }
            internal set => _World = value;
        }

        public CharacterMapping SpawnAndMap() => _CharacterService.SpawnAndMapCharacter(this);

        public void RemoveMapping()
        {
            _CharacterService.RemoveMapping(this);
        }

        public virtual async void Save()
        {
            await _CharacterService.SaveCharacterAsync(this);
        }

        public bool TryGetMapping(out CharacterMapping mapping)
        {
            mapping = CharacterMapping;
            return CharacterMapping != null;
        }

    }
}
