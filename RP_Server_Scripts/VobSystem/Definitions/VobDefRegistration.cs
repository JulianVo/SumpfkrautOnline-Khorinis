using System;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    /// <summary>
    /// Class that handles the registration of vob definitions and adds them to the individual def lists.
    /// </summary>
    internal class VobDefRegistration : IVobDefRegistration
    {
        private readonly INpcDefListRegistration _NpcDefRegistration;
        private readonly IVobDefListRegistration _VobDefRegistration;
        private readonly IItemDefRegistration _ItemDefRegistration;


        public VobDefRegistration(INpcDefListRegistration npcDefRegistration, IVobDefListRegistration vobDefRegistration, IItemDefRegistration itemDefRegistration)
        {
            _NpcDefRegistration = npcDefRegistration ?? throw new ArgumentNullException(nameof(npcDefRegistration));
            _VobDefRegistration = vobDefRegistration ?? throw new ArgumentNullException(nameof(vobDefRegistration));
            _ItemDefRegistration = itemDefRegistration ?? throw new ArgumentNullException(nameof(itemDefRegistration));
        }

        public void Register(BaseVobDef def)
        {
            switch (def)
            {
                case NpcDef npcDef:
                    _NpcDefRegistration.AddDef(npcDef);
                    break;
                case ProjDef projectileDef:
                    break;
                case ItemDef itemDef:
                    _ItemDefRegistration.AddDef(itemDef);
                    break;
                case VobDef vobDef:
                    _VobDefRegistration.AddDef(vobDef);
                    break;
            }
        }

        public void Unregister(BaseVobDef def)
        {
            switch (def)
            {
                case NpcDef npcDef:
                    _NpcDefRegistration.RemoveDef(npcDef);
                    break;
                case ProjDef projectileDef:
                    break;
                case ItemDef itemDef:
                    _ItemDefRegistration.RemoveDef(itemDef);
                    break;
                case VobDef vobDef:
                    _VobDefRegistration.RemoveDef(vobDef);
                    break;
            }
        }
    }
}