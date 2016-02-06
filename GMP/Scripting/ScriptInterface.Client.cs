﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Scripting
{
    public partial interface ScriptInterface
    {
        /// <summary>
        /// Is called each frame.
        /// </summary>
        /// <param name="ticks"> Current DateTime.UtcNow.Ticks </param>
        void Update(long ticks);

        /// <summary>
        /// Is called once when the outgame menu usually starts up.
        /// </summary>
        void StartOutgame();

        /// <summary>
        /// Is called once when Gothic is ingame for the first time.
        /// </summary>
        void StartIngame();
    }
}