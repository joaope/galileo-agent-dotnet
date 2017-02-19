using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public class Log
    {
        public Creator Creator { get; }

        public Entry[] Entries { get; }

        public Log(Creator creator, Entry[] entries)
        {
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            if (entries == null) throw new ArgumentNullException(nameof(entries));

            Creator = creator;
            Entries = entries;
        }
    }
}