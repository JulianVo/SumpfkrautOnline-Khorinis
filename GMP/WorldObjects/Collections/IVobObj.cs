﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Enumeration;

namespace GUC.Client.WorldObjects.Collections
{
    public interface IVobObj<TKey>
    {
        TKey ID { get; }
        VobType VobType { get; }
    }
}