using System;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Shared_Script;

namespace RP_Server_Scripts.Definitions
{
    class HumanoidDefBuilder:IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public HumanoidDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            // HUMAN MODEL
            ModelDef m = new ModelDef("humans", "HUMANS.MDS");
            m.SetAniCatalog(new NPCCatalog());
            AddFistAnis(m);
            Add1HAnis(m);
            Add2hAnis(m);
            AddJumpAnis(m);
            AddClimbAnis(m);
            AddBowAnis(m);
            AddXBowAnis(m);
            AddUnconsciousAnis(m);
            AddItemAnis(m);
            AddGestureAnis(m);

            m.AddOverlay(new ScriptOverlay("Humans_Torch", "Humans_Torch.mds"));
            m.AddOverlay(new ScriptOverlay("Humans_Skeleton", "Humans_Skeleton.mds"));

            m.Radius = 40;
            m.HalfHeight = 90;
            m.FistRange = 40;
            m.Create();


            // NPCs
            NpcDef npcDef = new NpcDef("maleplayer",_BaseDefFactory,_Registration);
            npcDef.Name = "Spieler";
            npcDef.Model = m;
            npcDef.BodyMesh = HumBodyMeshs.HUM_BODY_NAKED0.ToString();
            npcDef.BodyTex = (int)HumBodyTexs.G1Hero;
            npcDef.HeadMesh = HumHeadMeshs.HUM_HEAD_PONY.ToString();
            npcDef.HeadTex = (int)HumHeadTexs.Face_N_Player;
            npcDef.Guild = 1;
            npcDef.Create();

            npcDef = new NpcDef("femaleplayer", _BaseDefFactory, _Registration);
            npcDef.Name = "Spielerin";
            npcDef.Model = m;
            npcDef.BodyMesh = HumBodyMeshs.HUM_BODY_BABE0.ToString();
            npcDef.BodyTex = (int)HumBodyTexs.F_Babe1;
            npcDef.HeadMesh = HumHeadMeshs.HUM_HEAD_BABE.ToString();
            npcDef.HeadTex = (int)HumHeadTexs.FaceBabe_N_Anne;
            npcDef.Guild = 1;
            npcDef.Create();

            npcDef = new NpcDef("skeleton", _BaseDefFactory, _Registration);
            npcDef.Name = "Skelett";
            npcDef.Model = m;
            npcDef.BodyMesh = "Ske_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 31;
            npcDef.Create();

            npcDef = new NpcDef("skeleton2", _BaseDefFactory, _Registration);
            npcDef.Name = "Skelett";
            npcDef.Model = m;
            npcDef.BodyMesh = "Ske_Body2";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Guild = 31;
            npcDef.Create();

            npcDef = new NpcDef("skeleton_lord", _BaseDefFactory, _Registration);
            npcDef.Name = "Schattenlord";
            npcDef.Model = m;
            npcDef.BodyMesh = HumBodyMeshs.HUM_BODY_NAKED0.ToString();
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "Ske_Head";
            npcDef.HeadTex = 0;
            npcDef.Guild = 31;
            npcDef.Create();
        }

        void AddGestureAnis(ModelDef m)
        {
            m.AddAniJob(new ScriptAniJob("gesture_dontknow", "t_dontknow", new ScriptAni(0, 10)));

            m.AddAniJob(new ScriptAniJob("plunder", "t_plunder", new ScriptAni(0, 75)));
        }

        void AddItemAnis(ModelDef m)
        {
            // take item
            ScriptAniJob aniJob1 = new ScriptAniJob("take_item", "t_Stand_2_IGet", new ScriptAni(0, 9) { { SpecialFrame.ItemHandle, 9 } });
            m.AddAniJob(aniJob1);

            ScriptAniJob aniJob2 = new ScriptAniJob("take_item2", "s_IGet", new ScriptAni(0, 1));
            m.AddAniJob(aniJob2);
            aniJob1.NextAni = aniJob2;

            ScriptAniJob aniJob3 = new ScriptAniJob("take_item3", "t_IGet_2_Stand", new ScriptAni(0, 9));
            m.AddAniJob(aniJob3);
            aniJob2.NextAni = aniJob3;


            // drop item
            aniJob1 = new ScriptAniJob("drop_item", "t_Stand_2_IDrop", new ScriptAni(0, 6) { { SpecialFrame.ItemHandle, 6 } });
            m.AddAniJob(aniJob1);

            aniJob2 = new ScriptAniJob("drop_item2", "s_IDrop", new ScriptAni(0, 1));
            m.AddAniJob(aniJob2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("drop_item3", "t_IDrop_2_Stand", new ScriptAni(0, 6));
            m.AddAniJob(aniJob3);
            aniJob2.NextAni = aniJob3;


            // drink potion
            aniJob1 = new ScriptAniJob("chug_potion", "t_potionfast_Stand_2_S0", new ScriptAni(0, 5) { { SpecialFrame.ItemHandle, 30 } });
            m.AddAniJob(aniJob1);

            aniJob2 = new ScriptAniJob("chug_potion2", "s_potionfast_S0", new ScriptAni(0, 1));
            m.AddAniJob(aniJob2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("chug_potion3", "t_potionfast_S0_2_Stand", new ScriptAni(0, 32));
            m.AddAniJob(aniJob3);
            aniJob2.NextAni = aniJob3;
        }

        void AddUnconsciousAnis(ModelDef model)
        {
            var ani1 = new ScriptAniJob("uncon_dropfront", "t_Stand_2_Wounded", new ScriptAni(0, 20));
            ani1.DefaultAni.FPS = 10;

            var ani2 = new ScriptAniJob("uncon_front", "s_wounded", new ScriptAni());
            model.AddAniJob(ani1);
            model.AddAniJob(ani2);
            ani1.NextAni = ani2;

            ani1 = new ScriptAniJob("uncon_dropback", "t_Stand_2_Woundedb", new ScriptAni(0, 14));
            ani1.DefaultAni.FPS = 10;

            ani2 = new ScriptAniJob("uncon_back", "s_woundedb", new ScriptAni());

            model.AddAniJob(ani1);
            model.AddAniJob(ani2);

            ani1.NextAni = ani2;

            // STAND UP
            var ani = new ScriptAniJob("uncon_standupfront", "t_Wounded_2_Stand", new ScriptAni(0, 34));
            ani.DefaultAni.FPS = 10;
            model.AddAniJob(ani);

            ani = new ScriptAniJob("uncon_standupback", "t_Woundedb_2_Stand", new ScriptAni(0, 40));
            ani.DefaultAni.FPS = 10;
            model.AddAniJob(ani);
        }

        void AddJumpAnis(ModelDef model)
        {
            model.AddAniJob(new ScriptAniJob("jump_fwd", "t_Stand_2_Jump"));
            model.AddAniJob(new ScriptAniJob("jump_run", "t_RunL_2_Jump"));
            model.AddAniJob(new ScriptAniJob("jump_up", "t_Stand_2_JumpUp"));
        }

        void AddClimbAnis(ModelDef model)
        {
            var ani1 = new ScriptAniJob("climb_low", "t_Stand_2_JumpUpLow", new ScriptAni(0, 4));
            var ani2 = new ScriptAniJob("climb_low1", "s_JumpUpLow", new ScriptAni(0, 1));
            var ani3 = new ScriptAniJob("climb_low2", "t_JumpUpLow_2_Stand", new ScriptAni(0, 9));

            model.AddAniJob(ani1);
            model.AddAniJob(ani2);
            model.AddAniJob(ani3);

            ani1.NextAni = ani2;
            ani2.NextAni = ani3;

            ani1 = new ScriptAniJob("climb_mid", "t_Stand_2_JumpUpMid", new ScriptAni(0, 9));
            ani2 = new ScriptAniJob("climb_mid1", "s_JumpUpMid", new ScriptAni(0, 1));
            ani3 = new ScriptAniJob("climb_mid2", "t_JumpUpMid_2_Stand", new ScriptAni(0, 20));

            model.AddAniJob(ani1);
            model.AddAniJob(ani2);
            model.AddAniJob(ani3);

            ani1.NextAni = ani2;
            ani2.NextAni = ani3;

            ani1 = new ScriptAniJob("climb_high", "t_Jump_2_Hang", new ScriptAni(0, 17));
            ani2 = new ScriptAniJob("climb_high1", "s_hang", new ScriptAni(0, 1));
            ani3 = new ScriptAniJob("climb_high2", "t_Hang_2_Stand", new ScriptAni(0, 25));

            model.AddAniJob(ani1);
            model.AddAniJob(ani2);
            model.AddAniJob(ani3);

            ani1.NextAni = ani2;
            ani2.NextAni = ani3;
        }

        void AddFistAnis(ModelDef model)
        {
            // Draw Fists
            ScriptAniJob aniJob1 = new ScriptAniJob("drawfists_part0", "t_Run_2_Fist", new ScriptAni(0, 3));
            aniJob1.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 2);
            ScriptAniJob aniJob2 = new ScriptAniJob("drawfists_part1", "s_Fist", new ScriptAni(0, 1));
            ScriptAniJob aniJob3 = new ScriptAniJob("drawfists_part2", "t_Fist_2_FistRun", new ScriptAni(0, 3));

            model.AddAniJob(aniJob1);
            model.AddAniJob(aniJob2);
            model.AddAniJob(aniJob3);

            aniJob1.NextAni = aniJob2;
            aniJob2.NextAni = aniJob3;

            // Draw Fists running
            ScriptAniJob aniJob = new ScriptAniJob("drawfists_running", "t_Move_2_FistMove", new ScriptAni(0, 14));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 5);
            aniJob.Layer = 2;

            model.AddAniJob(aniJob);

            // Undraw fists
            aniJob1 = new ScriptAniJob("undrawfists_part0", "t_FistRun_2_Fist", new ScriptAni(0, 3));
            aniJob1.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 2);
            aniJob2 = new ScriptAniJob("undrawfists_part1", "s_Fist", new ScriptAni(0, 1));
            aniJob3 = new ScriptAniJob("undrawfists_part2", "t_Fist_2_Run", new ScriptAni(0, 3));

            model.AddAniJob(aniJob1);
            model.AddAniJob(aniJob2);
            model.AddAniJob(aniJob3);

            aniJob1.NextAni = aniJob2;
            aniJob2.NextAni = aniJob3;

            // Undraw Fists running
            aniJob = new ScriptAniJob("undrawfists_running", "t_FistMove_2_Move", new ScriptAni(0, 14));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 5);
            aniJob.Layer = 2;

            model.AddAniJob(aniJob);

            // Fight Animations
            aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 15));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Combo, 9);
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_fwd1", "s_FistAttack", new ScriptAni(15, 29));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 9);
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_run", "t_FistAttackMove", new ScriptAni(0, 29));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 19);
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_parade", "t_FistParade_0", new ScriptAni(0, 12));
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_jumpback", "t_FistParadeJumpB", new ScriptAni(0, 12));
            model.AddAniJob(aniJob);
        }

        void Add1HAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("1HST1", "Humans_1hST1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("1HST2", "Humans_1hST2"); model.AddOverlay(ov2);

            // Draw 1h
            ScriptAniJob aniJob1 = new ScriptAniJob("draw1h_part0", "t_Run_2_1h");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 2) { { SpecialFrame.Draw, 2 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 3) { { SpecialFrame.Draw, 3 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 1) { { SpecialFrame.Draw, 1 } }, ov2);

            ScriptAniJob aniJob2 = new ScriptAniJob("draw1h_part1", "s_1h");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            ScriptAniJob aniJob3 = new ScriptAniJob("draw1h_part2", "t_1h_2_1hRun");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 12));
            aniJob3.AddOverlayAni(new ScriptAni(0, 6), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 8), ov2);
            aniJob2.NextAni = aniJob3;

            // Draw 1h running
            ScriptAniJob aniJob = new ScriptAniJob("draw1h_running", "t_Move_2_1hMove", new ScriptAni(0, 24));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 6);
            model.AddAniJob(aniJob);

            // Undraw 1h
            aniJob1 = new ScriptAniJob("undraw1h_part0", "t_1hRun_2_1h");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 12) { { SpecialFrame.Draw, 12 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 6) { { SpecialFrame.Draw, 6 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 8) { { SpecialFrame.Draw, 8 } }, ov2);

            aniJob2 = new ScriptAniJob("undraw1h_part1", "s_1h");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undraw1h_part2", "t_1h_2_Run");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 2));
            aniJob3.AddOverlayAni(new ScriptAni(0, 3), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob2.NextAni = aniJob3;

            // Undraw 1h running
            aniJob = new ScriptAniJob("undraw1h_running", "t_1hMove_2_Move", new ScriptAni(0, 24));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 18);
            model.AddAniJob(aniJob);

            // Fwd attack 1
            ScriptAniJob job = new ScriptAniJob("1hattack_fwd0", "s_1hattack");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 23) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 15 } });
            job.AddOverlayAni(new ScriptAni(0, 33) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 14 } }, ov1);
            job.AddOverlayAni(new ScriptAni(0, 29) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 10 } }, ov2);

            // fwd combo 2
            job = new ScriptAniJob("1hattack_fwd1", "s_1hattack");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(26, 40) { { SpecialFrame.Hit, 7 } });
            job.AddOverlayAni(new ScriptAni(33, 68) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 15 } }, ov1);
            job.AddOverlayAni(new ScriptAni(33, 60) { { SpecialFrame.Hit, 3 }, { SpecialFrame.Combo, 9 } }, ov2);

            // fwd combo 3
            job = new ScriptAniJob("1hattack_fwd2", "s_1hattack");
            model.AddAniJob(job);
            job.AddOverlayAni(new ScriptAni(68, 103) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 17 } }, ov1);
            job.AddOverlayAni(new ScriptAni(65, 92) { { SpecialFrame.Hit, 8 }, { SpecialFrame.Combo, 13 } }, ov2);

            // fwd combo 4
            job = new ScriptAniJob("1hattack_fwd3", "s_1hattack");
            model.AddAniJob(job);
            job.AddOverlayAni(new ScriptAni(103, 120) { { SpecialFrame.Hit, 7 } }, ov1);
            job.AddOverlayAni(new ScriptAni(97, 113) { { SpecialFrame.Hit, 10 } }, ov2);

            // left attack
            job = new ScriptAniJob("1hattack_left", "t_1HAttackL");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 30) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 15 } });
            job.AddOverlayAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 10 } }, ov1);
            job.AddOverlayAni(new ScriptAni(0, 20) { { SpecialFrame.Hit, 3 }, { SpecialFrame.Combo, 8 } }, ov2);

            // right attack
            job = new ScriptAniJob("1hattack_right", "t_1HAttackR");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 30) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 15 } });
            job.AddOverlayAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 10 } }, ov1);
            job.AddOverlayAni(new ScriptAni(0, 20) { { SpecialFrame.Hit, 3 }, { SpecialFrame.Combo, 8 } }, ov2);

            // run attack
            job = new ScriptAniJob("1hattack_run", "t_1HAttackMove");
            job.Layer = 2;
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 29) { { SpecialFrame.Hit, 16 } });

            // parades
            job = new ScriptAniJob("1h_parade0", "t_1HParade_0");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 15));

            job = new ScriptAniJob("1h_parade1", "t_1HParade_0_A2");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 15));

            job = new ScriptAniJob("1h_parade2", "t_1HParade_0_A3");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 15));

            // dodge
            job = new ScriptAniJob("1h_dodge", "t_1HParadeJumpB");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 14));
        }

        void Add2hAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("2HST1", "Humans_2hST1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("2HST2", "Humans_2hST2"); model.AddOverlay(ov2);

            // Draw 2h
            ScriptAniJob aniJob1 = new ScriptAniJob("draw2h_part0", "t_Run_2_2h");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 4) { { SpecialFrame.Draw, 4 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 4) { { SpecialFrame.Draw, 4 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 3) { { SpecialFrame.Draw, 3 } }, ov2);

            ScriptAniJob aniJob2 = new ScriptAniJob("draw2h_part1", "s_2h");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            ScriptAniJob aniJob3 = new ScriptAniJob("draw2h_part2", "t_2h_2_2hRun");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 15));
            aniJob3.AddOverlayAni(new ScriptAni(0, 15), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 10), ov2);
            aniJob2.NextAni = aniJob3;

            // Draw 2h running
            ScriptAniJob aniJob = new ScriptAniJob("draw2h_running", "t_Move_2_2hMove", new ScriptAni(0, 24));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 6);
            model.AddAniJob(aniJob);

            // Undraw 2h
            aniJob1 = new ScriptAniJob("undraw2h_part0", "t_2hRun_2_2h");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 15) { { SpecialFrame.Draw, 15 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 15) { { SpecialFrame.Draw, 15 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 10) { { SpecialFrame.Draw, 10 } }, ov2);

            aniJob2 = new ScriptAniJob("undraw2h_part1", "s_2h");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undraw2h_part2", "t_2h_2_Run");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 4));
            aniJob3.AddOverlayAni(new ScriptAni(0, 4), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 3), ov2);
            aniJob2.NextAni = aniJob3;

            // Undraw 2h running
            aniJob = new ScriptAniJob("undraw2h_running", "t_2hMove_2_Move", new ScriptAni(0, 24));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 18);
            model.AddAniJob(aniJob);

            // Fwd attack 1
            ScriptAniJob job = new ScriptAniJob("2hattack_fwd0", "s_2hattack");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 26) { { SpecialFrame.Hit, 8 }, { SpecialFrame.Combo, 14 } });
            job.AddOverlayAni(new ScriptAni(0, 35) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 16 } }, ov1);
            job.AddOverlayAni(new ScriptAni(0, 34) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 13 } }, ov2);

            // fwd combo 2
            job = new ScriptAniJob("2hattack_fwd1", "s_2hattack");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(31, 50) { { SpecialFrame.Hit, 6 } });
            job.AddOverlayAni(new ScriptAni(40, 75) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 16 } }, ov1);
            job.AddOverlayAni(new ScriptAni(39, 75) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 12 } }, ov2);

            // fwd combo 3
            job = new ScriptAniJob("2hattack_fwd2", "s_2hattack");
            model.AddAniJob(job);
            job.AddOverlayAni(new ScriptAni(80, 114) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 16 } }, ov1);
            job.AddOverlayAni(new ScriptAni(79, 118) { { SpecialFrame.Hit, 9 }, { SpecialFrame.Combo, 17 } }, ov2);

            // fwd combo 4
            job = new ScriptAniJob("2hattack_fwd3", "s_2hattack");
            model.AddAniJob(job);
            job.AddOverlayAni(new ScriptAni(124, 146) { { SpecialFrame.Hit, 12 } }, ov2);

            // left attack
            job = new ScriptAniJob("2hattack_left", "t_2hAttackL");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 36) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 18 } });
            job.AddOverlayAni(new ScriptAni(0, 28) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 14 } }, ov1);
            job.AddOverlayAni(new ScriptAni(0, 26) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 14 } }, ov2);

            // right attack
            job = new ScriptAniJob("2hattack_right", "t_2hAttackR");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 36) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 18 } });
            job.AddOverlayAni(new ScriptAni(0, 29) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 14 } }, ov1);
            job.AddOverlayAni(new ScriptAni(0, 26) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 14 } }, ov2);

            // run attack
            job = new ScriptAniJob("2hattack_run", "t_2hAttackMove");
            job.Layer = 2;
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 23) { { SpecialFrame.Hit, 12 } });

            // parades
            job = new ScriptAniJob("2h_parade0", "t_2hParade_0");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 15));

            job = new ScriptAniJob("2h_parade1", "t_2hParade_0_A2");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 15));

            job = new ScriptAniJob("2h_parade2", "t_2hParade_0_A3");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 15));

            // dodge
            job = new ScriptAniJob("2h_dodge", "t_2hParadeJumpB");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 24));
        }

        void AddBowAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("BowT1", "Humans_BowT1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("BowT2", "Humans_BowT2"); model.AddOverlay(ov2);

            ScriptAniJob aniJob1 = new ScriptAniJob("drawbow_part0", "t_Run_2_bow");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 5) { { SpecialFrame.Draw, 5 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 5) { { SpecialFrame.Draw, 5 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 5) { { SpecialFrame.Draw, 5 } }, ov2);

            ScriptAniJob aniJob2 = new ScriptAniJob("drawbow_part1", "s_bow");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            ScriptAniJob aniJob3 = new ScriptAniJob("drawbow_part2", "t_bow_2_bowRun");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 12));
            aniJob3.AddOverlayAni(new ScriptAni(0, 10), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 8), ov2);
            aniJob2.NextAni = aniJob3;

            ScriptAniJob aniJob = new ScriptAniJob("drawbow_running", "t_Move_2_BowMove", new ScriptAni(0, 19));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 7);
            model.AddAniJob(aniJob);

            aniJob1 = new ScriptAniJob("undrawbow_part0", "t_BowRun_2_Bow");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 12) { { SpecialFrame.Draw, 12 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 10) { { SpecialFrame.Draw, 10 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 8) { { SpecialFrame.Draw, 8 } }, ov2);

            aniJob2 = new ScriptAniJob("undrawbow_part1", "s_Bow");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undrawbow_part2", "t_bow_2_Run");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 5));
            aniJob3.AddOverlayAni(new ScriptAni(0, 5), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 5), ov2);
            aniJob2.NextAni = aniJob3;

            aniJob = new ScriptAniJob("undrawbow_running", "t_BowMove_2_Move", new ScriptAni(0, 19));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 12);
            model.AddAniJob(aniJob);

            aniJob1 = new ScriptAniJob("aim_bow", "t_bowwalk_2_bowaim");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 11));
            aniJob1.AddOverlayAni(new ScriptAni(0, 11), ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 11), ov2);

            aniJob2 = new ScriptAniJob("aiming_bow", "s_BowAim");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni());
            aniJob2.AddOverlayAni(new ScriptAni(), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(), ov2);
            aniJob1.NextAni = aniJob2;

            // fixme: add s_bowshoot too?
            aniJob1 = new ScriptAniJob("reload_bow", "t_BowReload");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 28));
            aniJob1.AddOverlayAni(new ScriptAni(0, 26), ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 22), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob1 = new ScriptAniJob("unaim_bow", "t_BowAim_2_Bowwalk");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 11));
            aniJob1.AddOverlayAni(new ScriptAni(0, 11), ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 11), ov2);
        }

        void AddXBowAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("XBowT1", "Humans_CBowT1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("XBowT2", "Humans_CBowT2"); model.AddOverlay(ov2);

            ScriptAniJob aniJob1 = new ScriptAniJob("drawXbow_part0", "t_Run_2_Cbow");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 4) { { SpecialFrame.Draw, 4 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 4) { { SpecialFrame.Draw, 4 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 4) { { SpecialFrame.Draw, 4 } }, ov2);

            ScriptAniJob aniJob2 = new ScriptAniJob("drawXbow_part1", "s_Cbow");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            ScriptAniJob aniJob3 = new ScriptAniJob("drawXbow_part2", "t_Cbow_2_CbowRun");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 33));
            aniJob3.AddOverlayAni(new ScriptAni(0, 29), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 25), ov2);
            aniJob2.NextAni = aniJob3;

            ScriptAniJob aniJob = new ScriptAniJob("drawXbow_running", "t_Move_2_CBowMove", new ScriptAni(0, 39));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 7);
            model.AddAniJob(aniJob);

            aniJob1 = new ScriptAniJob("undrawXbow_part0", "t_CBowRun_2_CBow");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 33) { { SpecialFrame.Draw, 33 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 29) { { SpecialFrame.Draw, 29 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 25) { { SpecialFrame.Draw, 25 } }, ov2);

            aniJob2 = new ScriptAniJob("undrawXbow_part1", "s_CBow");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undrawXbow_part2", "t_Cbow_2_Run");
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 4));
            aniJob3.AddOverlayAni(new ScriptAni(0, 4), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 4), ov2);
            aniJob2.NextAni = aniJob3;

            aniJob = new ScriptAniJob("undrawXbow_running", "t_CBowMove_2_Move", new ScriptAni(0, 39));
            aniJob.Layer = 2;
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 32);
            model.AddAniJob(aniJob);

            aniJob1 = new ScriptAniJob("aim_xbow", "t_cbowwalk_2_cbowaim");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 5));
            aniJob1.AddOverlayAni(new ScriptAni(0, 6), ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 6), ov2);

            aniJob2 = new ScriptAniJob("aiming_xbow", "s_CBowAim");
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni());
            aniJob2.AddOverlayAni(new ScriptAni(), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(), ov2);
            aniJob1.NextAni = aniJob2;

            // fixme: add s_bowshoot too?
            aniJob1 = new ScriptAniJob("reload_xbow", "t_CBowReload");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 32));
            aniJob1.AddOverlayAni(new ScriptAni(0, 29), ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 23), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob1 = new ScriptAniJob("unaim_xbow", "t_CBowAim_2_CBowwalk");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 6));
            aniJob1.AddOverlayAni(new ScriptAni(0, 6), ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 6), ov2);
        }
    }
}
