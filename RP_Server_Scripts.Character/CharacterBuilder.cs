using System;
using RP_Server_Scripts.Database.Character;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Character
{
    internal class CharacterBuilder
    {
        private readonly INpcDefList _NpcDefList;

        public CharacterBuilder(INpcDefList npcDefList)
        {
            _NpcDefList = npcDefList ?? throw new ArgumentNullException(nameof(npcDefList));
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

            return new HumanCharacter(characterEntity.CharacterId)
            {
                Name = characterEntity.CharacterName,
                Template = _NpcDefList.GetByCode(characterEntity.TemplateName),
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

            return new NonHumanCharacter(characterEntity.CharacterId)
            {
                Name = characterEntity.CharacterName,
                Template = _NpcDefList.GetByCode(characterEntity.TemplateName)
            };
        }
    }
}
