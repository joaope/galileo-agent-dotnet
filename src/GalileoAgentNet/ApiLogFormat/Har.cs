using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public class Har
    {
        public Log Log { get; }

        public Har(Log log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            Log = log;
        }
    }
}