using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
    }
}
