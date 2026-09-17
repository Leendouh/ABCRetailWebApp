namespace ABCRetailWebApp.Services
{
    public interface IQueueStorageService
    {
        Task SendMessageAsync(string queueName, string message);
        Task<string?> ReceiveMessageAsync(string queueName);
        Task<IEnumerable<string>> PeekMessagesAsync(string queueName, int maxMessages = 10);
        Task DeleteMessageAsync(string queueName, string messageId, string popReceipt);
        Task ClearQueueAsync(string queueName);
    }
}
