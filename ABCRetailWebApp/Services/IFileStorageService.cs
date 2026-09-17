namespace ABCRetailWebApp.Services
{
    public interface IFileStorageService
    {
        Task UploadFileAsync(IFormFile file);
        Task<Stream?> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
        Task<IEnumerable<string>> ListFilesAsync();
        Task<bool> FileExistsAsync(string fileName);
    }
}
