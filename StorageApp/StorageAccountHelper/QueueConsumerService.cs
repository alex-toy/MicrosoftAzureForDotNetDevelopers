using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

namespace StorageAccountHelper;

public class QueueConsumerService
{
    public MessageHandler? HandleMessage { get; set; }

    public delegate void MessageHandler(string message);

    private readonly QueueClient? _queue;

    public QueueConsumerService(string connectionString, string queueName)
    {
        _queue = new(connectionString, queueName);
    }

    public async Task<List<string>> RetrieveMessages()
    {
        List<string> messages = new();

        if (_queue is null) return messages;

        if (await _queue.ExistsAsync())
        {
            QueueProperties properties = await _queue.GetPropertiesAsync();
            for(int i = 0; i < properties.ApproximateMessagesCount; i++)
            {
                string message = await RetrieveNextMessageAsync();
                if (HandleMessage is not null) HandleMessage(message);
                messages.Add(message);
            }
        }

        return messages;
    }

    public async Task<BinaryData> PeekMessagesAsync()
    {
        if (_queue is null) return new BinaryData(new byte());
        Azure.Response<PeekedMessage[]> message = await _queue.PeekMessagesAsync();
        if (message.Value.Length > 0) return message.Value[0].Body;
        return new BinaryData(new byte());
    }

    private async Task<string> RetrieveNextMessageAsync()
    {
        QueueMessage[] messageBase64 = await _queue!.ReceiveMessagesAsync(1);
        byte[] data = Convert.FromBase64String(messageBase64[0].Body.ToString());
        string message = Encoding.UTF8.GetString(data);

        await _queue.DeleteMessageAsync(messageBase64[0].MessageId, messageBase64[0].PopReceipt);

        return message;
    }
}
