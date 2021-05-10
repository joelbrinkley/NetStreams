namespace NetStreams.Configuration
{
    public class DeliveryMode
    {
        public static DeliveryMode At_Most_Once = new DeliveryMode();
        public static DeliveryMode At_Least_Once = new DeliveryMode { EnableAutoCommit = false, AutoCommitIntervalMs = 5000 };

        public bool EnableAutoCommit { get; set; } = true;
        public int AutoCommitIntervalMs { get; set; } = 5000;
    }


}
