using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ABCRetailFunctions
{
    public class BlobStorageFunction
    {
        private readonly ILogger<BlobStorageFunction> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageFunction(ILogger<BlobStorageFunction> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        [Function("BlobStorage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "blob/{containerName}/{blobName}")] HttpRequestData req,
            string containerName,
            string blobName)
        {
            _logger.LogInformation($"Blob Storage function triggered for container: {containerName}, blob: {blobName}");

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(blobName);
                
                var contentType = req.Headers.Contains("Content-Type") 
                    ? req.Headers.GetValues("Content-Type").FirstOrDefault() 
                    : "application/octet-stream";

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blobClient.UploadAsync(req.Body, blobHttpHeaders);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Successfully uploaded blob {blobName} to container {containerName}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading blob: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
