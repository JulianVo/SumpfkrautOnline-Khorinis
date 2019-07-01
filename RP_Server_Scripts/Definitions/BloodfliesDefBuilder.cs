using System;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Definitions
{
    class BloodfliesDefBuilder : IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public BloodfliesDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            // Bloodfly
            ModelDef m = new ModelDef("Bloodfly", "Bloodfly.mds");
            m.SetAniCatalog(new NPCCatalog());

            var aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 59));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 9);
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_run", "t_FistAttackMove", new ScriptAni(0, 29));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 16);
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_parade", "t_FistParade_0", new ScriptAni(0, 29));
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_jumpback", "t_FistParadeJumpB", new ScriptAni(1, 19));
            m.AddAniJob(aniJob);

            m.Radius = 40;
            m.HalfHeight = 40;
            m.FistRange = 60;
            m.Create();

            // NPCs
            NpcDef npcDef = new NpcDef("bloodfly", _BaseDefFactory, _Registration);
            npcDef.Name = "Blutfliege";
            npcDef.Model = m;
            npcDef.BodyMesh = "Blo_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 25;
            npcDef.Create();
        }
    }
}
