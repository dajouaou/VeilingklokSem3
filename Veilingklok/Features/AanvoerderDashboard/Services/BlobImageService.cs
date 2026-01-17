using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Veilingklok.Features.AanvoerderDashboard
{
    public class BlobImageService
    {
        private readonly BlobContainerClient _container;

        public BlobImageService(IConfiguration config)
        {
            var conn = config["BlobStorage:ConnectionString"];
            var containerName = config["BlobStorage:ContainerName"] ?? "uploads";

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("BlobStorage:ConnectionString ontbreekt.");

            _container = new BlobContainerClient(conn, containerName);

            // Zorgt dat container bestaat + publiek leesbaar (Blob)
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadAsync(IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Leeg bestand.");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

            var fileName = $"{Guid.NewGuid()}{ext}";
            var blob = _container.GetBlobClient(fileName);

            await using var stream = file.OpenReadStream();
            await blob.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = file.ContentType },
                cancellationToken: ct
            );

            return blob.Uri.ToString();
        }
    }
}
