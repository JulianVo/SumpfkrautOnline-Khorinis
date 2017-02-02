﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;

namespace GUC.Scripts.Sumpfkraut.EffectSystem.EffectHandlers
{

    public class NPCEffectHandler : VobEffectHandler
    {

        new public static readonly string _staticName = "NPCEffectHandler (static)";



        static NPCEffectHandler ()
        {
            PrintStatic(typeof(NPCEffectHandler), "Start subscribing listeners to events...");

            NPCInst.sOnHit += OnHit;

            PrintStatic(typeof(NPCEffectHandler), "Finished subscribing listeners to events...");
        }


        public NPCEffectHandler (List<Effect> effects, NPCDef linkedObject)
            : this("NPCEffectHandler (default)", effects, linkedObject)
        { }

        public NPCEffectHandler (List<Effect> effects, NPCInst linkedObject)
            : this("NPCEffectHandler (default)", effects, linkedObject)
        { }

        public NPCEffectHandler (string objName, List<Effect> effects, NPCDef linkedObject) 
            : base(objName, effects, linkedObject)
        { }

        public NPCEffectHandler (string objName, List<Effect> effects, NPCInst linkedObject) 
            : base(objName, effects, linkedObject)
        { }



        private static void OnHit (NPCInst attacker, NPCInst target, int damage)
        {
            throw new NotImplementedException();
        }



        //protected override void ApplyEffect (Effect effect, bool reverse = false)
        //{
        //    Print("Apply what? Naaaa!");
        //}

    }

}
