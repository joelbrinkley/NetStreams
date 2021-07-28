namespace NetStreams.Internal
{
    internal static class NetStreamConstants
    {
        public static string HEADER_TYPE = "X-NETSTREAM-TYPE";

        //security protocols
        public static string PLAINTEXT = "PLAINTEXT";
        public static string SSL = "SSL";
        public static string SASL_SSL = "SaslSsl";
        public static string SCRAM_256 = "ScramSha256";
        public static string SCRAM_512 = "ScramSha512";
    }
}
