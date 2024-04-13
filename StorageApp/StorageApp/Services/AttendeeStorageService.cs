using Azure;
using Azure.Data.Tables;
using StorageApp.Data;

namespace StorageApp.Services
{
    public class AttendeeStorageService : IAttendeeStorageService
    {
        private const string TableName = "Attendees";
        private readonly IConfiguration _config;

        public AttendeeStorageService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task<Attendee> GetAttendee(string industry, string id)
        {
            var tableClient = await GetTableClient();
            return await tableClient.GetEntityAsync<Attendee>(industry, id);
        }

        public async Task<List<Attendee>> GetAttendees()
        {
            var tableClient = await GetTableClient();
            Pageable<Attendee> attendees = tableClient.Query<Attendee>();
            return attendees.ToList();
        }

        public async Task UpsertAttendee(Attendee attendee)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(attendee);
        }

        public async Task DeleteAttendee(string industry, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(industry, id);
        }

        private async Task<TableClient> GetTableClient()
        {
            var connectionString = _config["StorageConnectionStrings"];
            var serviceClient = new TableServiceClient(connectionString);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }
}
