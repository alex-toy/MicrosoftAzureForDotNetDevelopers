using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace StorageApp.Services.BlobStorage;

public class BlobStorageService : IBlobStorageService
{
    public string ContainerName { get; set; } = string.Empty;

    private readonly IConfiguration _config;

    public BlobStorageService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> UploadBlob(IFormFile formFile, string imageName)
    {
        string blobName = $"{imageName}{Path.GetExtension(formFile.FileName)}";
        BlobContainerClient container = await GetBlobContainerClient();
        using MemoryStream memoryStream = new();
        formFile.CopyTo(memoryStream);
        memoryStream.Position = 0;
        BlobClient blob = container.GetBlobClient(imageName);
        await blob.UploadAsync(memoryStream, overwrite:true);
        return blobName;
    }

    public async Task<string> GetBlobUrl(string imageName)
    {
        BlobContainerClient container = await GetBlobContainerClient();
        BlobClient blob = container.GetBlobClient(imageName);
        BlobSasBuilder blobSasBuilder = new()
        {
            BlobContainerName = blob.BlobContainerName,
            BlobName = blob.Name,
            ExpiresOn = DateTime.UtcNow.AddSeconds(120),
            Protocol = SasProtocol.Https,
            Resource = "b"
        };

        blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
        return blob.GenerateSasUri(blobSasBuilder).ToString();
    }

    public async Task RemoveBlob(string imageName)
    {
        BlobContainerClient container = await GetBlobContainerClient();
        BlobClient blob = container.GetBlobClient(imageName);
        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    }

    private async Task<BlobContainerClient> GetBlobContainerClient()
    {
        try
        {
            string? connectionString = _config["StorageConnectionStrings"];
            BlobContainerClient container = new(connectionString, ContainerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
