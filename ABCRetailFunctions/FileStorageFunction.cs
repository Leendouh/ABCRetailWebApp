using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Files.Shares;

namespace ABCRetailFunctions
{
    public class FileStorageFunction
    {
        private readonly ILogger<FileStorageFunction> _logger;
        private readonly ShareServiceClient _shareServiceClient;

        public FileStorageFunction(ILogger<FileStorageFunction> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            _shareServiceClient = new ShareServiceClient(connectionString);
        }

        [Function("FileStorage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "file/{shareName}/{directoryName}/{fileName}")] HttpRequestData req,
            string shareName,
            string directoryName,
            string fileName)
        {
            _logger.LogInformation($"File Storage function triggered for share: {shareName}, directory: {directoryName}, file: {fileName}");

            try
            {
                var shareClient = _shareServiceClient.GetShareClient(shareName);
                await shareClient.CreateIfNotExistsAsync();

                var directoryClient = shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                var fileClient = directoryClient.GetFileClient(fileName);
                await fileClient.CreateAsync(req.Body.Length);
                await fileClient.UploadRangeAsync(new Azure.HttpRange(0, req.Body.Length), req.Body);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Successfully uploaded file {fileName} to share {shareName}/{directoryName}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
