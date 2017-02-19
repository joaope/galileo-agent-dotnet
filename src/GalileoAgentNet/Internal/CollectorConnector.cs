using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalileoAgentNet.ApiLogFormat;
using GalileoAgentNet.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GalileoAgentNet.Internal
{
    internal sealed class CollectorConnector
    {
        private readonly CollectorConnectorResponseHandler responseHandler = new CollectorConnectorResponseHandler();

        private readonly string collectorHost;

        private readonly int collectorPort;

        public CollectorConnector(string collectorHost, int collectorPort)
        {
            if (!collectorHost.HasValue())
            {
                throw new ArgumentException(nameof(collectorHost));
            }

            this.collectorHost = collectorHost;
            this.collectorPort = collectorPort;
        }

        public async Task<CollectorConnectorResult> Send(
            string galileoServiceToken,
            string appName,
            string appVersion,
            string environment,
            Entry[] entries)
        {
            if (entries == null || entries.Length == 0)
            {
                return new CollectorConnectorResult(CollectorConnectorResultStatus.Success);
            }

            using (var httpClient = new HttpClient())
            {
                var uri = new UriBuilder("https", collectorHost, collectorPort, "1.1.0/single").Uri;

                var alf = new Alf(
                    galileoServiceToken,
                    null,
                    new Har(
                        new Log(
                            new Creator(appName, appVersion),
                            entries.ToArray())));

                var jsonSerializerSets = new JsonSerializerSettings
                {
                    ContractResolver = new AlfContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(alf, Formatting.Indented, jsonSerializerSets), 
                    Encoding.UTF8, 
                    "application/json");

                return await responseHandler.ProcessResponse(await httpClient.PostAsync(uri, content));
            }
        }

        private sealed class AlfContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (property.DeclaringType == typeof(Entry))
                {
                    if (property.PropertyName == "clientIpAddress")
                    {
                        property.PropertyName = "clientIPAddress";
                    }
                    else if (property.PropertyName == "serverIpAddress")
                    {
                        property.PropertyName = "serverIPAddress";
                    }
                }

                return property;
            }
        }
    }
}