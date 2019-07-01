using GUC.WorldObjects.Instances;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Definitions
{
    public partial class NPCDef : NamedVobDef, NPCInstance.IScriptNPCInstance
    {
        public override VobType VobType => VobType.NPC;


        new public NPCInstance BaseDef => (NPCInstance)base.BaseDef;

        new public string Name
        {
            get => this.name;
            set { BaseDef.Name = value; this.name = value; }
        }
        public string BodyMesh { get => BaseDef.BodyMesh;
            set => BaseDef.BodyMesh = value;
        }
        public int BodyTex { get => BaseDef.BodyTex;
            set => BaseDef.BodyTex = value;
        }
        public string HeadMesh { get => BaseDef.HeadMesh;
            set => BaseDef.HeadMesh = value;
        }
        public int HeadTex { get => BaseDef.HeadTex;
            set => BaseDef.HeadTex = value;
        }
        public byte Guild { get => BaseDef.Guild;
            set => BaseDef.Guild = value;
        }

        partial void pConstruct();
        public NPCDef()
        {
            pConstruct();
        }

        protected override BaseVobInstance CreateVobInstance()
        {
            return new NPCInstance(this);
        }
    }
}