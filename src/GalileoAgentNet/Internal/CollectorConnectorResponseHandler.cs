using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GalileoAgentNet.Internal
{
    internal class CollectorConnectorResponseHandler
    {
        private static readonly Regex PartialSuccessAlfErrorRegex = new Regex(@"ALF\[(\d+)\](.*)", RegexOptions.Compiled);

        internal virtual async Task<CollectorConnectorResult> ProcessResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var responseBody = JsonConvert.DeserializeObject<CollectorResponse>(content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new CollectorConnectorResult(
                        CollectorConnectorResultStatus.Success,
                        responseBody.Sent,
                        responseBody.Saved,
                        null,
                        null);
                }

                if ((int)response.StatusCode == 207)
                {
                    var faultyAlfs = new List<int>();

                    foreach (var responseBodyError in responseBody.Errors)
                    {
                        var match = PartialSuccessAlfErrorRegex.Match(responseBodyError);

                        if (match.Groups.Count == 3 &&
                            int.TryParse(match.Groups[1].Value, out int entryIndex))
                        {
                            faultyAlfs.Add(entryIndex);
                        }
                    }

                    return new CollectorConnectorResult(
                        CollectorConnectorResultStatus.PartialSuccess,
                        responseBody.Sent,
                        responseBody.Saved,
                        faultyAlfs.ToArray(),
                        responseBody.Errors);
                }

                return new CollectorConnectorResult(CollectorConnectorResultStatus.UnknownResponseStatus);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new CollectorConnectorResult(CollectorConnectorResultStatus.BadRequest);
            }

            if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge)
            {
                return new CollectorConnectorResult(CollectorConnectorResultStatus.EntriesPayloadTooLarge);
            }

            return new CollectorConnectorResult(CollectorConnectorResultStatus.UnknownServerError);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class CollectorResponse
        {
            public string[] Errors { get; set; }

            public int Sent { get; set; }

            public int Saved { get; set; }
        }
    }
}