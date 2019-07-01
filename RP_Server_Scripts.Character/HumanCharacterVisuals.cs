using RP_Shared_Script;

namespace RP_Server_Scripts.Character
{
    public sealed class HumanCharacterVisuals
    {
        public HumBodyMeshs BodyMesh { get; internal set; } = HumBodyMeshs.HUM_BODY_NAKED0;
        public HumBodyTexs BodyTex { get; internal set; } = HumBodyTexs.G1Hero;
        public HumHeadMeshs HeadMesh { get; internal set; } = HumHeadMeshs.HUM_HEAD_PONY;
        public HumHeadTexs HeadTex { get; internal set; } = HumHeadTexs.Face_N_Player;
        public HumVoices Voice { get; internal set; } = HumVoices.Hero;

        public float BodyWidth { get; internal set; } = 1.0f;
        public float Fatness { get; internal set; } = 0.0f;
    }
}