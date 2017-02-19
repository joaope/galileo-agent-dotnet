using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GalileoAgentNet.ApiLogFormat;

namespace GalileoAgentNet
{
    public class InMemoryEntriesQueue : IEntriesQueue
    {
        private readonly ConcurrentQueue<Entry> internalQueue = new ConcurrentQueue<Entry>();

        public virtual int Size => internalQueue.Count;

        public virtual void Enqueue(Entry entry)
        {
            if (entry == null)
            {
                return;
            }

            internalQueue.Enqueue(entry);
        }

        public virtual void Enqueue(Entry[] entries)
        {
            if (entries == null || entries.Length == 0)
            {
                return;
            }

            foreach (var entry in entries.Where(a => a != null))
            {
                internalQueue.Enqueue(entry);
            }
        }

        public virtual Entry Dequeue()
        {
            Entry entry;
            return internalQueue.TryDequeue(out entry) ? entry : null;
        }

        public virtual Entry[] Dequeue(int dequeueCount)
        {
            if (dequeueCount <= 0)
            {
                return new Entry[0];
            }

            Entry entry;
            var entriesList = new List<Entry>();

            while (internalQueue.TryDequeue(out entry) && entriesList.Count < dequeueCount)
            {
                entriesList.Add(entry);
            }

            return entriesList.ToArray();
        }

        public virtual Entry[] DequeueAll()
        {
            return Dequeue(Size);
        }
    }
}