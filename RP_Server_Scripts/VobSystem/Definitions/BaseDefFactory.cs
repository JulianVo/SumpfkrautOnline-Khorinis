using System;
using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    internal class BaseDefFactory : IBaseDefFactory
    {
        public BaseVobInstance Build(BaseVobInstance.IScriptBaseVobInstance scriptBaseVob)
        {
            if (scriptBaseVob == null)
            {
                throw new ArgumentNullException(nameof(scriptBaseVob));
            }

            //The order is important here(classes further down the inheritance chain have higher priority)
            if (scriptBaseVob is ItemInstance.IScriptItemInstance scriptItemInstance)
            {
                return new ItemInstance(scriptItemInstance);
            }

            if (scriptBaseVob is NPCInstance.IScriptNPCInstance scriptNpcInstance)
            {
                return new NPCInstance(scriptNpcInstance);
            }
            
            if (scriptBaseVob is ProjectileInstance.IScriptProjectileInstance scriptProjectileInstance)
            {
                return new ProjectileInstance(scriptProjectileInstance);
            }
            if (scriptBaseVob is MobInterInstance.IScriptMobInterInstance scriptMobInterInstance)
            {
                return new MobInterInstance(scriptMobInterInstance);
            }
            if (scriptBaseVob is MobInstance.IScriptMobInstance scriptMobInstance)
            {
                return new MobInstance(scriptMobInstance);
            }
            if (scriptBaseVob is VobInstance.IScriptVobInstance scriptVob)
            {
                return new VobInstance(scriptVob);
            }

            throw new InvalidOperationException($"The given instance of {nameof(BaseVobInstance.IScriptBaseVobInstance)} is of a not supported type");
        }
    }
}
