using Azure.Storage.Queues;
using Newtonsoft.Json;

namespace StorageAccountHelper;

public class QueueProducerService 
{
    public string QueueName { get; set; } = string.Empty;

    private readonly string? _connectionString;

    public QueueProducerService(string conecctionString)
    {
        _connectionString = conecctionString;
    }

    public async Task SendMessage<T>(T emailMessage)
    {
        QueueClient queueClient = await GetQueueClientAsync();

        string message = JsonConvert.SerializeObject(emailMessage);

        await queueClient.SendMessageAsync(message);
    }

    private async Task<QueueClient> GetQueueClientAsync()
    {
        QueueClientOptions options = new QueueClientOptions()
        {
            MessageEncoding = QueueMessageEncoding.Base64
        };
        QueueClient queueClient = new(_connectionString, QueueName, options);

        await queueClient.CreateIfNotExistsAsync();
        return queueClient;
    }
}
