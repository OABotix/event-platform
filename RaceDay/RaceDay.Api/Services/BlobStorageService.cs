using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException("Blob connection string missing.");
        var containerName = configuration["AzureBlobStorage:ContainerName"] ?? "raceday-images";

        _container = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file supplied.");

        var extension = Path.GetExtension(file.FileName);
        var blobName = $"{folder}/{Guid.NewGuid()}{extension}";

        await _container.CreateIfNotExistsAsync();
        var blob = _container.GetBlobClient(blobName);

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        return blob.Uri.ToString();
    }
}