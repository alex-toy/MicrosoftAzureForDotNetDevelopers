using Azure.Storage.Queues;
using Newtonsoft.Json;

namespace StorageApp.Services.Queue;

public class QueueService : IQueueService
{
    public string QueueName { get; set; } = string.Empty;

    private readonly IConfiguration _config;
    private readonly string? _connectionString;

    public QueueService(IConfiguration config)
    {
        _config = config;
        _connectionString = _config["StorageConnectionString"];
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
