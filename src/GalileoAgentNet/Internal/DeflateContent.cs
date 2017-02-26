using System.IO;
using System.IO.Compression;

namespace GalileoAgentNet.Internal
{
    internal sealed class DeflateContent : CompressedContent
    {
        public DeflateContent(string content, CompressionLevel compressionLevel) 
            : base("deflate", content, compressionLevel)
        {
        }

        public DeflateContent(string content)
            : base("deflate", content)
        {
        }

        protected override Stream CreateCompressionStream(Stream inputStream, CompressionLevel compressionLevel)
        {
            return new DeflateStream(inputStream, compressionLevel, true);
        }
    }
}