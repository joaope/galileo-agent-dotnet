using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public class PostData
    {
        public string MimeType { get; }

        public string Encoding { get; }

        public string Text { get; }

        public PostData(string mimeType, string text)
            : this(mimeType, null, text)
        {
        }

        public PostData(string mimeType, string encoding, string text)
        {
            if (string.IsNullOrEmpty(mimeType)) throw new ArgumentException("mime type cannot be null or empty", nameof(mimeType));
            if (string.IsNullOrEmpty(text)) throw new ArgumentException("text cannot be null or empty", nameof(text));

            MimeType = mimeType;
            Encoding = encoding ?? "plain";
            Text = text;
        }
    }
}