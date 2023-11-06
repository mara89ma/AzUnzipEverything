using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace YourNamespace
{
    public class AzureBlobGZipProcessor
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<AzureBlobGZipProcessor> _logger;

        public AzureBlobGZipProcessor(string connectionString, ILogger<AzureBlobGZipProcessor> logger)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
        }

        public async Task ProcessGZipFileFromBlobStorage(string containerName, string blobName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                _logger.LogInformation($"Blob '{blobName}' exists.");

                // Download the GZip file to a memory stream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Create a GZip stream to decompress the memory stream
                    using (GZipStream decompressionStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        // Get the filename without the .gz extension
                        string fileName = blobName.Substring(0, blobName.Length - 3);

                        // Create a BlobClient for the extracted file
                        BlobClient extractedBlob = containerClient.GetBlobClient(fileName);

                        // Upload the extracted file to Azure Blob Storage
                        await extractedBlob.UploadAsync(decompressionStream);

                        _logger.LogInformation($"Extracted file '{fileName}' uploaded to the container '{containerName}'.");
                    }
                }
            }
            else
            {
                _logger.LogInformation($"Blob '{blobName}' does not exist.");
            }
        }
    }
}
