using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Http;

namespace ABCRetailWebApp.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ShareServiceClient _shareServiceClient;
        private const string ShareName = "retail-documents";
        private const string DirectoryName = "uploads";

        public FileStorageService(string connectionString)
        {
            _shareServiceClient = new ShareServiceClient(connectionString);
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty", nameof(file));
            }

            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            await directoryClient.CreateIfNotExistsAsync();

            var fileClient = directoryClient.GetFileClient(file.FileName);
            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(stream.Length);
            await fileClient.UploadRangeAsync(new Azure.HttpRange(0, stream.Length), stream);
        }

        public async Task<Stream?> DownloadFileAsync(string fileName)
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            var fileClient = directoryClient.GetFileClient(fileName);

            if (!await fileClient.ExistsAsync())
            {
                return null;
            }

            var response = await fileClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }

        public async Task<IEnumerable<string>> ListFilesAsync()
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            var files = new List<string>();

            if (await directoryClient.ExistsAsync())
            {
                await foreach (var fileItem in directoryClient.GetFilesAndDirectoriesAsync())
                {
                    if (!fileItem.IsDirectory)
                    {
                        files.Add(fileItem.Name);
                    }
                }
            }

            return files;
        }

        public async Task<bool> FileExistsAsync(string fileName)
        {
            var shareClient = _shareServiceClient.GetShareClient(ShareName);
            var directoryClient = shareClient.GetDirectoryClient(DirectoryName);
            var fileClient = directoryClient.GetFileClient(fileName);
            return await fileClient.ExistsAsync();
        }
    }
}
