using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GalileoAgentNet.ApiLogFormat;
using GalileoAgentNet.Configuration;

namespace GalileoAgentNet.WebApi
{
    public sealed class GalileoAgentDelegatingHandler : DelegatingHandler
    {
        private static GalileoAgent agentInstance;

        public GalileoAgentDelegatingHandler(AgentConfiguration configuration, IEntriesQueue queue)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            agentInstance = new GalileoAgent(configuration, queue);
            agentInstance.Start();
        }

        public GalileoAgentDelegatingHandler(AgentConfiguration configuration)
        {

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            agentInstance = new GalileoAgent(configuration);
            agentInstance.Start();
        }

        public GalileoAgentDelegatingHandler(string galileoServiceToken)
        {
            if (string.IsNullOrEmpty(galileoServiceToken))
            {
                throw new ArgumentNullException(nameof(galileoServiceToken));
            }

            agentInstance = new GalileoAgent(galileoServiceToken);
            agentInstance.Start();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = HttpContext.Current;

            var requestBody = string.Empty;

            request
                .Content?
                .ReadAsStringAsync()
                .ContinueWith(task =>
                {
                    requestBody = task.Result;
                }, cancellationToken)
                .ConfigureAwait(false);

            var response = await base.SendAsync(request, cancellationToken);

            var responseBody = response.Content != null ? await response.Content.ReadAsStringAsync() : null;

            // request

            var headers = request
                .Headers
                .Select(h => new Header(h.Key, string.Join(",", h.Value)))
                .ToArray();

            var parsedQueryString = HttpUtility.ParseQueryString(request.RequestUri.Query);

            var queryStrings = new QueryString(
                parsedQueryString
                .AllKeys
                .Select(key => new QueryStringNameValuePair(key, parsedQueryString[key])));

            var headersSize = Encoding.UTF8.GetByteCount($"{context.Request.Headers}{Environment.NewLine}");
            var bodySize = Encoding.UTF8.GetByteCount(requestBody);

            var alfRequest = new Request(
                request.Method.Method,
                request.RequestUri.ToString(),
                request.Version.ToString(),
                headersSize,
                true,
                bodySize,
                headers,
                queryStrings,
                !string.IsNullOrEmpty(requestBody) ? new PostData(context.Request.ContentType ?? "plain", requestBody) : null);

            // response

            headersSize = Encoding.UTF8.GetByteCount($"{context.Response.Headers}{Environment.NewLine}");
            bodySize = Encoding.UTF8.GetByteCount(responseBody ?? string.Empty);

            headers = response
                .Headers
                .Select(h => new Header(h.Key, string.Join(",", h.Value)))
                .ToArray();

            var alfResponse = new Response(
                (int)response.StatusCode,
                response.ReasonPhrase,
                response.Version.ToString(),
                headersSize,
                true,
                bodySize,
                headers,
                new Content(
                    response.Content?.Headers?.ContentType?.MediaType ?? "plain", 
                    context.Response.ContentEncoding.WebName, 
                    responseBody));

            agentInstance.Process(new Entry(
                DateTime.UtcNow,
                context.Request.UserHostAddress,
                context.Server.MachineName,
                alfRequest,
                alfResponse,
                new Timings(10, 10, 10, 10, 10)));

            return response;
        }
    }
}
