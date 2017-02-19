using System;
using GalileoAgentNet.Extensions;

namespace GalileoAgentNet.ApiLogFormat
{
    public sealed class Creator
    {
        public string Name { get; }

        public string Version { get; }

        public Creator()
            : this("GalileoAgent.Net", "1.0.0")
        {
        }

        public Creator(string name, string version)
        {
            if (!name.HasValue()) throw new ArgumentException(nameof(name));
            if (!version.HasValue()) throw new ArgumentException(nameof(version));

            Name = name;
            Version = version;
        }
    }
}