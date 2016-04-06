using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using GUC.Log;
using GUC.Scripting;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
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

            Logger.Log("######################## Finished #########################");
        }

        void AddSomeDefs()
        {
            // VOBS

            ModelDef m = new ModelDef("baum", "NW_Nature_BigTree_356P.3ds");
            m.Create();
            VobDef vobDef = new VobDef("baum");
            vobDef.Model = m;
            vobDef.Create();

            // ITEMS

            m = new ModelDef("zweihander", "ItMw_040_2h_PAL_Sword_03.3DS");
            m.Create();
            ItemDef itemDef = new ItemDef("zweihander");
            itemDef.Name = "Paladin Zweih�nder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = Enumeration.ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 100;
            itemDef.Create();

            m = new ModelDef("ITAR_Garde", "ItAr_Bloodwyn_ADDON.3ds");
            m.Create();

            itemDef = new ItemDef("ITAR_Garde");
            itemDef.Name = "Gardistenr�stung";
            itemDef.Material = Enumeration.ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Bloodwyn_ADDON.asc";
            itemDef.Model = m;
            itemDef.Create();

            // HUMAN MODEL

            m = new ModelDef("human", "humans.mds");
            m.Radius = 30;
            m.Height = 200;

            Add2hAttacks(m);

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

            m = new ModelDef("scavenger", "scavenger.mds");
            m.Radius = 80;
            m.Height = 160;
            m.Create();
            npcDef = new NPCDef("scavenger");
            npcDef.Name = "Scavenger";
            npcDef.Model = m;
            npcDef.BodyMesh = "Sca_Body";
            npcDef.BodyTex = 0;
            npcDef.HeadMesh = "";
            npcDef.HeadTex = 0;
            npcDef.Create();
        }

        void CreateTestWorld()
        {
            WorldDef wDef = new WorldDef();
            WorldInst.NewWorld = new WorldInst(default(WorldDef));

            VobInst vob = new VobInst(BaseVobDef.Get<VobDef>("baum"));
            vob.BaseInst.Spawn(WorldInst.NewWorld.BaseWorld, new Types.Vec3f(0, 0, 1000));

            var npc = new NPCInst(BaseVobDef.Get<NPCDef>("scavenger"));
            npc.BaseInst.Spawn(WorldInst.NewWorld.BaseWorld, new Types.Vec3f(0, 0, 500));

            WorldInst.NewWorld.Create();
            WorldInst.NewWorld.Clock.SetTime(new Types.WorldTime(0, 8), 5.0f);
            WorldInst.NewWorld.Clock.Start();
        }

        void Add2hAttacks(ModelDef model)
        {
            // 2h COMBO 1
            ScriptAniJob aniJob = new ScriptAniJob("attack2hfwd1");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HFwd1;
            aniJob.AniName = "s_2hAttack";

            var ani = new ScriptAni(10000000); ani.ComboTime = 5800000; ani.HitTime = 2800000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);

            // 2h COMBO 2
            aniJob = new ScriptAniJob("attack2hfwd2");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HFwd2;
            aniJob.AniName = "s_2hAttack";

            ani = new ScriptAni(6000000); ani.StartFrame = 31; ani.ComboTime = 4400000; ani.HitTime = 2300000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);

            // 2h LEFT ATTACK
            aniJob = new ScriptAniJob("attack2hleft");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HLeft;
            aniJob.AniName = "t_2hAttackL";

            ani = new ScriptAni(14000000); ani.ComboTime = 7200000; ani.HitTime = 2400000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);

            // 2h RIGHT ATTACK
            aniJob = new ScriptAniJob("attack2hright");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HRight;
            aniJob.AniName = "t_2hAttackR";

            ani = new ScriptAni(14000000); ani.ComboTime = 7200000; ani.HitTime = 2400000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);

            // 2h RUN ATTACK
            aniJob = new ScriptAniJob("attack2hrun");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HRun;
            aniJob.AniName = "t_2hAttackMove";

            ani = new ScriptAni(8800000); ani.ComboTime = 8800000; ani.HitTime = 6000000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);

            // 2h Parry
            aniJob = new ScriptAniJob("attack2hparry");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HParry;
            aniJob.AniName = "T_2HPARADE_0";

            ani = new ScriptAni(5600000); ani.ComboTime = 5600000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);

            // 2h Dodge
            aniJob = new ScriptAniJob("attack2hdodge");
            aniJob.BaseAniJob.ID = (int)SetAnis.Attack2HDodge;
            aniJob.AniName = "T_2HPARADEJUMPB";

            ani = new ScriptAni(9200000); ani.ComboTime = 9200000; aniJob.SetDefaultAni(ani);

            model.AddAniJob(aniJob);
        }
    }
}
