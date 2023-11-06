using AzUnzipEverything.Abstractions;
using AzUnzipEverything.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SharpCompress.Archives.GZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzUnzipEverything.Implementations
{
    public class GZipFileProcessor : FileProcessorBase
    {
        private readonly SecretSettings _secretSettings;
        private readonly ILogger<FileProcessorBase> _logger;

        public GZipFileProcessor(CloudBlobContainer destinationContainer, SecretSettings secretSettings, ILogger<FileProcessorBase> logger) : base(destinationContainer, logger)
        {
            _secretSettings = secretSettings;
            _logger = logger;
        }

        public override async Task ProcessFile(Stream blobStream)
        {
            if (GZipArchive.IsGZipFile(blobStream))
            {
                var zipReaderOptions = new ReaderOptions()
                {
                    ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
                    Password = _secretSettings?.ZipPassword,
                    LookForHeader = true
                };

                _logger.LogInformation("Blob is a gzip file; beginning extraction....");
                blobStream.Position = 0;

                using var reader = GZipArchive.Open(blobStream, zipReaderOptions);

                await ExtractArchiveFiles(reader.Entries);
            }
        }
    }
}
