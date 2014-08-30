﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpcModelsLibrary.Readers
{
    public interface IJsonReader
    {
        Task<JObject> ReadJObjectAsync();
    }
}
