using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using GUC.Log;
using GUC.Scripting;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.Scripts.Sumpfkraut.Visuals;
using GUC.Utilities;
using GUC.Animations;
using GUC.Network;
using GUC.Types;
using GUC.WorldObjects;

namespace GUC.Scripts
{
    #region
    interface IProbabilityItems
    {
        List<ItemInst> GetItems();
    }

    class ProbItemGroup : IProbabilityItems
    {
        public struct BucketPair
        {
            public IProbabilityItems Bucket;
            public float Probability;
        }

        public List<BucketPair> pairs = new List<BucketPair>();

        public void Add(IProbabilityItems item, float probability)
        {
            BucketPair pair = new BucketPair();
            pair.Bucket = item;
            pair.Probability = probability;
            pairs.Add(pair);
        }

        public virtual List<ItemInst> GetItems()
        {
            List<ItemInst> result = new List<ItemInst>();
            foreach (BucketPair pair in pairs)
            {
                if (pair.Probability == 1.0f || Randomizer.GetDouble() >= pair.Probability)
                {
                    result.AddRange(pair.Bucket.GetItems());
                }
            }
            return result;
        }
    }

    class ProbItemGroupSingle : ProbItemGroup  // gibt ein item von vielen zur�ck
    {
        public override List<ItemInst> GetItems()
        {
            double prob = Randomizer.GetDouble();
            foreach (BucketPair pair in pairs)
            {
                if (prob < pair.Probability)
                    return pair.Bucket.GetItems();
                prob -= pair.Probability;
            }
            throw new Exception("Sum of all probabilities is < 1!");
        }
    }

    class ProbItem : IProbabilityItems // gibt ein item zuf�lliger anzahl zur�ck
    {
        public ItemDef itemDef;

        public int minAmount = 1; // 1 bis maxAmount
        public int maxAmount = 1; // minAmount bis unendlich
        public double exponent = 1; // Exponent der Wahrscheinlichkeit f�r exponentielle Verteilung

        public ProbItem(ItemDef def, int min = 1, int max = 1, double expo = 1)
        {
            this.itemDef = def;
            this.minAmount = min;
            this.maxAmount = max;
            this.exponent = expo;
        }

        public List<ItemInst> GetItems()
        {
            int amount;
            int diff = maxAmount - minAmount;
            if (diff > 0)
            {
                amount = minAmount + (int)(diff * Math.Pow(Randomizer.GetDouble(), exponent));
            }
            else
            {
                amount = minAmount;
            }

            ItemInst inst = new ItemInst(itemDef);
            inst.SetAmount(amount);
            return new List<ItemInst>() { inst };
        }
    }
    #endregion

    public partial class GUCScripts : ScriptInterface
    {
        public WorldObjects.VobGuiding.TargetCmd GetTestCmd(BaseVob target)
        {
            return new Sumpfkraut.AI.GuideCommands.GoToVobCommand((BaseVobInst)target.ScriptObject);
        }

        partial void pConstruct()
        {
            Logger.Log("######## Initalise SumpfkrautOnline ServerScripts #########");

            //Sumpfkraut.EffectSystem.Changes.ChangeInitializer.Init();
            //Sumpfkraut.EffectSystem.Destinations.DestInitializer.Init();

            //Sumpfkraut.Daedalus.AniParser.ReadMDSFiles();
            /*Sumpfkraut.Daedalus.ConstParser.ParseConstValues();
            Sumpfkraut.Daedalus.FuncParser.ParseConstValues();
            Sumpfkraut.Daedalus.PrototypeParser.ParsePrototypes();
            Sumpfkraut.Daedalus.InstanceParser.ParseInstances();

            Sumpfkraut.Daedalus.InstanceParser.AddInstances();

            Sumpfkraut.Daedalus.ConstParser.Free();
            Sumpfkraut.Daedalus.FuncParser.Free();
            Sumpfkraut.Daedalus.PrototypeParser.Free();
            Sumpfkraut.Daedalus.InstanceParser.Free();*/

            NPCInst.Requests.OnJump += (npc, move) => npc.EffectHandler.TryJump(move);
            NPCInst.Requests.OnDrawFists += npc => npc.EffectHandler.TryDrawFists();
            NPCInst.Requests.OnDrawWeapon += (npc, item) => npc.EffectHandler.TryDrawWeapon(item);
            NPCInst.Requests.OnFightMove += (npc, move) => npc.EffectHandler.TryFightMove(move);


            CreateTestWorld();
            AddSomeDefs();


            // -- Websocket-Server --
            Sumpfkraut.Web.WS.WSServer wsServer = new Sumpfkraut.Web.WS.WSServer();
            wsServer.Init();
            wsServer.Start();

            // -- command console --
            Sumpfkraut.CommandConsole.CommandConsole cmdConsole = new Sumpfkraut.CommandConsole.CommandConsole();

            //Sumpfkraut.TestingThings.Init();
            //Sumpfkraut.AI.TestingAI.Test();

            Logger.Log("######################## Finished #########################");
        }

        void CreateTestWorld()
        {
            WorldDef wDef = new WorldDef();
            WorldInst.Current = new WorldInst(default(WorldDef));

            WorldInst.Current.Create();
            WorldInst.Current.Clock.SetTime(new WorldTime(0, 8), 10.0f);
            WorldInst.Current.Clock.Start();

            /* for (int i = 0; i < WorldObjects.Instances.BaseVobInstance.GetCount(); i++)
             {
                 BaseVobInst inst;
                 BaseVobDef def;
                 if (BaseVobDef.TryGetDef(i, out def))
                 {
                     if (def is ItemDef)
                         inst = new ItemInst((ItemDef)def);
                     else if (def is NPCDef)
                         inst = new NPCInst((NPCDef)def);
                     else continue;

                     ((WorldObjects.VobGuiding.GuidedVob)inst.BaseInst).SetNeedsClientGuide(true);
                     inst.Spawn(WorldInst.Current, Randomizer.GetVec3fRad(new Types.Vec3f(0, 1500, 0), 30000), Randomizer.GetVec3fRad(new Types.Vec3f(0, 0, 0), 1).Normalise());
                 }
             }*/
        }

        void AddSomeDefs()
        {
            // HUMAN MODEL
            ModelDef m = new ModelDef("humans", "humans.mds");
            m.SetAniCatalog(new Sumpfkraut.Visuals.AniCatalogs.NPCCatalog());
            AddFistAnis(m);
            Add1HAnis(m);

            m.Radius = 80;
            m.Height = 180;
            m.FistRange = 40;
            m.Create();

            // NPCs
            NPCDef npcDef = new NPCDef("maleplayer");
            npcDef.Name = "Spieler";
            npcDef.Model = m;
            npcDef.BodyMesh = HumBodyMeshs.HUM_BODY_NAKED0.ToString();
            npcDef.BodyTex = (int)HumBodyTexs.G1Hero;
            npcDef.HeadMesh = HumHeadMeshs.HUM_HEAD_PONY.ToString();
            npcDef.HeadTex = (int)HumHeadTexs.Face_N_Player;
            npcDef.Create();

            AddItems();
        }

        #region Items

        void AddItems()
        {
            //ZWEIHANDER
            ModelDef m = new ModelDef("2hschwert", "ItMw_060_2h_sword_01.3DS");
            m.Create();
            ItemDef itemDef = new ItemDef("2hschwert");
            itemDef.Name = "Zweih�nder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 110;
            itemDef.Damage = 42;
            itemDef.Create();

            // GARDER�STUNG
            m = new ModelDef("ITAR_Garde", "ItAr_Bloodwyn_ADDON.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Garde");
            itemDef.Name = "Gardistenr�stung";
            itemDef.Material = ItemMaterials.Leather;
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
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 40;
            itemDef.Range = 90;
            itemDef.Create();

            // SCHATTENR�STUNG
            m = new ModelDef("ITAR_Schatten", "ItAr_Diego.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Schatten");
            itemDef.Name = "Schattenr�stung";
            itemDef.Material = ItemMaterials.Leather;
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
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 44;
            itemDef.Range = 95;
            itemDef.Create();

            // S�LDNERR�STUNG
            m = new ModelDef("ITAR_S�ldner", "ItAr_Sld_M.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_S�ldner");
            itemDef.Name = "S�ldnerr�stung";
            itemDef.Material = ItemMaterials.Leather;
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
            itemDef.Material = ItemMaterials.Wood;
            itemDef.Damage = 42;
            itemDef.Model = m;
            itemDef.Range = 75;
            itemDef.Create();

            // BANDITENR�STUNG
            m = new ModelDef("ITAR_bandit", "ItAr_Bdt_H.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_bandit");
            itemDef.Name = "Banditenr�stung";
            itemDef.Material = ItemMaterials.Leather;
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
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.AmmoBow;
            itemDef.Damage = 5;
            itemDef.Model = m;
            itemDef.Create();

            /*var projDef = new ProjDef("arrow");
            projDef.Model = m;
            projDef.Velocity = 0.0003f;
            projDef.Create();*/

            // LANGBOGEN
            m = new ModelDef("itrw_longbow", "ItRw_Bow_M_01.mms");
            m.Create();
            itemDef = new ItemDef("itrw_longbow");
            itemDef.Name = "Langbogen";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepBow;
            itemDef.Damage = 32;
            itemDef.Model = m;
            itemDef.Create();

            // BOLZEN
            m = new ModelDef("itrw_bolt", "ItRw_Bolt.3ds");
            m.Create();
            itemDef = new ItemDef("itrw_Bolt");
            itemDef.Name = "Bolzen";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.AmmoXBow;
            itemDef.Damage = 6;
            itemDef.Model = m;
            itemDef.Create();

            /*projDef = new ProjDef("bolt");
            projDef.Model = m;
            projDef.Velocity = 0.0003f;
            projDef.Create();*/

            // ARMBRUST
            m = new ModelDef("itrw_crossbow", "ItRw_Crossbow_L_01.mms");
            m.Create();
            itemDef = new ItemDef("itrw_crossbow");
            itemDef.Name = "Armbrust";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepXBow;
            itemDef.Damage = 32;
            itemDef.Model = m;
            itemDef.Create();


            // HOSE
            m = new ModelDef("ITAR_Prisoner", "ItAr_Prisoner.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Prisoner");
            itemDef.Name = "Malaks letzte Hose";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.Protection = 5;
            itemDef.VisualChange = "Armor_Prisoner.asc";
            itemDef.Model = m;
            itemDef.Create();

            // SCHWERER AST
            m = new ModelDef("ItMw_1h_Bau_Mace", "ItMw_010_1h_Club_01.3DS");
            m.Create();
            itemDef = new ItemDef("ItMw_1h_Bau_Mace");
            itemDef.Name = "Sehr schwerer Ast";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Wood;
            itemDef.Model = m;
            itemDef.Damage = 10;
            itemDef.Range = 40;
            itemDef.Create();
        }

        #endregion

        #region Fist Animations

        void AddFistAnis(ModelDef model)
        {
            #region Draw

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
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 5);
            aniJob.Layer = 2;

            model.AddAniJob(aniJob);

            #endregion

            #region Fighting

            // Fight Animations
            aniJob = new ScriptAniJob("fistattack_fwd0", "s_FistAttack", new ScriptAni(0, 15));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Combo, 9);
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_fwd1", "s_FistAttack", new ScriptAni(15, 29));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 9);
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fistattack_run", "t_FistAttackMove", new ScriptAni(0, 29));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Hit, 19);
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_parade", "t_FistParade_0", new ScriptAni(0, 12));
            model.AddAniJob(aniJob);

            aniJob = new ScriptAniJob("fist_jumpback", "t_FistParadeJumpB", new ScriptAni(0, 12));
            model.AddAniJob(aniJob);

            #endregion
        }

        #endregion

        #region 1H Anis

        void Add1HAnis(ModelDef model)
        {
            var ov1 = new ScriptOverlay("1HST1", "Humans_1hST1"); model.AddOverlay(ov1);
            var ov2 = new ScriptOverlay("1HST2", "Humans_1hST2"); model.AddOverlay(ov2);

            #region Draw

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
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 6);
            model.AddAniJob(aniJob);

            // Undraw 1h
            aniJob1 = new ScriptAniJob("undraw1h_part0", "t_1hRun_2_1h");
            model.AddAniJob(aniJob1);
            aniJob1.SetDefaultAni(new ScriptAni(0, 12) { { SpecialFrame.Draw, 12 } });
            aniJob1.AddOverlayAni(new ScriptAni(0, 6) { { SpecialFrame.Draw, 6 } }, ov1);
            aniJob1.AddOverlayAni(new ScriptAni(0, 8) { { SpecialFrame.Draw, 8 } }, ov2);

            aniJob2 = new ScriptAniJob("undraw1h_part1", "s_1h", new ScriptAni(0, 1));
            model.AddAniJob(aniJob2);
            aniJob2.SetDefaultAni(new ScriptAni(0, 1));
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob1.NextAni = aniJob2;

            aniJob3 = new ScriptAniJob("undraw1h_part2", "t_1h_2_Run", new ScriptAni(0, 2));
            model.AddAniJob(aniJob3);
            aniJob3.SetDefaultAni(new ScriptAni(0, 2));
            aniJob3.AddOverlayAni(new ScriptAni(0, 3), ov1);
            aniJob2.AddOverlayAni(new ScriptAni(0, 1), ov2);
            aniJob2.NextAni = aniJob3;

            // Undraw 1h running
            aniJob = new ScriptAniJob("undraw1h_running", "t_1hMove_2_Move", new ScriptAni(0, 24));
            aniJob.DefaultAni.SetSpecialFrame(SpecialFrame.Draw, 18);
            model.AddAniJob(aniJob);

            #endregion

            #region Fighting

            // Fwd attack 1
            ScriptAniJob job = new ScriptAniJob("1hattack_fwd0", "s_1hattack");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 23) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 15 } });
            job.AddOverlayAni(new ScriptAni(0, 33) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 14 } }, ov1);

            // fwd combo 2
            job = new ScriptAniJob("1hattack_fwd1", "s_1hattack");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(26, 40) { { SpecialFrame.Hit, 7 } });
            job.AddOverlayAni(new ScriptAni(33, 68) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 15 } }, ov1);

            // fwd combo 3
            job = new ScriptAniJob("1hattack_fwd2", "s_1hattack");
            model.AddAniJob(job);
            job.AddOverlayAni(new ScriptAni(68, 103) { { SpecialFrame.Hit, 6 }, { SpecialFrame.Combo, 17 } }, ov1);

            // fwd combo 4
            job = new ScriptAniJob("1hattack_fwd3", "s_1hattack");
            model.AddAniJob(job);
            job.AddOverlayAni(new ScriptAni(103, 120) { { SpecialFrame.Hit, 7 } }, ov1);

            // left attack
            job = new ScriptAniJob("1hattack_left", "t_1HAttackL");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 30) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 15 } });
            job.AddOverlayAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 10 } }, ov1);

            // right attack
            job = new ScriptAniJob("1hattack_right", "t_1HAttackR");
            model.AddAniJob(job);
            job.SetDefaultAni(new ScriptAni(0, 30) { { SpecialFrame.Hit, 5 }, { SpecialFrame.Combo, 15 } });
            job.AddOverlayAni(new ScriptAni(0, 24) { { SpecialFrame.Hit, 4 }, { SpecialFrame.Combo, 10 } }, ov1);

            // run attack
            job = new ScriptAniJob("1hattack_run", "t_1HAttackMove");
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

            #endregion
            /*

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
            model.AddAniJob(aniJob);*/
        }

        #endregion


        void Add2hAttacks(ModelDef model)
        {
            /*var ov1 = new ScriptOverlay("2HST1", "Humans_2hST1"); model.AddOverlay(ov1);
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
            model.AddAniJob(aniJob);*/
        }


        void AddBowAnis(ModelDef model)
        {
            /*var ov1 = new ScriptOverlay("BowT1", "Humans_BowT1"); model.AddOverlay(ov1);
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


            // AIMING

            aniJob = new ScriptAniJob("bowaim");
            aniJob.BaseAniJob.ID = (int)SetAnis.BowAim;
            aniJob.AniName = "t_BowWalk_2_BowAim";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(new ScriptAni(4000000));
            aniJob.AddOverlayAni(new ScriptAni(4000000), ov1);
            aniJob.AddOverlayAni(new ScriptAni(4000000), ov2);
            
            // RELOADING

            aniJob = new ScriptAniJob("bowreload");
            aniJob.BaseAniJob.ID = (int)SetAnis.BowReload;
            aniJob.AniName = "t_BowReload";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(new ScriptAni(11200000));
            aniJob.AddOverlayAni(new ScriptAni(10400000), ov1);
            aniJob.AddOverlayAni(new ScriptAni(8800000), ov2);

            // LOWERING

            aniJob = new ScriptAniJob("bowlower");
            aniJob.BaseAniJob.ID = (int)SetAnis.BowLower;
            aniJob.AniName = "t_BowAim_2_BowWalk";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(new ScriptAni(4000000));
            aniJob.AddOverlayAni(new ScriptAni(4000000), ov1);
            aniJob.AddOverlayAni(new ScriptAni(4000000), ov2);*/
        }

        void AddXBowAnis(ModelDef model)
        {
            /*var ov1 = new ScriptOverlay("XBowT1", "Humans_CBowT1"); model.AddOverlay(ov1);
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
            ani = ScriptAni.NewDrawAni(15200000, 12540000); ani.Layer = 2; aniJob.SetDefaultAni(ani);


            // AIMING

            aniJob = new ScriptAniJob("xbowaim");
            aniJob.BaseAniJob.ID = (int)SetAnis.XBowAim;
            aniJob.AniName = "t_CBowWalk_2_CBowAim";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(new ScriptAni(1600000));
            aniJob.AddOverlayAni(new ScriptAni(2000000), ov1);
            aniJob.AddOverlayAni(new ScriptAni(2000000), ov2);
            
            // RELOADING

            aniJob = new ScriptAniJob("Xbowreload");
            aniJob.BaseAniJob.ID = (int)SetAnis.XBowReload;
            aniJob.AniName = "t_CBowReload";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(new ScriptAni(12800000));
            aniJob.AddOverlayAni(new ScriptAni(11600000), ov1);
            aniJob.AddOverlayAni(new ScriptAni(9200000), ov2);

            // LOWERING

            aniJob = new ScriptAniJob("xbowlower");
            aniJob.BaseAniJob.ID = (int)SetAnis.XBowLower;
            aniJob.AniName = "t_CBowAim_2_CBowWalk";
            model.AddAniJob(aniJob);

            aniJob.SetDefaultAni(new ScriptAni(1600000));
            aniJob.AddOverlayAni(new ScriptAni(2000000), ov1);
            aniJob.AddOverlayAni(new ScriptAni(2000000), ov2);*/
        }
    }
}
