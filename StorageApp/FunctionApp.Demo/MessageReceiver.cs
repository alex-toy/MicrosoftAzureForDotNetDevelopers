using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Demo
{
    public class MessageReceiver
    {
        private readonly ILogger _logger;

        public MessageReceiver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MessageReceiver>();    
        }

        [FunctionName("MessageReceiver")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [Queue("message-queue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> msg
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string body = await new StreamReader(req.Body).ReadToEndAsync();
            msg.Add(body);

            return new OkResult();
        }
    }
}
