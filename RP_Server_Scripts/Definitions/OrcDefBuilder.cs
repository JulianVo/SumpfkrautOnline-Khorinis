using System;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Definitions
{
    class OrcDefBuilder:IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public OrcDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            ModelDef m = new ModelDef("orc", "orc.mds");
            m.SetAniCatalog(new NPCCatalog());

            #region Draw

            // Draw 2h
            ScriptAniJob aniJob1 = new ScriptAniJob("draw2h_part0", "t_Run_2_2h");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 5) { { SpecialFrame.Draw, 5 } });

            ScriptAniJob aniJob2 = new ScriptAniJob("draw2h_part1", "s_2h");
            m.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob1.NextAni = aniJob2;

            ScriptAniJob aniJob3 = new ScriptAniJob("draw2h_part2", "t_2h_2_2hRun");
            m.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 12));
            aniJob2.NextAni = aniJob3;

            // Draw 2h running
            ScriptAniJob aniJob = new ScriptAniJob("draw2h_running", "t_Move_2_2hMove", new ScriptAni(0, 20));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 7);
            m.AddAniJob(aniJob);

            // Undraw 2h
            aniJob1 = new ScriptAniJob("undraw2h_part0", "t_2hRun_2_2h");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 12) { { SpecialFrame.Draw, 12 } });

            aniJob2 = new ScriptAniJob("undraw2h_part1", "s_2h");
            m.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undraw2h_part2", "t_2h_2_Run");
            m.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 5));
            aniJob2.NextAni = aniJob3;

            // Undraw 2h running
            aniJob = new ScriptAniJob("undraw2h_running", "t_2hMove_2_Move", new ScriptAni(0, 20));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 13);
            m.AddAniJob(aniJob);

            #endregion

            #region Fighting

            // Fwd attack 1
            ScriptAniJob job = new ScriptAniJob("2hattack_fwd0", "s_2hattack");
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 11 } });

            // fwd combo 2
            job = new ScriptAniJob("2hattack_fwd1", "s_2hattack");
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(25, 70) { { SpecialFrame.Hit, 20 } });

            // left attack
            job = new ScriptAniJob("2hattack_left", "t_2hAttackL");
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 3 }, { SpecialFrame.Combo, 8 } });

            // right attack
            job = new ScriptAniJob("2hattack_right", "t_2hAttackR");
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 3 }, { SpecialFrame.Combo, 8 } });

            // run attack
            job = new ScriptAniJob("2hattack_run", "t_2hAttackMove");
            job.Layer = 2;
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 19) { { SpecialFrame.Hit, 13 } });

            // parades
            job = new ScriptAniJob("2h_parade0", "t_2hParade_0");
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 30));

            // dodge
            job = new ScriptAniJob("2h_dodge", "t_2hParadeJumpB");
            m.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 14));

            #endregion

            #region XBow anis

            #region Draw

            aniJob1 = new ScriptAniJob("drawXbow_part0", "t_Run_2_Cbow");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 4) { { SpecialFrame.Draw, 4 } });

            aniJob2 = new ScriptAniJob("drawXbow_part1", "s_Cbow");
            m.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("drawXbow_part2", "t_Cbow_2_CbowRun");
            m.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 34));
            aniJob2.NextAni = aniJob3;

            aniJob = new ScriptAniJob("drawXbow_running", "t_Move_2_CBowMove", new ScriptAni(0, 20));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 7);
            m.AddAniJob(aniJob);

            aniJob1 = new ScriptAniJob("undrawXbow_part0", "t_CBowRun_2_CBow");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 34) { { SpecialFrame.Draw, 34 } });

            aniJob2 = new ScriptAniJob("undrawXbow_part1", "s_CBow");
            m.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undrawXbow_part2", "t_Cbow_2_Run");
            m.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 4));
            aniJob2.NextAni = aniJob3;

            aniJob = new ScriptAniJob("undrawXbow_running", "t_CBowMove_2_Move", new ScriptAni(0, 20));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 14);
            m.AddAniJob(aniJob);

            #endregion

            #region Fight

            aniJob1 = new ScriptAniJob("aim_xbow", "t_cbowwalk_2_cbowaim");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 11));

            aniJob2 = new ScriptAniJob("aiming_xbow", "s_CBowAim");
            m.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni());
            aniJob1.NextAni = aniJob2;

            // fixme: add s_bowshoot too?
            aniJob1 = new ScriptAniJob("reload_xbow", "t_CBowReload");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 20));
            aniJob1.NextAni = aniJob2;

            aniJob1 = new ScriptAniJob("unaim_xbow", "t_CBowAim_2_CBowwalk");
            m.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 11));

            #endregion

            #endregion

            #region Jump Anis

            m.AddAniJob(new ScriptAniJob("jump_fwd", "t_Stand_2_Jump"));
            m.AddAniJob(new ScriptAniJob("jump_run", "t_RunL_2_Jump"));


            #endregion

            #region Climb Anis

            var ani1 = new ScriptAniJob("climb_low", "t_Stand_2_JumpUpLow", new ScriptAni(0, 10));
            var ani2 = new ScriptAniJob("climb_low1", "s_JumpUpLow", new ScriptAni(0, 4));
            var ani3 = new ScriptAniJob("climb_low2", "t_JumpUpLow_2_Stand", new ScriptAni(0, 15));

            m.AddAniJob(ani1);
            m.AddAniJob(ani2);
            m.AddAniJob(ani3);

            ani1.NextAni = ani2;
            ani2.NextAni = ani3;

            ani1 = new ScriptAniJob("climb_mid", "t_Stand_2_JumpUpMid", new ScriptAni(0, 10));
            ani2 = new ScriptAniJob("climb_mid1", "s_JumpUpMid", new ScriptAni(0, 2));
            ani3 = new ScriptAniJob("climb_mid2", "t_JumpUpMid_2_Stand", new ScriptAni(0, 23));

            m.AddAniJob(ani1);
            m.AddAniJob(ani2);
            m.AddAniJob(ani3);

            ani1.NextAni = ani2;
            ani2.NextAni = ani3;

            ani1 = new ScriptAniJob("climb_high", "t_Stand_2_JumpUp", new ScriptAni(0, 9));
            ani2 = new ScriptAniJob("climb_high1", "t_JumpUp_2_Hang", new ScriptAni(0, 2));
            ani3 = new ScriptAniJob("climb_high2", "t_Hang_2_Stand", new ScriptAni(0, 40));

            m.AddAniJob(ani1);
            m.AddAniJob(ani2);
            m.AddAniJob(ani3);

            ani1.NextAni = ani2;
            ani2.NextAni = ani3;

            #endregion

            m.Radius = 80;
            m.HalfHeight = 100;
            m.CenterOffset = 20;
            m.Create();

            // NPCs
            NpcDef npcDef = new NpcDef("orc_scout",_BaseDefFactory,_Registration);
            npcDef.Name = "Ork-Späher";
            npcDef.Model = m;
            npcDef.BodyMesh = "Orc_BodyWarrior";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "Orc_HeadWarrior";
            npcDef.HeadTex = 0;
            npcDef.Guild = 59;
            npcDef.Create();

            // NPCs
            npcDef = new NpcDef("orc_warrior",_BaseDefFactory,_Registration);
            npcDef.Name = "Ork-Krieger";
            npcDef.Model = m;
            npcDef.BodyMesh = "Orc_BodyWarrior";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "Orc_HeadWarrior";
            npcDef.HeadTex = 0;
            npcDef.Guild = 59;
            npcDef.Create();

            npcDef = new NpcDef("orc_elite",_BaseDefFactory,_Registration);
            npcDef.Name = "Ork-Elite";
            npcDef.Model = m;
            npcDef.BodyMesh = "Orc_BodyElite";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "Orc_HeadWarrior";
            npcDef.HeadTex = 0;
            npcDef.Guild = 59;
            npcDef.Create();

            npcDef = new NpcDef("orc_oberst",_BaseDefFactory,_Registration);
            npcDef.Name = "Orkischer Oberst";
            npcDef.Model = m;
            npcDef.BodyMesh = "Orc_BodyElite";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "Orc_HeadWarrior";
            npcDef.HeadTex = 0;
            npcDef.Guild = 59;
            npcDef.Create();
        }
    }
}
