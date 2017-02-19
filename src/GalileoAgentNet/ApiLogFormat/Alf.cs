using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public sealed class Alf
    {
        public string Version { get; } = "1.1.0";

        public string ServiceToken { get; }

        public string Environment { get; }

        public Har Har { get; }

        public Alf(string serviceToken, string environment, Har har)
        {
            if (har == null) throw new ArgumentNullException(nameof(har));
            if (string.IsNullOrEmpty(serviceToken)) throw new ArgumentException(nameof(serviceToken));

            ServiceToken = serviceToken;
            Environment = environment ?? string.Empty;
            Har = har;
        }
    }
}