using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Authentication
{
    public abstract class AuthenticationMethod
    {
        public string SecurityProtocol { get; }

        public AuthenticationMethod(string securityProtocol)
        {
            this.SecurityProtocol = securityProtocol;
        }

        public abstract void Apply(ClientConfig config);
            
    }
}
