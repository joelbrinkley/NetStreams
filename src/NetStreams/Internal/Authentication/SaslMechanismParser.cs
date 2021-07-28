using Confluent.Kafka;
using System;

namespace NetStreams.Authentication
{
    internal static class SaslMechanismParser
    {
        internal static SaslMechanism? Parse(string saslMechanism)
        {
            if (!string.IsNullOrEmpty(saslMechanism))
            {
                if (Enum.TryParse<SaslMechanism>(saslMechanism, true, out var sm))
                {
                    return sm;
                }
                else
                {
                    throw new ArgumentException(nameof(saslMechanism));
                }
            }

            return null;
        }
    }
}
