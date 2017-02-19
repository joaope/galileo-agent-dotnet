using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public sealed class QueryStringNameValuePair
    {
        public string Name { get; }

        public string Value { get; }

        public QueryStringNameValuePair(string name, string value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("query string value's name cannot be null or empty", nameof(name));

            Name = name;
            Value = value ?? string.Empty;
        }
    }
}