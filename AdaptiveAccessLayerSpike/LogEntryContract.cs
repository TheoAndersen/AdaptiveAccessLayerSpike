﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveAccessLayerSpike
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LogEntryContract : Attribute
    {
    }
}