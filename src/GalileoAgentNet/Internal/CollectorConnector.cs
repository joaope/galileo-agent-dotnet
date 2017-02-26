using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalileoAgentNet.ApiLogFormat;
using GalileoAgentNet.Configuration;
using GalileoAgentNet.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GalileoAgentNet.Internal
{
    internal sealed class CollectorConnector : IDisposable
    {
        private readonly CollectorConnectorResponseHandler responseHandler = new CollectorConnectorResponseHandler();

        private readonly string collectorHost;

        private readonly int collectorPort;

        private readonly string galileoServiceToken;

        private readonly string appName;

        private readonly string appVersion;

        private readonly string environment;

        private readonly CollectorRequestCompression requestCompression;

        private readonly HttpClient httpClient;

        public CollectorConnector(
            HttpClient httpClient,
            string collectorHost,
            int collectorPort,
            string galileoServiceToken,
            string appName,
            string appVersion,
            string environment,
            CollectorRequestCompression requestCompression)
        {
            if (!collectorHost.HasValue())
            {
                throw new ArgumentException(nameof(collectorHost));
            }

            this.httpClient = httpClient;
            this.collectorHost = collectorHost;
            this.collectorPort = collectorPort;
            this.galileoServiceToken = galileoServiceToken;
            this.appName = appName;
            this.appVersion = appVersion;
            this.environment = environment;
            this.requestCompression = requestCompression;
        }

        public async Task<CollectorConnectorResult> Send(Entry[] entries)
        {
            if (entries == null || entries.Length == 0)
            {
                return new CollectorConnectorResult(CollectorConnectorResultStatus.Success);
            }

            var uri = new UriBuilder("https", collectorHost, collectorPort, "1.1.0/single").Uri;

            var alf = new Alf(
                galileoServiceToken,
                environment,
                new Har(
                    new Log(
                        new Creator(appName, appVersion),
                        entries.ToArray())));

            var jsonSerializerSets = new JsonSerializerSettings
            {
                ContractResolver = new AlfContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var jsonRequestBody = JsonConvert.SerializeObject(alf, Formatting.Indented, jsonSerializerSets);
            HttpContent content;

            switch (requestCompression)
            {
                case CollectorRequestCompression.PlainText:
                    content = new StringContent(jsonRequestBody, Encoding.UTF8);
                    break;

                case CollectorRequestCompression.GZip:
                    content = new GZipContent(jsonRequestBody);
                    break;

                case CollectorRequestCompression.Deflate:
                    content = new DeflateContent(jsonRequestBody);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(requestCompression), requestCompression, null);
            }

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            return await responseHandler.ProcessResponse(await httpClient.PostAsync(uri, content));
        }

        public void Dispose()
        {
            httpClient?.Dispose();
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