namespace GalileoAgentNet.Internal
{
    internal sealed class CollectorConnectorResult
    {
        public CollectorConnectorResultStatus Status { get; }

        public int Sent { get; }

        public int Saved { get; }

        public int[] FaultyAlfPackages { get; }

        public string[] ErrorMessages { get; }

        public CollectorConnectorResult(CollectorConnectorResultStatus status)
            : this(status, 0, 0, new int[0], new string[0])
        {
        }

        public CollectorConnectorResult(
            CollectorConnectorResultStatus status,
            int sent,
            int saved,
            int[] faultyAlfPackages,
            string[] errorMessages)
        {
            Status = status;
            Sent = sent;
            Saved = saved;
            FaultyAlfPackages = faultyAlfPackages ?? new int[0];
            ErrorMessages = errorMessages ?? new string[0];
        }
    }
}