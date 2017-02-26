using System.IO;
using System.IO.Compression;

namespace GalileoAgentNet.Internal
{
    internal sealed class GZipContent : CompressedContent
    {
        public GZipContent(string content, CompressionLevel compressionLevel) 
            : base("gzip", content, compressionLevel)
        {
        }

        public GZipContent(string content)
            : base("gzip", content)
        {
        }

        protected override Stream CreateCompressionStream(Stream inputStream, CompressionLevel compressionLevel)
        {
            return new GZipStream(inputStream, compressionLevel, true);
        }
    }
}