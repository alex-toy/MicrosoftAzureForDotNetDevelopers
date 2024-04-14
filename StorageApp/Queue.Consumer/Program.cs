using StorageAccountHelper;

Console.WriteLine("Queue consumer");

static void HandleMessage(string message)
{
    Console.WriteLine(message);
}

string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
QueueConsumerService queueConsumerService = new(connectionString, "attendees") { HandleMessage = HandleMessage };

List<string> messages = await queueConsumerService.RetrieveMessages();
