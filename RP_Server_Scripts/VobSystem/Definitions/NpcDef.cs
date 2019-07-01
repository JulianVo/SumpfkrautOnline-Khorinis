using System.Diagnostics;
using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    [DebuggerDisplay("NpcDef:{CodeName}")]
    public class NpcDef : NamedVobDef, NPCInstance.IScriptNPCInstance
    {
        public override VobType VobType => VobType.NPC;

        public new NPCInstance BaseDef => (NPCInstance) base.BaseDef;

        public override string Name
        {
            get => base.Name;
            set
            {
                BaseDef.Name = value;
                base.Name = value;
            }
        }

        public string BodyMesh
        {
            get => BaseDef.BodyMesh;
            set => BaseDef.BodyMesh = value;
        }

        public int BodyTex
        {
            get => BaseDef.BodyTex;
            set => BaseDef.BodyTex = value;
        }

        public string HeadMesh
        {
            get => BaseDef.HeadMesh;
            set => BaseDef.HeadMesh = value;
        }

        public int HeadTex
        {
            get => BaseDef.HeadTex;
            set => BaseDef.HeadTex = value;
        }

        public byte Guild
        {
            get => BaseDef.Guild;
            set => BaseDef.Guild = value;
        }

        public NpcDef(string codeName, IBaseDefFactory baseDefFactory,   IVobDefRegistration registration) : base(codeName, baseDefFactory,  registration)
        {
        }
    }
}