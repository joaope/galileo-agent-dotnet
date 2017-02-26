using System;

namespace GalileoAgentNet.Configuration
{
    public sealed class AgentConfiguration
    {
        public string GalileoServiceToken { get; }

        public string Environment { get; set; } = string.Empty;

        public LogBodies LogBodies { get; set; }

        public int RetryCount { get; set; }

        public int ConnectionTimeout { get; set; } = 30;

        public int FlushTimeout { get; set; } = 20;

        public int QueueSize { get; set; } = 1000;

        public string Host { get; set; }

        public int Port { get; set; }

        public string FailLogPath { get; set; } = "/dev/null";

        public CollectorRequestCompression RequestCompression { get; set; }

        public AgentConfiguration(string galileoServiceToken)
            : this(galileoServiceToken, "collector.galileo.mashape.com", 443)
        {
        }

        public AgentConfiguration(string galileoServiceToken, string host)
            : this(galileoServiceToken, host, 443)
        {
        }

        public AgentConfiguration(string galileoServiceToken, string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (string.IsNullOrEmpty(galileoServiceToken))
            {
                throw new ArgumentException("service token cannot be null or empty", nameof(galileoServiceToken));
            }

            Host = host;
            Port = port;
            GalileoServiceToken = galileoServiceToken;
        }
    }
}
