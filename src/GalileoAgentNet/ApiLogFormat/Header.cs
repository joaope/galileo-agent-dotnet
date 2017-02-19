using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public class Header
    {
        public string Name { get; }

        public string Value { get; }

        public Header(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("header value name cannot be null or empty", nameof(name));

            Name = name;
            Value = value ?? string.Empty;
        }
    }
}