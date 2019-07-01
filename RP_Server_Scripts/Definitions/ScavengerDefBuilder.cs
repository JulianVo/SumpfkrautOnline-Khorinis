using System;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Definitions
{
    class ScavengerDefBuilder:IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public ScavengerDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            ModelDef m = new ModelDef("Scavenger", "Scavenger.mds");
            m.SetAniCatalog(new NPCCatalog());

            var aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 20));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 9);
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_run", "t_FistAttackMove", new ScriptAni(1, 15));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 16);
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_parade", "t_FistParade_0", new ScriptAni(0, 29));
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_jumpback", "t_FistParadeJumpB", new ScriptAni(1, 20));
            m.AddAniJob(aniJob);

            m.Radius = 60;
            m.HalfHeight = 30;
            m.CenterOffset = 50;
            m.FistRange = 100;
            m.Create();

            // NPCs
            NpcDef npcDef = new NpcDef("scavenger",_BaseDefFactory,_Registration);
            npcDef.Name = "Scavenger";
            npcDef.Model = m;
            npcDef.BodyMesh = "Sca_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 22;
            npcDef.Create();
        }
    }
}
