using GalileoAgentNet.ApiLogFormat;

namespace GalileoAgentNet
{
    public interface IEntriesQueue
    {
        int Size { get; }

        void Enqueue(Entry entry);

        void Enqueue(Entry[] entries);

        Entry Dequeue();

        Entry[] Dequeue(int dequeueCount);

        Entry[] DequeueAll(); 
    }
}