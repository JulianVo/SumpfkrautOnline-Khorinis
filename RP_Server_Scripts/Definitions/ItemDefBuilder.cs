using System;
using GUC.Types;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.VobSystem.Definitions.Item;

namespace RP_Server_Scripts.Definitions
{
    class ItemDefBuilder : IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public ItemDefBuilder(IBaseDefFactory baseDefFactory,IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            // TEMPLER MINE

            ModelDef m = new ModelDef("leichter_zweihaender", "ItMw_032_2h_sword_light_01.3DS");
            m.Create();
            ItemDef itemDef = new ItemDef("leichter_zweihaender", _BaseDefFactory, _Registration);
            itemDef.Name = "Leichter Zweihänder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 100;
            itemDef.Damage = 50;
            itemDef.Create();

            m = new ModelDef("ITAR_templer", "ARMOR_TPLM.3DS");
            m.Create();
            itemDef = new ItemDef("ITAR_templer", _BaseDefFactory, _Registration);
            itemDef.Name = "Templerrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.Protection = 35;
            itemDef.VisualChange = "ARMOR_TPLM.ASC";
            itemDef.Model = m;
            itemDef.Create();

            // GARDIST MINE

            m = new ModelDef("grobes_schwert", "ItMw_025_1h_sld_sword_01.3DS");
            m.Create();
            itemDef = new ItemDef("grobes_schwert", _BaseDefFactory, _Registration);
            itemDef.Name = "Grobes Schwert";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 45;
            itemDef.Range = 70;
            itemDef.Create();

            m = new ModelDef("ITAR_garde_l", "ARMOR_GRDL.3DS");
            m.Create();
            itemDef = new ItemDef("ITAR_garde_l", _BaseDefFactory, _Registration);
            itemDef.Name = "Leichte Garderüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.Protection = 40;
            itemDef.VisualChange = "ARMOR_GRDL.ASC";
            itemDef.InvOffset = new Vec3f(0, -20, -20);
            itemDef.Model = m;
            itemDef.Create();

            // GARDIST BURG

            m = new ModelDef("2hschwert", "ItMw_060_2h_sword_01.3DS");
            m.Create();
            itemDef = new ItemDef("2hschwert", _BaseDefFactory, _Registration);
            itemDef.Name = "Zweihänder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 110;
            itemDef.Damage = 50;
            itemDef.Create();

            m = new ModelDef("ITAR_Garde", "ItAr_Bloodwyn_ADDON.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Garde", _BaseDefFactory, _Registration);
            itemDef.Name = "Gardistenrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.Protection = 40;
            itemDef.VisualChange = "Armor_Bloodwyn_ADDON.asc";
            itemDef.InvOffset = new Vec3f(0, -20, -20);
            itemDef.Model = m;
            itemDef.Create();

            // SCHATTEN BURG

            m = new ModelDef("1hschwert", "Itmw_025_1h_Mil_Sword_broad_01.3DS");
            m.Create();
            itemDef = new ItemDef("1hschwert", _BaseDefFactory, _Registration);
            itemDef.Name = "Breitschwert";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 45;
            itemDef.Range = 80;
            itemDef.Create();

            m = new ModelDef("ITAR_Schatten", "ItAr_Diego.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Schatten", _BaseDefFactory, _Registration);
            itemDef.Name = "Schattenrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Diego.asc";
            itemDef.Protection = 35;
            itemDef.Model = m;
            itemDef.Create();

            // SÖLDNER BURG

            m = new ModelDef("2haxt", "ItMw_060_2h_axe_heavy_01.3DS");
            m.Create();
            itemDef = new ItemDef("2haxt", _BaseDefFactory, _Registration);
            itemDef.Name = "Söldneraxt";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 50;
            itemDef.Range = 100;
            itemDef.Create();

            m = new ModelDef("ITAR_Söldner", "ItAr_Sld_M.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Söldner", _BaseDefFactory, _Registration);
            itemDef.Name = "Söldnerrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Sld_M.asc";
            itemDef.Protection = 40;
            itemDef.Model = m;
            itemDef.Create();

            // BANDIT BURG

            m = new ModelDef("1haxt", "ItMw_025_1h_sld_axe_01.3DS");
            m.Create();
            itemDef = new ItemDef("1haxt", _BaseDefFactory, _Registration);
            itemDef.Name = "Grobes Kriegsbeil";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Wood;
            itemDef.Damage = 45;
            itemDef.Model = m;
            itemDef.Range = 80;
            itemDef.Create();

            m = new ModelDef("ITAR_bandit", "ItAr_Bdt_H.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_bandit", _BaseDefFactory, _Registration);
            itemDef.Name = "Banditenrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Bdt_H.asc";
            itemDef.Protection = 35;
            itemDef.Model = m;
            itemDef.Create();

            // PFEIL
            m = new ModelDef("itrw_arrow", "ItRw_Arrow.3ds");
            m.Create();
            itemDef = new ItemDef("itrw_arrow", _BaseDefFactory, _Registration);
            itemDef.Name = "Pfeil";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.AmmoBow;
            itemDef.Model = m;
            itemDef.Create();

            var projDef = new ProjDef("arrow", _BaseDefFactory, _Registration);
            projDef.Create();

            // BOGEN
            m = new ModelDef("itrw_longbow", "ItRw_Bow_M_01.mms");
            m.Create();
            itemDef = new ItemDef("itrw_longbow", _BaseDefFactory, _Registration);
            itemDef.Name = "Langbogen";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepBow;
            itemDef.Damage = 50;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("itrw_shortbow", "ItRw_Bow_L_01.mms");
            m.Create();
            itemDef = new ItemDef("itrw_shortbow", _BaseDefFactory, _Registration);
            itemDef.Name = "Kurzbogen";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepBow;
            itemDef.Damage = 45;
            itemDef.Model = m;
            itemDef.Create();

            // BOLZEN
            m = new ModelDef("itrw_bolt", "ItRw_Bolt.3ds");
            m.Create();
            itemDef = new ItemDef("itrw_Bolt", _BaseDefFactory, _Registration);
            itemDef.Name = "Bolzen";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.AmmoXBow;
            itemDef.Model = m;
            itemDef.Create();

            // ARMBRUST
            m = new ModelDef("light_xbow", "ItRw_Crossbow_L_01.mms");
            m.Create();
            itemDef = new ItemDef("light_xbow", _BaseDefFactory, _Registration);
            itemDef.Name = "Leichte Armbrust";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepXBow;
            itemDef.Damage = 45;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("war_xbow", "ItRw_Crossbow_M_02.mms");
            m.Create();
            itemDef = new ItemDef("war_xbow", _BaseDefFactory, _Registration);
            itemDef.Name = "Kriegsarmbrust";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepXBow;
            itemDef.Damage = 50;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("heavy_xbow", "ItRw_Crossbow_H_02.mms");
            m.Create();
            itemDef = new ItemDef("heavy_xbow", _BaseDefFactory, _Registration);
            itemDef.Name = "Schwere Armbrust";
            itemDef.Material = ItemMaterials.Wood;
            itemDef.ItemType = ItemTypes.WepXBow;
            itemDef.Damage = 50;
            itemDef.Model = m;
            itemDef.Create();

            // HOSE
            m = new ModelDef("ITAR_Prisoner", "ItAr_Prisoner.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_Prisoner", _BaseDefFactory, _Registration);
            itemDef.Name = "Malaks letzte Hose";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.Protection = 10;
            itemDef.VisualChange = "Armor_Prisoner.asc";
            itemDef.Model = m;
            itemDef.Create();

            // SCHWERER AST
            m = new ModelDef("ItMw_1h_Bau_Mace", "ItMw_010_1h_Club_01.3DS");
            m.Create();
            itemDef = new ItemDef("ItMw_1h_Bau_Mace", _BaseDefFactory, _Registration);
            itemDef.Name = "Sehr schwerer Ast";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Wood;
            itemDef.Model = m;
            itemDef.Damage = 15;
            itemDef.Range = 50;
            itemDef.Create();

            // ORK WAFFEN
            m = new ModelDef("krush_pach", "ItMw_2H_OrcAxe_02.3DS");
            m.Create();
            itemDef = new ItemDef("krush_pach", _BaseDefFactory, _Registration);
            itemDef.Name = "Krush Pach";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 45;
            itemDef.Range = 80;
            itemDef.Create();

            m = new ModelDef("orc_sword", "ItMw_2H_OrcSword_02.3DS");
            m.Create();
            itemDef = new ItemDef("orc_sword", _BaseDefFactory, _Registration);
            itemDef.Name = "Orkisches Kriegsschwert";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 50;
            itemDef.Range = 100;
            itemDef.Create();

            m = new ModelDef("echsenschwert", "ItMw_2H_OrcSword_01.3DS");
            m.Create();
            itemDef = new ItemDef("echsenschwert", _BaseDefFactory, _Registration);
            itemDef.Name = "Echsenschwert";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 45;
            itemDef.Range = 80;
            itemDef.Create();

            // Miliz
            m = new ModelDef("ITAR_miliz_s", "ItAr_MIL_M.3DS");
            m.Create();
            itemDef = new ItemDef("ITAR_miliz_s", _BaseDefFactory, _Registration);
            itemDef.Name = "Schwere Milizrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_MIL_M.asc";
            itemDef.Protection = 35;
            itemDef.Model = m;
            itemDef.Create();

            // Ritter
            m = new ModelDef("ITAR_ritter", "ItAr_Pal_M.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_ritter", _BaseDefFactory, _Registration);
            itemDef.Name = "Ritterrüstung";
            itemDef.Material = ItemMaterials.Metal;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Pal_M.asc";
            itemDef.Protection = 40;
            itemDef.Model = m;
            itemDef.Create();


            // Tempel
            m = new ModelDef("ITAR_bandit_m", "ItAr_Bdt_M.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_bandit_m", _BaseDefFactory, _Registration);
            itemDef.Name = "mittlere Banditenrüstung";
            itemDef.Material = ItemMaterials.Leather;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Bdt_M.asc";
            itemDef.Protection = 35;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("grober_2h", "ItMw_035_2h_sld_sword_01.3DS");
            m.Create();
            itemDef = new ItemDef("grober_2h", _BaseDefFactory, _Registration);
            itemDef.Name = "Grober Zweihänder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 50;
            itemDef.Range = 110;
            itemDef.Create();

            m = new ModelDef("ITAR_pal_skel", "ItAr_Pal_H.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_pal_skel", _BaseDefFactory, _Registration);
            itemDef.Name = "Alte Paladinrüstung";
            itemDef.Material = ItemMaterials.Metal;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Pal_Skeleton.asc";
            itemDef.Protection = 40;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("ITAR_pal_h", "ItAr_Pal_H.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_pal_h", _BaseDefFactory, _Registration);
            itemDef.Name = "Paladinrüstung";
            itemDef.Material = ItemMaterials.Metal;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Pal_H.asc";
            itemDef.Protection = 50;
            itemDef.Model = m;
            itemDef.Create();

            // trank
            m = new ModelDef("hptrank", "ItPo_Health_01.3ds");
            m.Create();
            itemDef = new ItemDef("hptrank", _BaseDefFactory, _Registration);
            itemDef.Name = "Heiltrank";
            itemDef.ItemType = ItemTypes.Drinkable;
            itemDef.Material = ItemMaterials.Glass;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("itmw_schlachtaxt", "ItMw_070_2h_axe_heavy_03.3DS");
            m.Create();
            itemDef = new ItemDef("itmw_schlachtaxt", _BaseDefFactory, _Registration);
            itemDef.Name = "Schlachtaxt";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 50;
            itemDef.Range = 100;
            itemDef.Create();

            m = new ModelDef("ITAR_garde_h", "ItAr_Thorus_ADDON.3ds");
            m.Create();
            itemDef = new ItemDef("ITAR_garde_h", _BaseDefFactory, _Registration);
            itemDef.Name = "Schwere Gardistenrüstung";
            itemDef.Material = ItemMaterials.Metal;
            itemDef.ItemType = ItemTypes.Armor;
            itemDef.VisualChange = "Armor_Thorus_ADDON.asc";
            itemDef.Protection = 50;
            itemDef.InvOffset = new Vec3f(0, -20, -20);
            itemDef.Model = m;
            itemDef.Create();

            //ItLs_Torch_01.3ds
            //ITLSTORCHBURNING.ZEN
            m = new ModelDef("torch_burning", "ITLSTORCHBURNING.ZEN");
            m.Create();
            itemDef = new ItemDef("torch_burning", _BaseDefFactory, _Registration);
            itemDef.Name = "Brennende Fackel";
            itemDef.ItemType = ItemTypes.Torch;
            itemDef.Material = ItemMaterials.Wood;
            itemDef.Model = m;
            itemDef.Create();

            m = new ModelDef("paladinschwert", "ItMw_030_1h_PAL_Sword_02.3DS");
            m.Create();
            itemDef = new ItemDef("paladinschwert", _BaseDefFactory, _Registration);
            itemDef.Name = "Paladinschwert";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 45;
            itemDef.Range = 60;
            itemDef.Create();

            m = new ModelDef("paladin2h", "ItMw_040_2h_PAL_Sword_03.3DS");
            m.Create();
            itemDef = new ItemDef("paladin2h", _BaseDefFactory, _Registration);
            itemDef.Name = "Paladinzweihänder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 100;
            itemDef.Damage = 50;
            itemDef.Create();

            m = new ModelDef("rostiger2h", "ItMw_025_2h_Sword_old_01.3DS");
            m.Create();
            itemDef = new ItemDef("rostiger2h", _BaseDefFactory, _Registration);
            itemDef.Name = "rostiger Zweihänder";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 100;
            itemDef.Damage = 30;
            itemDef.Create();

            m = new ModelDef("rostigeaxt", "ItMw_025_2h_Misc_Axe_old_01.3DS");
            m.Create();
            itemDef = new ItemDef("rostigeaxt", _BaseDefFactory, _Registration);
            itemDef.Name = "rostige Axt";
            itemDef.ItemType = ItemTypes.Wep2H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Range = 70;
            itemDef.Damage = 35;
            itemDef.Create();

            m = new ModelDef("rostiger1h", "ItMw_020_1h_sword_old_01.3DS");
            m.Create();
            itemDef = new ItemDef("rostiger1h", _BaseDefFactory, _Registration);
            itemDef.Name = "rostiges Schwert";
            itemDef.ItemType = ItemTypes.Wep1H;
            itemDef.Material = ItemMaterials.Metal;
            itemDef.Model = m;
            itemDef.Damage = 20;
            itemDef.Range = 50;
            itemDef.Create();
        }
    }
}