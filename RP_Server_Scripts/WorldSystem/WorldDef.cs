using System;
using System.Collections.Generic;
using GUC.Utilities;
using GUC.WorldObjects;

namespace RP_Server_Scripts.WorldSystem
{
    public class WorldDef : ExtendedObject
    {

        protected World baseWorld;
        public World BaseWorld => baseWorld;



        public void Create()
        {
            baseWorld.Create();
            throw new NotImplementedException();
        }


        public void Delete()
        {
            baseWorld.Delete();
            throw new NotImplementedException();
        }



        private List<List<List<object>>> sqlResult;
        private readonly bool sqlResultInUse = false;

        public void DropSQLResult()
        {
            if (!sqlResultInUse)
            {
                sqlResult = null;
            }
        }


        public WorldDef()
            : this("WorldDef (default)")
        {
        }

        public WorldDef(string objName)
        {
            SetObjName(objName);
        }
    }
}