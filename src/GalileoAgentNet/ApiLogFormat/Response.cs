using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public class Response
    {
        public int Status { get; }

        public string StatusText { get; }

        public string HttpVersion { get; }

        public double HeadersSize { get; }

        public bool BodyCaptured { get; }

        public double BodySize { get; }

        public Header[] Headers { get; }

        public Content Content { get; }

        public Response(
            int status, 
            string statusText, 
            string httpVersion, 
            double headersSize, 
            bool bodyCaptured, 
            double bodySize, 
            Header[] headers, 
            Content content)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (string.IsNullOrEmpty(statusText)) throw new ArgumentException("statusText cannot be null or empty", nameof(statusText));

            Status = status;
            StatusText = statusText;
            HttpVersion = httpVersion;
            HeadersSize = headersSize;
            BodyCaptured = bodyCaptured;
            BodySize = bodySize;
            Headers = headers;
            Content = content;
        }
    }
}
