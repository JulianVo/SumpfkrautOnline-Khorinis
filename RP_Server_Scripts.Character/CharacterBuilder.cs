using System;
using GUC.Types;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.Character
{
    internal class CharacterBuilder
    {
        private readonly INpcDefList _NpcDefList;
        private readonly WorldList _WorldList;
        private readonly CharacterService _CharacterService;

        public CharacterBuilder(INpcDefList npcDefList,WorldList worldList,CharacterService characterService)
        {
            _NpcDefList = npcDefList ?? throw new ArgumentNullException(nameof(npcDefList));
            _WorldList = worldList ?? throw new ArgumentNullException(nameof(worldList));
            _CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
        }

        public Character HumanCharacterFromEntities(CharacterEntity characterEntity,
            CharacterCustomVisualsEntity visualsEntity)
        {
            if (characterEntity == null)
            {
                throw new ArgumentNullException(nameof(characterEntity));
            }

            if (visualsEntity == null)
            {
                throw new ArgumentNullException(nameof(visualsEntity));
            }

            return new HumanCharacter(characterEntity.CharacterId, _CharacterService)
            {
                IsValid = true,
                Name = characterEntity.CharacterName,
                LastKnownPosition = new Vec3f(characterEntity.PositionX,characterEntity.PositionY,characterEntity.PositionZ),
                Rotation =new Angles(0,characterEntity.Rotation,0),
                Template = _NpcDefList.GetByCode(characterEntity.TemplateName),
                World = _WorldList.GetWorldOrFallback(characterEntity.WorldName),
                HumanVisuals = new HumanCharacterVisuals
                {
                    BodyMesh = visualsEntity.BodyMesh,
                    BodyTex = visualsEntity.BodyTex,
                    HeadMesh = visualsEntity.HeadMesh,
                    HeadTex = visualsEntity.HeadTex,
                    BodyWidth = visualsEntity.BodyWidth,
                    Fatness = visualsEntity.Fatness,
                    Voice = visualsEntity.Voice,
                }
            };
        }

        public Character NonHumanCharacterFromEntities(CharacterEntity characterEntity)
        {
            if (characterEntity == null)
            {
                throw new ArgumentNullException(nameof(characterEntity));
            }

            return new NonHumanCharacter(characterEntity.CharacterId, _CharacterService)
            {
                IsValid = true,
                Name = characterEntity.CharacterName,
                LastKnownPosition = new Vec3f(characterEntity.PositionX, characterEntity.PositionY, characterEntity.PositionZ),
                Rotation = new Angles(0, characterEntity.Rotation, 0),
                Template = _NpcDefList.GetByCode(characterEntity.TemplateName),
                World = _WorldList.GetWorldOrFallback(characterEntity.WorldName)
            };
        }
    }
}
