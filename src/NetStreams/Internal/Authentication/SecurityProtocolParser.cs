using Confluent.Kafka;
using System;

namespace NetStreams.Authentication
{
    public class SecurityProtocolParser
    {
        internal static SecurityProtocol? Parse(string securityProtocol)
        {
            if (!string.IsNullOrEmpty(securityProtocol))
            {
                if (Enum.TryParse<SecurityProtocol>(securityProtocol, true, out var sp))
                {
                    return sp;
                }
                else
                {
                    throw new ArgumentException(nameof(securityProtocol));
                }
            }

            return null;
        }
    }
}
