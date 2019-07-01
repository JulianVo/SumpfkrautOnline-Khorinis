using System;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.Definitions
{
    class MiscDefBuilder : IDefBuilder
    {
        private readonly IBaseDefFactory _BaseDefFactory;
        private readonly IVobDefRegistration _Registration;

        public MiscDefBuilder(IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            _BaseDefFactory = baseDefFactory ?? throw new ArgumentNullException(nameof(baseDefFactory));
            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        public void BuildDefinition()
        {
            ModelDef m = new ModelDef("trollpalisade");
            m.Visual = "OW_TROLLPALISSADE.3DS";
            m.Create();

            VobDef vobDef = new VobDef("trollpalisade",_BaseDefFactory,_Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("invwall");
            m.Visual = "TRANS_WAND.3DS";
            m.Create();

            vobDef = new VobDef("invwall", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("irdorathwall");
            m.Visual = "NW_DRAGONISLE_INVISIBLEORCWALL_01.3DS";
            m.Create();

            vobDef = new VobDef("irdorathwall", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();


            m = new ModelDef("gate");
            m.Visual = "OC_LOB_GATE_BIG.3DS";
            m.Create();

            vobDef = new VobDef("gate", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();


            m = new ModelDef("bridge");
            m.Visual = "NW_DRAGONISLE_BIGBRIDGE_01.3DS";
            m.Create();

            vobDef = new VobDef("bridge", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();


            m = new ModelDef("door");
            m.Visual = "DOOR_NW_DRAGONISLE_02.MDS";
            m.Create();

            vobDef = new VobDef("door", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();


            m = new ModelDef("door_puzzle_left");
            m.Visual = "EVT_MAINHALL_DOOR_LEFT_01.3DS";
            m.Create();

            vobDef = new VobDef("door_puzzle_left", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("door_puzzle_right");
            m.Visual = "EVT_MAINHALL_DOOR_RIGHT_01.3DS";
            m.Create();
            vobDef = new VobDef("door_puzzle_right", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("redeye");
            m.Visual = "THEREDEYE.pfx";
            m.Create();
            vobDef = new VobDef("redeye", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = false;
            vobDef.Create();

            m = new ModelDef("bigdoor_head_right");
            m.Visual = "NW_DRAGONISLE_BIGDOOR_HEAD_RIGHT_01.3DS";
            m.Create();
            vobDef = new VobDef("bigdoor_head_right", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("bigdoor_head_left");
            m.Visual = "NW_DRAGONISLE_BIGDOOR_HEAD_LEFT_01.3DS";
            m.Create();
            vobDef = new VobDef("bigdoor_head_left", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("bigdoor_right");
            m.Visual = "NW_DRAGONISLE_BIGDOOR_RIGHT_01.3DS";
            m.Create();
            vobDef = new VobDef("bigdoor_right", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();

            m = new ModelDef("bigdoor_left");
            m.Visual = "NW_DRAGONISLE_BIGDOOR_LEFT_01.3DS";
            m.Create();
            vobDef = new VobDef("bigdoor_left", _BaseDefFactory, _Registration);
            vobDef.Model = m;
            vobDef.CDDyn = vobDef.CDStatic = true;
            vobDef.Create();
        }
    }
}
