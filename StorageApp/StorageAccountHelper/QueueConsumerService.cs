using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

namespace StorageAccountHelper;

public class QueueConsumerService
{
    public string QueueName { get; set; } = string.Empty;

    private readonly QueueClient? _queue;

    public QueueConsumerService(string connectionString)
    {
        _queue = new(connectionString, QueueName);
    }

    public async Task<List<string>> Test()
    {
        List<string> messages = new List<string>();

        if (_queue is null) return messages;

        QueueProperties properties = await _queue.GetPropertiesAsync();
        while (properties.ApproximateMessagesCount > 0)
        {
            string message = await RetrieveNextMessage();
            messages.Add(message);
        }

        return messages;
    }

    public async Task<List<string>> RetrieveMessages()
    {
        List<string> messages = new List<string>();

        if (_queue is null) return messages;

        if (await _queue.ExistsAsync())
        {
            QueueProperties properties = await _queue.GetPropertiesAsync();
            while (properties.ApproximateMessagesCount > 0)
            {
                string message = await RetrieveNextMessage();
                messages.Add(message);
            }
        }

        return messages;
    }

    private async Task<string> RetrieveNextMessage()
    {
        QueueMessage[] messageBase64 = await _queue!.ReceiveMessagesAsync(1);
        byte[] data = Convert.FromBase64String(messageBase64[0].Body.ToString());
        string message = Encoding.UTF8.GetString(data);

        await _queue.DeleteMessageAsync(messageBase64[0].MessageId, messageBase64[0].PopReceipt);

        return message;
    }
}
