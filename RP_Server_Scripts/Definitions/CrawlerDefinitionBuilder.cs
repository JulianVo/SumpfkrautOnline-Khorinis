using System;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Definitions
{
    class CrawlerDefBuilder:IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public CrawlerDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            // HUMAN MODEL
            ModelDef m = new ModelDef("crawler", "crawler.mds");
            m.SetAniCatalog(new NPCCatalog());

            var aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 20));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 9);
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_run", "t_FistAttackMove", new ScriptAni(0, 29));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 16);
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_parade", "t_FistParade_0", new ScriptAni(0, 29));
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_jumpback", "t_FistParadeJumpB", new ScriptAni(0, 29));
            m.AddAniJob(aniJob);

            m.Radius = 130;
            m.HalfHeight = 80;
            m.FistRange = 40;
            m.CenterOffset = 50;
            m.Create();

            // NPCs
            NpcDef npcDef = new NpcDef("minecrawler",_BaseDefFactory,_Registration);
            npcDef.Name = "Minecrawler";
            npcDef.Model = m;
            npcDef.BodyMesh = "Crw_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 29;
            npcDef.Create();

            npcDef = new NpcDef("minecrawler_warrior", _BaseDefFactory, _Registration);
            npcDef.Name = "Minecrawler-Krieger";
            npcDef.Model = m;
            npcDef.BodyMesh = "Cr2_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 29;
            npcDef.Create();

            // CRAWLER KÖNIGIN
            m = new ModelDef("crawler_queen", "CRWQUEEN.mds");
            m.SetAniCatalog(new NPCCatalog());

            aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 24));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 13);
            m.AddAniJob(aniJob);

            m.Radius = 250;
            m.HalfHeight = 300;
            m.FistRange = 100;
            m.Create();

            // NPCs
            npcDef = new NpcDef("crawler_queen", _BaseDefFactory, _Registration);
            npcDef.Name = "Minecrawler-Königin";
            npcDef.Model = m;
            npcDef.BodyMesh = "CrQ_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 29;
            npcDef.Create();
        }
    }
}
