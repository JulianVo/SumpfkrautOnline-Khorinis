using System;
using GUC.Network;
using GUC.Scripts.Character;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking.MessageHandler
{
    internal sealed class CharacterVisualsReader
    {
        private readonly NpcDefList _NpcDefList;

        public CharacterVisualsReader(NpcDefList npcDefList)
        {
            _NpcDefList = npcDefList ?? throw new ArgumentNullException(nameof(npcDefList));
        }

        public Character.Character ReadCharacter(PacketReader stream)
        {
            try
            {
                int characterId = stream.ReadInt();
                string characterName = stream.ReadString();
                int templateId = stream.ReadInt();
                bool isHumanCharacter = stream.ReadBit();

                if (!_NpcDefList.TryGetByName(templateId, out NPCDef def))
                {
                    throw new ArgumentException($"Character with name '{characterName}' has a invalid templateId(NPCDef)!");
                }


                if (isHumanCharacter)
                {
                    HumanCharacterVisuals additionalVisuals = new HumanCharacterVisuals
                    {
                        BodyMesh = (HumBodyMeshs)stream.ReadByte(),
                        BodyTex = (HumBodyTexs)stream.ReadByte(),
                        HeadMesh = (HumHeadMeshs)stream.ReadByte(),
                        HeadTex = (HumHeadTexs)stream.ReadByte(),
                        Voice = (HumVoices)stream.ReadByte(),
                        BodyWidth = stream.ReadFloat(),
                        Fatness = stream.ReadFloat(),
                    };
                    return new HumanCharacter(characterId, characterName, additionalVisuals, def);
                }
                return new NonHumanCharacter(characterId, characterName, def);
            }
            catch (Exception e)
            {
                throw new ScriptMessageHandlingException("Something went wrong while reading character visuals information from a server script message!", e);
            }
        }
    }
}