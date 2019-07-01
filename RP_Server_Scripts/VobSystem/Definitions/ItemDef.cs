using GUC.Network;
using GUC.Types;
using GUC.WorldObjects.Instances;
using RP_Server_Scripts.VobSystem.Definitions.Item;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    // create an inherited class for each type ?

    public class ItemDef : NamedVobDef, ItemInstance.IScriptItemInstance
    {
        public override VobType VobType => VobType.Item;


        public new ItemInstance BaseDef => (ItemInstance) base.BaseDef;

        /// <summary>The material of this item. Controls the dropping sound.</summary>
        public ItemMaterials Material = ItemMaterials.Wood;

        private string _Effect = "";

        /// <summary>
        ///     Magic effect when laying in the world. (case insensitive) See
        ///     _work/Data/Scripts/System/VisualFX/VisualFxInst.d
        /// </summary>
        public string Effect
        {
            get => _Effect;
            set => _Effect = value == null ? "" : value.ToUpperInvariant();
        }

        private string _VisualChange = "";

        /// <summary>For Armors</summary>
        public string VisualChange
        {
            get => _VisualChange;
            set => _VisualChange = value == null ? "" : value.ToUpperInvariant();
        }

        public ItemTypes ItemType = ItemTypes.Misc;

        public bool IsAmmo => ItemType >= ItemTypes.AmmoBow && ItemType <= ItemTypes.AmmoXBow;
        public bool IsWeapon => ItemType >= ItemTypes.Wep1H && ItemType <= ItemTypes.WepXBow;
        public bool IsWepRanged => ItemType >= ItemTypes.WepBow && ItemType <= ItemTypes.WepXBow;
        public bool IsWepMelee => ItemType >= ItemTypes.Wep1H && ItemType <= ItemTypes.Wep2H;

        public float Range { get; set; }
        public int Damage { get; set; }
        public int Protection { get; set; }

        public Vec3f InvOffset;
        public Angles InvRotation;


        public override void OnWriteProperties(PacketWriter stream)
        {
            base.OnWriteProperties(stream);
            stream.Write((byte) ItemType);
            stream.Write(Name);
            stream.Write(_VisualChange);
            stream.Write((byte) Material);

            if (Range != 0)
            {
                stream.Write(true);
                stream.Write((ushort) Range);
            }
            else
            {
                stream.Write(false);
            }

            if (Damage != 0)
            {
                stream.Write(true);
                stream.Write((ushort) Damage);
            }
            else
            {
                stream.Write(false);
            }

            if (stream.Write(Protection != 0))
            {
                stream.Write((ushort) Protection);
            }

            if (stream.Write(!InvOffset.IsExactNull()))
            {
                stream.Write(InvOffset);
            }

            if (stream.Write(!InvRotation.IsExactNull()))
            {
                stream.WriteCompressedAngles(InvRotation);
            }
        }

        public override void OnReadProperties(PacketReader stream)
        {
            base.OnReadProperties(stream);
            ItemType = (ItemTypes) stream.ReadByte();
            Name = stream.ReadString();
            _VisualChange = stream.ReadString();
            Material = (ItemMaterials) stream.ReadByte();

            if (stream.ReadBit())
            {
                Range = stream.ReadUShort();
            }

            if (stream.ReadBit())
            {
                Damage = stream.ReadUShort();
            }

            if (stream.ReadBit())
            {
                Protection = stream.ReadUShort();
            }

            if (stream.ReadBit())
            {
                InvOffset = stream.ReadVec3f();
            }

            if (stream.ReadBit())
            {
                InvRotation = stream.ReadCompressedAngles();
            }
        }



        public ItemDef(string codeName, IBaseDefFactory baseDefFactory,   IVobDefRegistration registration) : base(codeName, baseDefFactory,  registration)
        {
        }

    }
}