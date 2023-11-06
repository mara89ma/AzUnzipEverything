using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

public class GZipFileProcessor
{
    private readonly string _destinationPath;
    private readonly ILogger _logger;

    public GZipFileProcessor(string destinationPath, ILogger logger)
    {
        _destinationPath = destinationPath;
        _logger = logger;
    }

    public byte[] ProcessFile(Stream blobStream)
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
                // For example, you can save it to a file or perform other operations.
                // To access the byte array, use: decompressedStream.ToArray()

                _logger.LogInformation("GZIP extraction completed successfully.");
                return decompressedStream.ToArray(); // Return decompressed data if needed
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during GZIP extraction: {ex.Message}");
            throw; // Propagate the exception
        }
    }
}

// Usage example:
// Assuming you have the necessary settings (destination path and logger) initialized:
// var gzipProcessor = new GZipFileProcessor("/path/to/destination", logger);
// using (FileStream blobStream = File.OpenRead("your_gzip_file.gz"))
// {
//     byte[] decompressedData = gzipProcessor.ProcessFile(blobStream);
//     // Use the decompressedData byte array as needed
// }
