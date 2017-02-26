using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GalileoAgentNet.Internal
{
    internal abstract class CompressedContent : HttpContent
    {
        private readonly byte[] content;

        private readonly CompressionLevel compressionLevel;

        protected CompressedContent(string encodingHeaderType, string content)
            : this(encodingHeaderType, content, CompressionLevel.Optimal)
        {
        }

        protected CompressedContent(string encodingHeaderType, string content, CompressionLevel compressionLevel)
        {
            if (string.IsNullOrEmpty(encodingHeaderType))
            {
                throw new ArgumentException("encodingHeaderType should be specified", nameof(encodingHeaderType));
            }

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.compressionLevel = compressionLevel;
            this.content = Encoding.UTF8.GetBytes(content);

            Headers.ContentEncoding.Add(encodingHeaderType);
        }

        protected abstract Stream CreateCompressionStream(Stream inputStream, CompressionLevel compressionLevel);

        protected override bool TryComputeLength(out long length)
        {
            length = content.Length;
            return true;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var compressionStream = CreateCompressionStream(stream, compressionLevel);
            
            return compressionStream
                .WriteAsync(content, 0, content.Length)
                .ContinueWith(task =>
                {
                    compressionStream.Dispose();
                });
        }
    }
}
