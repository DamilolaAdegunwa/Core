﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace CreativeCoders.Net.Servers.Http.AspNetCore
{
    [PublicAPI]
    public interface IWebHostConfig
    {
        IEnumerable<string> Urls { get; }
        
        bool DisableLogging { get; set; }
    }
}