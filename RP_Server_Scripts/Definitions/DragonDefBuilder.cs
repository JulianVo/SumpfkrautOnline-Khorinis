using System;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Definitions
{
    class DragonDefBuilder:IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public DragonDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            // Dragon
            ModelDef m = new ModelDef("Dragon", "Dragon.mds");
            m.SetAniCatalog(new NPCCatalog());

            var aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 40));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 24);
            m.AddAniJob(aniJob);

            // strafe anis for block
            aniJob = new ScriptAniJob("fist_parade", "t_FistRunStrafeL", new ScriptAni(0, 50));
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_jumpback", "t_FistRunStrafeR", new ScriptAni(0, 50));
            m.AddAniJob(aniJob);

            m.Radius = 120;
            m.HalfHeight = 200;
            m.FistRange = 300;
            m.CenterOffset = 20;
            m.Create();

            // NPCs
            NpcDef npcDef = new NpcDef("dragon_fire",_BaseDefFactory,_Registration);
            npcDef.Name = "Feuerdrache";
            npcDef.Model = m;
            npcDef.BodyMesh = "Dragon_FIRE_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 47;
            npcDef.Create();

            npcDef = new NpcDef("dragon_undead", _BaseDefFactory, _Registration);
            npcDef.Name = "Untoter Drache";
            npcDef.Model = m;
            npcDef.BodyMesh = "Dragon_Undead_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 47;
            npcDef.Create();
        }
    }
}
