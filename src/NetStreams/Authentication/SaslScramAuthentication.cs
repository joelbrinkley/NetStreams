using Confluent.Kafka;
using NetStreams.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Authentication
{
    public abstract class SaslScramAuthentication : AuthenticationMethod
    {
        public string UserName { get; }
        public string Password { get; }
        public string SslCaLocation { get; }
        public string SaslMechanism { get; }

        public SaslScramAuthentication(string userName, string password, string sslCaLocation, string saslMechanism) : base(NetStreamConstants.SASL_SSL)
        {
            UserName = userName;
            Password = password;
            SslCaLocation = sslCaLocation;
            SaslMechanism = saslMechanism;
        }

        public override void Apply(ClientConfig config)
        {
            config.SecurityProtocol = SecurityProtocolParser.Parse(SecurityProtocol);
            config.SaslMechanism = SaslMechanismParser.Parse(SaslMechanism);
            config.SslCaLocation = SslCaLocation;
            config.SaslUsername = UserName;
            config.SaslPassword = Password;
        }
    }

    public class SaslScram256Authentication : SaslScramAuthentication
    {
        public SaslScram256Authentication(string userName, string password, string sslCaLocation)
            : base(userName, password, sslCaLocation, NetStreamConstants.SCRAM_256 )
        {
        }
    }
    public class SaslScram512Authentication : SaslScramAuthentication
    {
        public SaslScram512Authentication(string userName, string password, string sslCaLocation)
            : base(userName, password, sslCaLocation, NetStreamConstants.SCRAM_512)
        {
        }
    }
}
