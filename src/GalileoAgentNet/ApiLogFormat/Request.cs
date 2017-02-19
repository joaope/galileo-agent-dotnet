using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public sealed class Request
    {
        public string Method { get; }

        public string Url { get; }

        public string HttpVersion { get; }

        public double HeadersSize { get; }

        public bool BodyCaptured { get; }

        public double BodySize { get; }

        public Header[] Headers { get; }

        public QueryString QueryString { get; }

        public PostData PostData { get; }

        public Request(
            string method,
            string url,
            string httpVersion,
            double headersSize,
            bool bodyCaptured,
            double bodySize,
            Header[] headers,
            QueryString queryString)
            : this(method, url, httpVersion, headersSize, bodyCaptured, bodySize, headers, queryString, null)
        {
        }

        public Request(
            string method, 
            string url,
            string httpVersion,
            double headersSize, 
            bool bodyCaptured, 
            double bodySize,
            Header[] headers, 
            QueryString queryString, 
            PostData postData)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (httpVersion == null) throw new ArgumentNullException(nameof(httpVersion));
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (queryString == null) throw new ArgumentNullException(nameof(queryString));
            if (headersSize < 0) throw new ArgumentException($"header size should not be lower than 0 - actual size: {headersSize}", nameof(headersSize));
            if (bodySize < 0) throw new ArgumentException($"body size should not be lower than 0 - actual size: {bodySize}", nameof(bodySize));

            Method = method;
            Url = url;
            HttpVersion = httpVersion;
            HeadersSize = headersSize;
            BodyCaptured = bodyCaptured;
            BodySize = bodySize;
            Headers = headers;
            QueryString = queryString;
            PostData = postData;
        }
    }
}