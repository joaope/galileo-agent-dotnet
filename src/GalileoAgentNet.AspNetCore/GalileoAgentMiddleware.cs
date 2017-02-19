using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalileoAgentNet.ApiLogFormat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using QueryString = GalileoAgentNet.ApiLogFormat.QueryString;

namespace GalileoAgentNet.AspNetCore
{
    internal sealed class GalileoAgentMiddleware
    {
        private readonly RequestDelegate next;

        public GalileoAgentMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            var startedDateTime = DateTime.UtcNow;

            // Request build

            var startProcessingRequestTicks = DateTime.UtcNow.Ticks;

            var requestBodyStream = new MemoryStream();
            var originalRequestBody = context.Request.Body;

            await context.Request.Body.CopyToAsync(requestBodyStream).ConfigureAwait(false);
            requestBodyStream.Seek(0, SeekOrigin.Begin);
            var requestBody = new StreamReader(requestBodyStream).ReadToEnd();

            var headers = context
                .Request
                .Headers
                .Select(h => new Header(h.Key, h.Value))
                .ToArray();

            var queryStrings = new QueryString(context
                .Request
                .Query
                .Select(q => new QueryStringNameValuePair(q.Key, q.Value)));

            var headersSize = Encoding.UTF8.GetByteCount($"{context.Request.Headers}{Environment.NewLine}");
            var bodySize = Encoding.UTF8.GetByteCount(requestBody);

            var alfRequest = new Request(
                context.Request.Method,
                context.Request.GetDisplayUrl(),
                context.Request.Protocol,
                headersSize,
                true,
                bodySize,
                headers,
                queryStrings,
                !string.IsNullOrEmpty(requestBody) ? new PostData(context.Request.ContentType ?? "plain", requestBody) : null);

            var endProcessingRequestTicks = DateTime.UtcNow.Ticks;

            // Continue with pipeline

            var startWaitingResponseTicks = DateTime.UtcNow.Ticks;

            var originalBodyStream = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await next(context);
            context.Request.Body = originalRequestBody;

            var endWaitingResponseTicks = DateTime.UtcNow.Ticks;

            // Response build

            var startProcessingResponseTicks = DateTime.UtcNow.Ticks;

            await responseBodyStream.CopyToAsync(responseBodyStream);
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();

            headersSize = Encoding.UTF8.GetByteCount($"{context.Response.Headers}{Environment.NewLine}");
            bodySize = Encoding.UTF8.GetByteCount(responseBody);

            headers = context
                .Response
                .Headers
                .Select(h => new Header(h.Key, h.Value))
                .ToArray();

            var alfResponse = new Response(
                context.Response.StatusCode,
                context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase ?? context.Response.StatusCode.ToString(),
                context.Request.Protocol,
                headersSize,
                true,
                bodySize,
                headers,
                new Content(context.Response.ContentType ?? "plain", "plain", responseBody));

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);

            var endProcessingResponseTicks = DateTime.UtcNow.Ticks;

            // Send payload to agent

            var entry = new Entry(
                startedDateTime,
                context.Connection.RemoteIpAddress.ToString(),
                context.Connection.LocalIpAddress.ToString(),
                alfRequest,
                alfResponse,
                new Timings(
                    endProcessingRequestTicks - startProcessingRequestTicks,
                    endProcessingResponseTicks - startProcessingResponseTicks,
                    endWaitingResponseTicks - startWaitingResponseTicks));

            await GalileoAgentAccessor.AgentInstance.Process(entry);
        }
    }
}