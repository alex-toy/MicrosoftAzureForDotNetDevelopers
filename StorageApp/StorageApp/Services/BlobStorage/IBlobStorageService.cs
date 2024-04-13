
namespace StorageApp.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        string ContainerName { get; set; }

        Task<string> GetBlobUrl(string imageName);
        Task RemoveBlob(string imageName);
        Task<string> UploadBlob(IFormFile formFile, string imageName);
    }
}