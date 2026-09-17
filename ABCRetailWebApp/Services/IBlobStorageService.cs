namespace ABCRetailWebApp.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadBlobAsync(string containerName, string blobName, Stream content, string contentType);
        Task<Stream?> DownloadBlobAsync(string containerName, string blobName);
        Task DeleteBlobAsync(string containerName, string blobName);
        Task<IEnumerable<string>> ListBlobsAsync(string containerName);
        Task<bool> BlobExistsAsync(string containerName, string blobName);
    }
}
