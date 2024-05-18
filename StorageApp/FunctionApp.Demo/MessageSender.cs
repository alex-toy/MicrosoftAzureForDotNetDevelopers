using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace FunctionApp.Demo
{
    public class MessageSender
    {
        private readonly ILogger _logger;

        public MessageSender(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MessageSender>();
        }

        [Function("MessageSender")]
        public void Run([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            string message = $"C# Timer trigger function executed at: {DateTime.Now}";

            HttpClient client = new HttpClient();
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "http://localhost:7008/api/MessageReceiver");
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
            client.Send(requestMessage);    

            _logger.LogInformation(message);
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
