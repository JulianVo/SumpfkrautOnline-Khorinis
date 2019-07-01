using System;
using System.Collections.Generic;
using Autofac;
using RP_Server_Scripts.Definitions;

namespace RP_Server_Scripts
{
    internal class ScriptInitialization: IStartable
    {
        private readonly IEnumerable<IDefBuilder> _DefBuilders;
        private bool _WasRun;


        public ScriptInitialization(IEnumerable<IDefBuilder> defBuilders)
        {
            _DefBuilders = defBuilders ?? throw new ArgumentNullException(nameof(defBuilders));
        }

        public void Start()
        {
            if (_WasRun)
            {
                throw new InvalidOperationException("The script initialization was already executed.");
            }

            _WasRun = true;
            foreach (var defBuilder in _DefBuilders)
            {
                defBuilder.BuildDefinition();
            }
        }
    }
}
