using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GalileoAgentNet.Internal;
using Xunit;

namespace GalileoAgentNet.UnitTests
{
    public class GZipContentTests
    {
        [Fact]
        public async Task GZipContentShouldCompressContentDifferentLevels()
        {
            // arrange
            const string contentStr = @"
This is

Some content";

            // act
            var contentFastest = new GZipContent(contentStr, CompressionLevel.Fastest);
            var contentOptimal = new GZipContent(contentStr, CompressionLevel.Optimal);
            var contentNoCompression = new GZipContent(contentStr, CompressionLevel.NoCompression);

            // assert
            Assert.Contains("gzip", contentFastest.Headers.ContentEncoding);
            Assert.Contains("gzip", contentOptimal.Headers.ContentEncoding);
            Assert.Contains("gzip", contentNoCompression.Headers.ContentEncoding);

            using (var stream = await contentFastest.ReadAsStreamAsync())
            using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                Assert.Equal(contentStr, streamReader.ReadToEnd());
            }

            using (var stream = await contentOptimal.ReadAsStreamAsync())
            using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                Assert.Equal(contentStr, streamReader.ReadToEnd());
            }

            using (var stream = await contentNoCompression.ReadAsStreamAsync())
            using (var deflateStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(deflateStream))
            {
                Assert.Equal(contentStr, streamReader.ReadToEnd());
            }
        }

        [Fact]
        public void GZipContentShouldThrowOnNullContent()
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