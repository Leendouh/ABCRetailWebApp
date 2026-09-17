using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABCRetailWebApp.Services
{
    public class QueueStorageService : IQueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;

        public QueueStorageService(string connectionString)
        {
            _queueServiceClient = new QueueServiceClient(connectionString);
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }

        public async Task<string?> ReceiveMessageAsync(string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            if (!await queueClient.ExistsAsync())
            {
                return null;
            }

            var response = await queueClient.ReceiveMessageAsync();
            if (response.Value == null)
            {
                return null;
            }

            await queueClient.DeleteMessageAsync(response.Value.MessageId, response.Value.PopReceipt);
            return response.Value.MessageText;
        }

        public async Task<IEnumerable<string>> PeekMessagesAsync(string queueName, int maxMessages = 10)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            if (!await queueClient.ExistsAsync())
            {
                return Enumerable.Empty<string>();
            }

            var messages = new List<string>();
            var response = await queueClient.PeekMessagesAsync(maxMessages: maxMessages);

            foreach (var message in response.Value)
            {
                messages.Add(message.MessageText);
            }

            return messages;
        }

        public async Task DeleteMessageAsync(string queueName, string messageId, string popReceipt)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.DeleteMessageAsync(messageId, popReceipt);
        }

        public async Task ClearQueueAsync(string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.ClearMessagesAsync();
        }
    }
}
