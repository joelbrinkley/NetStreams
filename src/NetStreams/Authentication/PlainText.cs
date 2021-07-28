using Confluent.Kafka;
using NetStreams.Internal;


namespace NetStreams.Authentication
{
    public class PlainTextAuthentication : AuthenticationMethod
    {

        public PlainTextAuthentication() : base(NetStreamConstants.PLAINTEXT)
        {

        }

        public override void Apply(ClientConfig config)
        {
            config.SecurityProtocol = SecurityProtocolParser.Parse(SecurityProtocol);
        }
    }
}
