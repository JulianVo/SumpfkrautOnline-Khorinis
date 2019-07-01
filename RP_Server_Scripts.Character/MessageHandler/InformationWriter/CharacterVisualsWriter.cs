using System;
using GUC.Network;

namespace RP_Server_Scripts.Character.MessageHandler.InformationWriter
{
   public sealed class CharacterVisualsWriter
    {
        public void WriteCharacter(PacketWriter writer, Character character)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            try
            {
                //General character visuals information(name and template)
                writer.Write(character.CharacterId );
                writer.Write(character.Name);
                writer.Write(character.Template.ID);

                //Its a human character which has a lot more customization options that need to be transmitted.
                if (character is HumanCharacter humanCharacter)
                {
                    //Its a human character
                    writer.Write(true);

                    writer.Write((byte) humanCharacter.HumanVisuals.BodyMesh);
                    writer.Write((byte)humanCharacter.HumanVisuals.BodyTex);
                    writer.Write((byte)humanCharacter.HumanVisuals.HeadMesh);
                    writer.Write((byte)humanCharacter.HumanVisuals.HeadTex);
                    writer.Write((byte) humanCharacter.HumanVisuals.Voice);
                    writer.Write(humanCharacter.HumanVisuals.BodyWidth);
                    writer.Write(humanCharacter.HumanVisuals.Fatness);
                }
                else
                {
                    //It a non human character(the template codename is enough to recreate the visuals in the client).
                    writer.Write(false);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Something went wrong while writing character visuals information to a {nameof(PacketWriter)}",e);
            }
        }
    }
}
