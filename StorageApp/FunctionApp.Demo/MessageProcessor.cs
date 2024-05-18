using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Demo
{
    public class MessageProcessor
    {
        private readonly ILogger _logger;

        public MessageProcessor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MessageProcessor>();
        }

        [Function("MessageProcessor")]
        public void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
