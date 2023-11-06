using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace AzUnzipEverything.Implementations
{
    public class GZipFileProcessor : FileProcessorBase
    {
        private readonly ILogger<FileProcessorBase> _logger;

        public GZipFileProcessor(CloudBlobContainer destinationContainer, SecretSettings secretSettings, ILogger<FileProcessorBase> logger) : base(destinationContainer, logger)
        {
            _logger = logger;
        }

        public override async Task ProcessFile(Stream blobStream)
        {
            try
            {
                _logger.LogInformation("Attempting GZIP extraction...");

                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream decompressionStream = new GZipStream(blobStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }

                    // You can further process the decompressed data here
                    // For example, save it to a file or perform other operations

                    _logger.LogInformation("GZIP extraction completed successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during GZIP extraction: {ex.Message}");
                throw; // Propagate the exception
            }
        }
    }
}
