using Azure.Data.Tables;
using Azure;
using StorageApp.Data;

namespace StorageApp.Services;

public class TableStorageService<T> : ITableStorageService<T> where T : Entity, ITableEntity
{
    private const string TableName = "Attendees";
    private readonly IConfiguration _config;

    public TableStorageService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public async Task<T> Get(string partitionKey, string id)
    {
        var tableClient = await GetTableClient();
        return await tableClient.GetEntityAsync<T>(partitionKey, id);
    }

    public async Task<List<T>> GetAll()
    {
        var tableClient = await GetTableClient();
        Pageable<T> entities = tableClient.Query<T>();
        return entities.ToList();
    }

    public async Task Upsert(T entity)
    {
        var tableClient = await GetTableClient();
        await tableClient.UpsertEntityAsync(entity);
    }

    public async Task Delete(string partitionKey, string id)
    {
        var tableClient = await GetTableClient();
        await tableClient.DeleteEntityAsync(partitionKey, id);
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
