using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GalileoAgentNet.Internal;
using Xunit;

namespace GalileoAgentNet.UnitTests
{
    public class DeflateContentTests
    {
        [Fact]
        public async Task DeflateContentShouldCompressContentDifferentLevels()
        {
            // arrange
            const string contentStr = @"
This is

Some content";

            // act
            var contentFastest = new DeflateContent(contentStr, CompressionLevel.Fastest);
            var contentOptimal = new DeflateContent(contentStr, CompressionLevel.Optimal);
            var contentNoCompression = new DeflateContent(contentStr, CompressionLevel.NoCompression);

            // assert
            Assert.Contains("deflate", contentFastest.Headers.ContentEncoding);
            Assert.Contains("deflate", contentOptimal.Headers.ContentEncoding);
            Assert.Contains("deflate", contentNoCompression.Headers.ContentEncoding);

            using (var stream = await contentFastest.ReadAsStreamAsync())
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                Assert.Equal(contentStr, streamReader.ReadToEnd());
            }

            using (var stream = await contentOptimal.ReadAsStreamAsync())
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                Assert.Equal(contentStr, streamReader.ReadToEnd());
            }

            using (var stream = await contentNoCompression.ReadAsStreamAsync())
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                Assert.Equal(contentStr, streamReader.ReadToEnd());
            }
        }

        [Fact]
        public void DeflateContentShouldThrowOnNullContent()
        {
            // arrange
            const string contentStr = null;

            // act
            var exception = Record.Exception(() => new DeflateContent(contentStr, CompressionLevel.Fastest));

            // act
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}
