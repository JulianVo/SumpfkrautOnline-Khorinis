using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using GUC.Log;
using GUC.Scripting;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.Scripts.Sumpfkraut.Visuals;

namespace GUC.Scripts
{
    public partial class GUCScripts : ScriptInterface
    {
        public GUCScripts()
        {
            Logger.Log("######## Initalise SumpfkrautOnline ServerScripts #########");

            AddSomeDefs();

            CreateTestWorld();

            // -- Websocket-Server --
            Sumpfkraut.Web.WS.WSServer wsServer = new Sumpfkraut.Web.WS.WSServer();
            wsServer.SetObjName("WSServer");
            wsServer.Init();
            wsServer.Start();

            // -- command console --
            Sumpfkraut.CommandConsole.CommandConsole cmdConsole = new Sumpfkraut.CommandConsole.CommandConsole();

            Logger.Log("######################## Finished #########################");
        }

        void AddSomeDefs()
        {
            AddItems();

            // HUMAN MODEL
            var m = new ModelDef("human", "humans.mds");
            m.Radius = 80;
            m.Height = 180;
            
            Add2hAttacks(m);
            Add1hAttacks(m);
            AddBowAnis(m);
            AddXBowAnis(m);

            // JUMPS
            var aniJob = new ScriptAniJob("jumprun", new ScriptAni(8000000));
            aniJob.BaseAniJob.ID = (int)SetAnis.JumpRun;
            aniJob.AniName = "t_RunL_2_Jump";
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("jumpfwd", new ScriptAni(9200000));
            aniJob.BaseAniJob.ID = (int)SetAnis.JumpFwd;
            aniJob.AniName = "T_STAND_2_JUMP";
            m.AddAniJob(aniJob);

            // CLIMBING
            aniJob = new ScriptAniJob("climblow", new ScriptAni(1200000 + 2000000 + 3200000));
            aniJob.BaseAniJob.ID = (int)SetAnis.ClimbLow;
            aniJob.AniName = "T_STAND_2_JUMPUPLOW";
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("climbmid", new ScriptAni(3200000 + 1200000 + 8000000));
            aniJob.BaseAniJob.ID = (int)SetAnis.ClimbMid;
            aniJob.AniName = "T_STAND_2_JUMPUPMID";
            m.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("climbhigh", new ScriptAni(6800000 + 0 + 10000000));
            aniJob.BaseAniJob.ID = (int)SetAnis.ClimbHigh;
            aniJob.AniName = "T_JUMP_2_HANG";
            m.AddAniJob(aniJob);

            m.Create();

            // NPCs

            NPCDef npcDef = new NPCDef("player");
            npcDef.Name = "Spieler";
            npcDef.Model = m;
            npcDef.BodyMesh = Enumeration.HumBodyMeshs.HUM_BODY_NAKED0.ToString();
            npcDef.BodyTex = (int)Enumeration.HumBodyTexs.G1Hero;
            npcDef.HeadMesh = Enumeration.HumHeadMeshs.HUM_HEAD_PONY.ToString();
            npcDef.HeadTex = (int)Enumeration.HumHeadTexs.Face_N_Player;
            npcDef.Create();
        }

        void CreateTestWorld()
        {
            WorldDef wDef = new WorldDef();
            WorldInst.Current = new WorldInst(default(WorldDef));
            
            WorldInst.Current.Create();
            WorldInst.Current.Clock.SetTime(new Types.WorldTime(0, 8), 10.0f);
            WorldInst.Current.Clock.Start();
        }

        void Add1hAttacks(ModelDef model)
        {
            var ov1 = new ScriptOverlay("1HST1", "Humans_1hST1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("1HST2", "Humans_1hST2"); model.AddOverlay(ov2);

            // Weapon drawing

            ScriptAniJob aniJob = new ScriptAniJob("draw1h");
            aniJob.BaseAniJob.ID = (int)SetAnis.Draw1H;
            aniJob.AniName = "draw1h";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(6000000, 1600000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(4100000, 2000000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(4100000, 1200000), ov2);

            aniJob = new ScriptAniJob("draw1hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Draw1HRun;
            aniJob.AniName = "T_MOVE_2_1HMOVE";
            model.AddAniJob(aniJob);
            var ani = ScriptAni.NewDrawAni(11000000, 2300000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            aniJob = new ScriptAniJob("undraw1h");
            aniJob.BaseAniJob.ID = (int)SetAnis.Undraw1H;
            aniJob.AniName = "undraw1h";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(6000000, 4400000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(3600000, 2000000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(3600000, 2800000), ov2);

            aniJob = new ScriptAniJob("undraw1hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Undraw1HRun;
            aniJob.AniName = "T_1HMOVE_2_MOVE";
            model.AddAniJob(aniJob);
            ani = ScriptAni.NewDrawAni(11000000, 6900000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            // 1h COMBO 1
            aniJob = new ScriptAniJob("attack1hfwd1");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HFwd1;
            aniJob.AniName = "s_1hAttack";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(8400000, 2000000, 4400000));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(13200000, 1200000, 4400000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(11200000, 1200000, 3600000), ov2);
            
            // 1h COMBO 2
            aniJob = new ScriptAniJob("attack1hfwd2");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HFwd2;
            aniJob.AniName = "s_1hAttack";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(5200000, 3200000, 3200000, 26));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(12500000, 1400000, 5200000, 36), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(10800000, 1800000, 5200000, 33), ov2);

            // 1h COMBO 3
            aniJob = new ScriptAniJob("attack1hfwd3");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HFwd3;
            aniJob.AniName = "s_1hAttack";
            model.AddAniJob(aniJob);

            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(12500000, 1800000, 5200000, 71), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(10800000, 3000000, 6800000, 65), ov2);
            
            // 1h COMBO 4
            aniJob = new ScriptAniJob("attack1hfwd4");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HFwd4;
            aniJob.AniName = "s_1hAttack";
            model.AddAniJob(aniJob);

            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(6400000, 2200000, 2400000, 106), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(6400000, 4000000, 5600000, 97), ov2);

            // 1h LEFT ATTACK
            aniJob = new ScriptAniJob("attack1hleft");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HLeft;
            aniJob.AniName = "t_1hAttackL";
            aniJob.AttackBonus = -2;
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(11600000, 2000000, 6000000));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(9200000, 1600000, 4000000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(7200000, 1200000, 3200000), ov2);

            // 1h RIGHT ATTACK
            aniJob = new ScriptAniJob("attack1hright");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HRight;
            aniJob.AniName = "t_1hAttackR";
            aniJob.AttackBonus = -2;
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(11600000, 2000000, 6000000));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(9200000, 1600000, 4000000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(7200000, 1200000, 3200000), ov2);

            // 1h RUN ATTACK
            aniJob = new ScriptAniJob("attack1hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HRun;
            aniJob.AniName = "t_1hAttackMove";
            aniJob.AttackBonus = 5;
            model.AddAniJob(aniJob);

            ani = ScriptAni.NewAttackAni(11200000, 7000000); ani.Layer = 2; aniJob.SetDefaultAni(ani);
            
            // 1h Parry
            aniJob = new ScriptAniJob("attack1hparry1", ScriptAni.NewFightAni(5600000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HParry1;
            aniJob.AniName = "T_1HPARADE_0";
            model.AddAniJob(aniJob);
            
            // 1h Parry
            aniJob = new ScriptAniJob("attack1hparry2", ScriptAni.NewFightAni(5600000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HParry2;
            aniJob.AniName = "T_1HPARADE_0_A2";
            model.AddAniJob(aniJob);

            // 1h Parry
            aniJob = new ScriptAniJob("attack1hparry3", ScriptAni.NewFightAni(5600000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HParry3;
            aniJob.AniName = "T_1HPARADE_0_A3";
            model.AddAniJob(aniJob);

            // 1h Dodge
            aniJob = new ScriptAniJob("attack1hdodge", ScriptAni.NewFightAni(5200000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack1HDodge;
            aniJob.AniName = "T_1HPARADEJUMPB";
            model.AddAniJob(aniJob);
        }

        void Add2hAttacks(ModelDef model)
        {
            var ov1 = new ScriptOverlay("2HST1", "Humans_2hST1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("2HST2", "Humans_2hST2"); model.AddOverlay(ov2);

            // Weapon drawing

            ScriptAniJob aniJob = new ScriptAniJob("draw2h");
            aniJob.BaseAniJob.ID = (int)SetAnis.Draw2H;
            aniJob.AniName = "draw2h";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(8000000, 2400000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(8000000, 2400000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(6100000, 1330000), ov2);

            aniJob = new ScriptAniJob("draw2hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Draw2HRun;
            aniJob.AniName = "T_MOVE_2_2HMOVE";
            model.AddAniJob(aniJob);
            var ani = ScriptAni.NewDrawAni(11000000, 2680000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            aniJob = new ScriptAniJob("undraw2h");
            aniJob.BaseAniJob.ID = (int)SetAnis.Undraw2H;
            aniJob.AniName = "undraw2h";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(7500000, 5600000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(7500000, 5600000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(5600000, 2670000), ov2);

            aniJob = new ScriptAniJob("undraw2hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Undraw2HRun;
            aniJob.AniName = "T_2HMOVE_2_MOVE";
            model.AddAniJob(aniJob);
            ani = ScriptAni.NewDrawAni(11000000, 6520000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            // 2h COMBO 1
            aniJob = new ScriptAniJob("attack2hfwd1");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HFwd1;
            aniJob.AniName = "s_2hAttack";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(10000000, 2800000, 5800000));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(13000000, 2000000, 6000000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(13000000, 1600000, 4800000), ov2);

            // 2h COMBO 2
            aniJob = new ScriptAniJob("attack2hfwd2");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HFwd2;
            aniJob.AniName = "s_2hAttack";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(6000000, 2300000, 4400000, 31));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(14000000, 2500000, 8000000, 40), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(11500000, 1800000, 6800000, 41), ov2);

            // 2h COMBO 3
            aniJob = new ScriptAniJob("attack2hfwd3");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HFwd3;
            aniJob.AniName = "s_2hAttack";
            model.AddAniJob(aniJob);
            
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(7000000, 3000000, 7000000, 80), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(13500000, 4000000, 8800000, 81), ov2);

            // 2h COMBO 4
            aniJob = new ScriptAniJob("attack2hfwd4");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HFwd4;
            aniJob.AniName = "s_2hAttack";
            model.AddAniJob(aniJob);

            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(7500000, 4000000, 7500000, 126), ov2);

            // 2h LEFT ATTACK
            aniJob = new ScriptAniJob("attack2hleft");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HLeft;
            aniJob.AniName = "t_2hAttackL";
            aniJob.AttackBonus = -2;
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(14000000, 2300000, 7200000));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(10700000, 2000000, 5600000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(10200000, 2000000, 5600000), ov2);

            // 2h RIGHT ATTACK
            aniJob = new ScriptAniJob("attack2hright");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HRight;
            aniJob.AniName = "t_2hAttackR";
            aniJob.AttackBonus = -2;
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewAttackAni(14000000, 2300000, 7200000));
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(10700000, 2000000, 5600000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewAttackAni(10200000, 2000000, 5600000), ov2);

            // 2h RUN ATTACK
            aniJob = new ScriptAniJob("attack2hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HRun;
            aniJob.AniName = "t_2hAttackMove";
            aniJob.AttackBonus = 5;
            model.AddAniJob(aniJob);

            ani = ScriptAni.NewAttackAni(8800000, 6000000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            // 2h Parry
            aniJob = new ScriptAniJob("attack2hparry1", ScriptAni.NewFightAni(5600000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HParry1;
            aniJob.AniName = "T_2HPARADE_0";
            model.AddAniJob(aniJob);

            // 2h Parry
            aniJob = new ScriptAniJob("attack2hparry2", ScriptAni.NewFightAni(5600000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HParry2;
            aniJob.AniName = "T_2HPARADE_0_A2";
            model.AddAniJob(aniJob);

            // 2h Parry
            aniJob = new ScriptAniJob("attack2hparry3", ScriptAni.NewFightAni(5600000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HParry3;
            aniJob.AniName = "T_2HPARADE_0_A3";
            model.AddAniJob(aniJob);

            // 2h Dodge
            aniJob = new ScriptAniJob("attack2hdodge", ScriptAni.NewFightAni(9200000));
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HDodge;
            aniJob.AniName = "T_2HPARADEJUMPB";
            model.AddAniJob(aniJob);
        }

        void AddItems()
        {
            //ZWEIHANDER
            ModelDef m = new ModelDef("2hschwert", "ItMw_060_2h_sword_01.3DS");
            m.Create();
            ItemDef itemDef = new ItemDef("2hschwert");
            itemDef.Name = "Zweih�nder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = Enumeration.ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 110;
            itemDef.Damage = 42;
            itemDef.Create();

            // GARDER�STUNG
            m = new ModelDef("ITAR_Garde", "ItAr_Bloodwyn_ADDON.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Garde");
            itemDef.Name = "Gardistenr�stung";
            itemDef.Material = Enumeration.ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.Protection = 30;
            itemDef.VisualChange = "Armor_Bloodwyn_ADDON.asc";
            itemDef.Model = m;
            itemDef.Create();

            //EINHANDER
            m = new ModelDef("1hschwert", "Itmw_025_1h_Mil_Sword_broad_01.3DS");
            m.Create();
            itemDef = new ItemDef("1hschwert");
            itemDef.Name = "Breitschwert";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = Enumeration.ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 40;
            itemDef.Range = 90;
            itemDef.Create();

            // SCHATTENR�STUNG
            m = new ModelDef("ITAR_Schatten", "ItAr_Diego.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Schatten");
            itemDef.Name = "Schattenr�stung";
            itemDef.Material = Enumeration.ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Diego.asc";
            itemDef.Protection = 27;
            itemDef.Model = m;
            itemDef.Create();

            //ZWEIHAND AXT
            m = new ModelDef("2haxt", "ItMw_060_2h_axe_heavy_01.3DS");
            m.Create();
            itemDef = new ItemDef("2haxt");
            itemDef.Name = "S�ldneraxt";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = Enumeration.ItemMaterials.Wood;
            itemDef.Model = m;
            itemDef.Damage = 44;
            itemDef.Range = 95;
            itemDef.Create();

            // S�LDNERR�STUNG
            m = new ModelDef("ITAR_S�ldner", "ItAr_Sld_M.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_S�ldner");
            itemDef.Name = "S�ldnerr�stung";
            itemDef.Material = Enumeration.ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Sld_M.asc";
            itemDef.Protection = 30;
            itemDef.Model = m;
            itemDef.Create();

            //EINHAND AXT
            m = new ModelDef("1haxt", "ItMw_025_1h_sld_axe_01.3DS");
            m.Create();
            itemDef = new ItemDef("1haxt");
            itemDef.Name = "Grobes Kriegsbeil";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = Enumeration.ItemMaterials.Wood;
            itemDef.Damage = 42;
            itemDef.Model = m;
            itemDef.Range = 75;
            itemDef.Create();

            // BANDITENR�STUNG
            m = new ModelDef("ITAR_bandit", "ItAr_Bdt_H.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_bandit");
            itemDef.Name = "Banditenr�stung";
            itemDef.Material = Enumeration.ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Bdt_H.asc";
            itemDef.Protection = 27;
            itemDef.Model = m;
            itemDef.Create();

            // PFEIL
            m = new ModelDef("itrw_arrow", "ItRw_Arrow.3ds");
            m.Create();
            itemDef = new ItemDef("itrw_arrow");
            itemDef.Name = "Pfeil";
            itemDef.Material = Enumeration.ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.AmmoBow;
            itemDef.Damage = 5;
            itemDef.Model = m;
            itemDef.Create();

            // LANGBOGEN
            m = new ModelDef("itrw_longbow", "ItRw_Bow_M_01.mms");
            m.Create();
            itemDef = new ItemDef("itrw_longbow");
            itemDef.Name = "Langbogen";
            itemDef.Material = Enumeration.ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepBow;
            itemDef.Damage = 20;
            itemDef.Model = m;
            itemDef.Create();

            // BOLZEN
            m = new ModelDef("itrw_bolt", "ItRw_Bolt.3ds");
            m.Create();
            itemDef = new ItemDef("itrw_Bolt");
            itemDef.Name = "Bolzen";
            itemDef.Material = Enumeration.ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.AmmoXBow;
            itemDef.Damage = 5;
            itemDef.Model = m;
            itemDef.Create();

            // ARMBRUST
            m = new ModelDef("itrw_crossbow", "ItRw_Crossbow_L_01.mms");
            m.Create();
            itemDef = new ItemDef("itrw_crossbow");
            itemDef.Name = "Armbrust";
            itemDef.Material = Enumeration.ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepXBow;
            itemDef.Damage = 20;
            itemDef.Model = m;
            itemDef.Create();
        }

        void AddBowAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("BowT1", "Humans_BowT1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("BowT2", "Humans_BowT2"); model.AddOverlay(ov2);

            // Weapon drawing

            ScriptAniJob aniJob = new ScriptAniJob("drawbow");
            aniJob.BaseAniJob.ID = (int)SetAnis.DrawBow;
            aniJob.AniName = "drawBow";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(7000000, 2600000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(6000000, 2330000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(5300000, 2310000), ov2);

            aniJob = new ScriptAniJob("drawbowrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.DrawBowRun;
            aniJob.AniName = "T_MOVE_2_BOWMOVE";
            model.AddAniJob(aniJob);
            var ani = ScriptAni.NewDrawAni(7200000, 2650000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            aniJob = new ScriptAniJob("undrawbow");
            aniJob.BaseAniJob.ID = (int)SetAnis.UndrawBow;
            aniJob.AniName = "undrawBow";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(6500000, 4400000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(5500000, 3670000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(4800000, 2990000), ov2);

            aniJob = new ScriptAniJob("undrawbowrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.UndrawBowRun;
            aniJob.AniName = "T_BOWMOVE_2_MOVE";
            model.AddAniJob(aniJob);
            ani = ScriptAni.NewDrawAni(7200000, 4550000); ani.Layer = 2; aniJob.SetDefaultAni(ani);
        }

        void AddXBowAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("XBowT1", "Humans_CBowT1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("XBowT2", "Humans_CBowT2"); model.AddOverlay(ov2);

            // Weapon drawing

            ScriptAniJob aniJob = new ScriptAniJob("drawxbow");
            aniJob.BaseAniJob.ID = (int)SetAnis.DrawXBow;
            aniJob.AniName = "drawXBow";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(15000000, 2700000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(13300000, 2660000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(11700000, 2260000), ov2);

            aniJob = new ScriptAniJob("drawxbowrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.DrawXBowRun;
            aniJob.AniName = "T_MOVE_2_CBOWMOVE";
            model.AddAniJob(aniJob);
            var ani = ScriptAni.NewDrawAni(15200000, 2660000); ani.Layer = 2; aniJob.SetDefaultAni(ani);

            aniJob = new ScriptAniJob("undrawxbow");
            aniJob.BaseAniJob.ID = (int)SetAnis.UndrawXBow;
            aniJob.AniName = "undrawXBow";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(ScriptAni.NewDrawAni(14500000, 12300000));
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(12800000, 10140000), ov1);
            aniJob.AddOverlayAni(ScriptAni.NewDrawAni(11200000, 8940000), ov2);

            aniJob = new ScriptAniJob("undrawxbowrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.UndrawXBowRun;
            aniJob.AniName = "T_CBOWMOVE_2_MOVE";
            model.AddAniJob(aniJob);
            ani = ScriptAni.NewDrawAni(15200000, 6540000); ani.Layer = 2; aniJob.SetDefaultAni(ani);
        }
    }
}
