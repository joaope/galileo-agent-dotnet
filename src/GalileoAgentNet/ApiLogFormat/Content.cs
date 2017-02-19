using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public sealed class Content
    {
        public string MimeType { get; set; }

        public string Encoding { get; set; }

        public string Text { get; set; }

        public Content(string mimeType, string encoding)
            : this(mimeType, encoding, null)
        {
        }

        public Content(string mimeType, string encoding, string text)
        {
            if (string.IsNullOrEmpty(mimeType)) throw new ArgumentException("mime type cannot be null or empty", nameof(mimeType));
            if (string.IsNullOrEmpty(encoding)) throw new ArgumentException("encoding cannot be null or empty", nameof(encoding));

            MimeType = mimeType;
            Encoding = encoding;
            Text = text ?? string.Empty;
        }
    }
}