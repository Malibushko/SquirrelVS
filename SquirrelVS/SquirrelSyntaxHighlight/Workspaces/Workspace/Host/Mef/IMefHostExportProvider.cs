﻿using System;
using System.Collections.Generic;

namespace SquirrelSyntaxHighlight.Host.Mef
{
    internal interface IMefHostExportProvider
    {
        IEnumerable<Lazy<TExtension, TMetadata>> GetExports<TExtension, TMetadata>();
        IEnumerable<Lazy<TExtension>> GetExports<TExtension>();
    }
}
