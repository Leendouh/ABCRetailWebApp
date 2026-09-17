using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Queues;

namespace ABCRetailFunctions
{
    public class QueueStorageFunction
    {
        private readonly ILogger<QueueStorageFunction> _logger;
        private readonly QueueServiceClient _queueServiceClient;

        public QueueStorageFunction(ILogger<QueueStorageFunction> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            _queueServiceClient = new QueueServiceClient(connectionString);
        }

        [Function("QueueStorage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "queue/{queueName}")] HttpRequestData req,
            string queueName)
        {
            _logger.LogInformation($"Queue Storage function triggered for queue: {queueName}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Message content is required");
                return badResponse;
            }

            try
            {
                var queueClient = _queueServiceClient.GetQueueClient(queueName);
                await queueClient.CreateIfNotExistsAsync();

                await queueClient.SendMessageAsync(requestBody);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Successfully sent message to queue {queueName}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message to queue: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
