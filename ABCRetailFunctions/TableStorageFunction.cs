using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Text.Json;

namespace ABCRetailFunctions
{
    public class TableStorageFunction
    {
        private readonly ILogger<TableStorageFunction> _logger;
        private readonly TableServiceClient _tableServiceClient;

        public TableStorageFunction(ILogger<TableStorageFunction> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
            _tableServiceClient = new TableServiceClient(connectionString);
        }

        [Function("TableStorage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "table/{tableName}")] HttpRequestData req,
            string tableName)
        {
            _logger.LogInformation($"Table Storage function triggered for table: {tableName}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);

            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request body");
                return badResponse;
            }

            try
            {
                var tableClient = _tableServiceClient.GetTableClient(tableName);
                await tableClient.CreateIfNotExistsAsync();

                // Create TableEntity with partition key and row key
                var partitionKey = data.ContainsKey("PartitionKey") ? data["PartitionKey"].ToString() : Guid.NewGuid().ToString();
                var rowKey = data.ContainsKey("RowKey") ? data["RowKey"].ToString() : partitionKey;
                
                var entity = new TableEntity(partitionKey, rowKey);
                
                // Add all other properties
                foreach (var kvp in data)
                {
                    if (kvp.Key != "PartitionKey" && kvp.Key != "RowKey")
                    {
                        entity[kvp.Key] = kvp.Value;
                    }
                }
                
                await tableClient.AddEntityAsync(entity);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Successfully added entity to table {tableName}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding entity to table: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
