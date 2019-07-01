using System;
using GUC.Network;
using GUC.Utilities;
using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    public abstract class BaseVobDef : ExtendedObject, BaseVobInstance.IScriptBaseVobInstance
    {
        private readonly IVobDefRegistration _Registration;

        protected BaseVobDef(string codeName, IBaseDefFactory baseDefFactory, IVobDefRegistration registration)
        {
            if (baseDefFactory == null)
            {
                throw new ArgumentNullException(nameof(baseDefFactory));
            }


            if (string.IsNullOrWhiteSpace(codeName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(codeName));
            }

            _Registration = registration ?? throw new ArgumentNullException(nameof(registration));

            BaseDef = baseDefFactory.Build(this);
            CodeName = codeName;
        }

        public byte GetVobType()
        {
            return (byte)VobType;
        } // for base vob interface

        public abstract VobType VobType { get; }


        // Definition
        public BaseVobInstance BaseDef { get; }

        public int ID => BaseDef.ID;
        public bool IsStatic => BaseDef.IsStatic;
        public bool IsCreated => BaseDef.IsCreated;


        public void Create()
        {
            BaseDef.Create();
            _Registration.Register(this);
        }


        public void Delete()
        {
            BaseDef.Delete();
            _Registration.Unregister(this);
        }

        public virtual void OnWriteProperties(PacketWriter stream)
        {
        }

        public virtual void OnReadProperties(PacketReader stream)
        {
        }

        public string CodeName { get; }
    }
}