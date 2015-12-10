﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Network;

namespace GUC.Client.WorldObjects.Instances
{
    public class MobLadderInstance : MobInterInstance
    {
        new public readonly static Enumeration.VobType sVobType = Enumeration.VobType.MobLadder;
        new public readonly static Collections.InstanceDictionary Instances = Network.Server.sInstances.GetDict(sVobType);

        public MobLadderInstance(string instanceName, object scriptObject)
            : this(0, instanceName, scriptObject)
        {
        }

        public MobLadderInstance(ushort ID, string instanceName, object scriptObject) 
            : base(ID, instanceName, scriptObject)
        {
            this.VobType = sVobType;
        }

        public new static Action<MobLadderInstance, PacketWriter> OnWriteProperties;
        internal override void WriteProperties(PacketWriter stream)
        {
            base.WriteProperties(stream);

            if (MobLadderInstance.OnWriteProperties != null)
            {
                MobLadderInstance.OnWriteProperties(this, stream);
            }
        }
    }
}