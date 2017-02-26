using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GalileoAgentNet.ApiLogFormat;
using GalileoAgentNet.Configuration;
using GalileoAgentNet.Extensions;
using GalileoAgentNet.Internal;

namespace GalileoAgentNet
{
    public sealed class GalileoAgent : IDisposable
    {
        private readonly CollectorConnector collectorConnector;

        private readonly Timer flushTimer;

        public string SupportedFormatVersion { get; } = "1.1.0";

        public string AgentName { get; } = "GalileoAgentNet";

        public string AgentVersion { get; } = typeof(GalileoAgent).GetAssemblyVersion();

        public IEntriesQueue Queue { get; }

        public AgentConfiguration Configuration { get; }

        public GalileoAgent(AgentConfiguration configuration, IEntriesQueue queue)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            Configuration = configuration;
            Queue = queue;

            collectorConnector = new CollectorConnector(
                new HttpClient(), 
                Configuration.Host, 
                Configuration.Port,
                Configuration.GalileoServiceToken,
                AgentName,
                AgentVersion,
                Configuration.Environment,
                Configuration.RequestCompression);

            flushTimer = new Timer(ElapsedFlushTimeout, this, 0, Configuration.FlushTimeout * 1000);
        }

        public GalileoAgent(AgentConfiguration configuration)
            : this(configuration, new InMemoryEntriesQueue())
        {
        }

        public GalileoAgent(string galileoServiceToken)
            : this(new AgentConfiguration(galileoServiceToken))
        {
        }

        public void Start()
        {
            flushTimer.Change(0, Configuration.FlushTimeout * 1000);
        }

        public void Process(Entry entry)
        {
            if (Queue.Size == Configuration.QueueSize)
            {
                FlushQueue();
            }
            
            Queue.Enqueue(entry);
        }

        private void ElapsedFlushTimeout(object state)
        {
            if (Queue.Size == 0)
            {
                return;
            }

            FlushQueue();
        }

        private void FlushQueue()
        {
            flushTimer.Change(Timeout.Infinite, Timeout.Infinite);

            Task.Factory.StartNew(async () =>
                {
                    var result = await collectorConnector.Send(Queue.DequeueAll());
                })
                .ContinueWith(t =>
                {
                    flushTimer.Change(0, Configuration.FlushTimeout * 1000);
                })
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            flushTimer?.Dispose();
            collectorConnector?.Dispose();
        }
    }
}
