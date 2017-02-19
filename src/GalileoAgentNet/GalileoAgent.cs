using System;
using System.Threading;
using System.Threading.Tasks;
using GalileoAgentNet.ApiLogFormat;
using GalileoAgentNet.Configuration;
using GalileoAgentNet.Extensions;
using GalileoAgentNet.Internal;

namespace GalileoAgentNet
{
    public class GalileoAgent : IDisposable
    {
        private readonly CollectorConnector collectorConnector;

        private readonly Timer flushTimer;

        public string SupportedFormatVersion { get; } = "1.1.0";

        public virtual string AgentName { get; } = "GalileoAgentNet";

        public virtual string AgentVersion { get; } = typeof(GalileoAgent).GetAssemblyVersion();

        public virtual IEntriesQueue Queue { get; }

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

            collectorConnector = new CollectorConnector(Configuration.Host, Configuration.Port);
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

        public async Task Process(Entry entry)
        {
            if (Queue.Size == Configuration.QueueSize)
            {
                await FlushQueue();
            }
            
            Queue.Enqueue(entry);
        }

        private void ElapsedFlushTimeout(object state)
        {
            if (Queue.Size == 0)
            {
                return;
            }

            flushTimer.Change(Timeout.Infinite, Timeout.Infinite);

            Task.Run(async () => await FlushQueue())
                .ContinueWith(t =>
                {
                    flushTimer.Change(0, Configuration.FlushTimeout * 1000);
                })
                .ConfigureAwait(false);
        }

        private async Task FlushQueue()
        {
            var result = await collectorConnector.Send(
                Configuration.GalileoServiceToken, 
                AgentName, 
                AgentVersion,
                Configuration.Environment,
                Queue.DequeueAll());
        }

        public void Dispose()
        {
            flushTimer?.Dispose();
        }
    }
}
