using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Serialization
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
