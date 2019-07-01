using System;
using GUC.Network;
using RP_Shared_Script;

namespace GUC.Scripts.ReusedClasses
{
    public class CharCreationInfo
    {
        public string Name = "Spieler";
        public HumBodyMeshs BodyMesh = HumBodyMeshs.HUM_BODY_NAKED0;
        public HumBodyTexs BodyTex = HumBodyTexs.G1Hero;
        public HumHeadMeshs HeadMesh = HumHeadMeshs.HUM_HEAD_PONY;
        public HumHeadTexs HeadTex = HumHeadTexs.Face_N_Player;
        public HumVoices Voice = HumVoices.Hero;

        public float BodyWidth = 1.0f;
        public float Fatness = 0.0f;

        public bool IsMale => BodyMesh == HumBodyMeshs.HUM_BODY_NAKED0;

        public void Write(PacketWriter stream)
        {
            stream.Write(Name);
            stream.Write((byte)BodyMesh);
            stream.Write((byte)BodyTex);
            stream.Write((byte)HeadMesh);
            stream.Write((byte)HeadTex);
            stream.Write((byte)Voice);
            stream.Write(BodyWidth);
            stream.Write(Fatness);
        }

        public void Read(PacketReader stream)
        {
            Name = stream.ReadString();
            BodyMesh = (HumBodyMeshs)stream.ReadByte();
            BodyTex = (HumBodyTexs)stream.ReadByte();
            HeadMesh = (HumHeadMeshs)stream.ReadByte();
            HeadTex = (HumHeadTexs)stream.ReadByte();
            Voice = (HumVoices)stream.ReadByte();

            BodyWidth = stream.ReadFloat();
            Fatness = stream.ReadFloat();
        }

        /// <summary>
        /// Checks whether  all properties have a valid value.
        /// </summary>
        /// <param name="invalidProperty">The name of the property with the invalid value.</param>
        /// <returns>True if the current <see cref="CharCreationInfo"/> only contains valid values. False if not.</returns>
        public bool Validate(out string invalidProperty, out object value)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                invalidProperty = nameof(Name);
                value = Name ?? "null";
                return false;
            }

            if (!Enum.IsDefined(typeof(HumBodyMeshs), BodyMesh))
            {
                invalidProperty = nameof(BodyMesh);
                value = (int)BodyMesh;
                return false;
            }

            if (!Enum.IsDefined(typeof(HumBodyTexs), BodyTex))
            {
                invalidProperty = nameof(BodyTex);
                value = (int)BodyTex;
                return false;
            }

            if (!Enum.IsDefined(typeof(HumHeadMeshs), HeadMesh))
            {
                invalidProperty = nameof(HeadMesh);
                value = (int)HeadMesh;
                return false;
            }

            if (!Enum.IsDefined(typeof(HumHeadTexs), HeadTex))
            {
                invalidProperty = nameof(HeadTex);
                value =(int) HeadTex;
                return false;
            }

            if (!Enum.IsDefined(typeof(HumVoices), Voice))
            {
                invalidProperty = nameof(Voice);
                value = (int)Voice;
                return false;
            }

            if (BodyWidth < 0.9F || BodyWidth > 1.1F)
            {
                invalidProperty = nameof(BodyWidth);
                value = BodyWidth;
                return false;
            }

            if (Fatness < -1F || Fatness > 1F)
            {
                invalidProperty = nameof(Fatness);
                value = Fatness;
                return false;
            }

            invalidProperty = null;
            value = null;
            return true;
        }
    }
}
