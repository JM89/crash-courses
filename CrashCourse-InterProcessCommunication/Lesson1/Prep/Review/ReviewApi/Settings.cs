namespace ReviewApi
{
    public class Settings
    {
        public string ServiceName { get; set; }
        public int InducedFailureRateFactor { get; set; }
        public int InducedLatencyFactor { get; set; }

        public bool IsValid =>
            InducedFailureRateFactor >= 0
            && InducedFailureRateFactor <= 100
            && InducedLatencyFactor >= 0;
    }
}
