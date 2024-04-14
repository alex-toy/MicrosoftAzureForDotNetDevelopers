using StorageAccountHelper;

Console.WriteLine("Queue consumer");


string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
Console.WriteLine(connectionString);
QueueConsumerService queueConsumerService = new(connectionString) { QueueName = "attendees" };

//List<string> messages = await queueConsumerService.RetrieveMessages();
List<string> messages = await queueConsumerService.Test();

HandleMessage(messages);

static void HandleMessage(List<string> messages)
{
    messages.ForEach(message => Console.WriteLine(message));
}