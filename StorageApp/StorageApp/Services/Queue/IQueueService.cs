namespace StorageApp.Services.Queue;

public interface IQueueService
{
    string QueueName { get; set; }

    Task SendMessage<T>(T emailMessage);
}