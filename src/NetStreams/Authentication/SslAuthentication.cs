using Confluent.Kafka;
using NetStreams.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Authentication
{
    public class SslAuthentication : AuthenticationMethod
    {
        public string SslCaLocation { get; }
        public string SslCertificateLocation { get; }
        public string SslKeyLocation { get; }
        public string SslKeyPassword { get; }

        public SslAuthentication(string sslCaLocation, string sslCertificateLocation, string sslKeyLocation, string sslKeyPassword) :base (NetStreamConstants.SSL)
        {
            SslCaLocation = sslCaLocation;
            SslCertificateLocation = sslCertificateLocation;
            SslKeyLocation = sslKeyLocation;
            SslKeyPassword = sslKeyPassword;
        }      

        public override void Apply(ClientConfig config)
        {
            config.SecurityProtocol = SecurityProtocolParser.Parse(SecurityProtocol);
            config.SslCaLocation = SslCaLocation;
            config.SslCertificateLocation = SslCertificateLocation;
            config.SslKeyLocation = SslKeyLocation;
            config.SslKeyPassword = SslKeyPassword;
        }
    }
}
