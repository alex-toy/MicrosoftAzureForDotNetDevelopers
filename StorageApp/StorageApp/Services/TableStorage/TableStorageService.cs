using Azure.Data.Tables;
using Azure;
using StorageApp.Data;

namespace StorageApp.Services.TableStorage;

public class TableStorageService<T> : ITableStorageService<T> where T : Entity, ITableEntity
{
    public string TableName { get; set; } = string.Empty;

    private readonly IConfiguration _config;

    public TableStorageService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public async Task<T> Get(string partitionKey, string id)
    {
        TableClient tableClient = await GetTableClient();
        return await tableClient.GetEntityAsync<T>(partitionKey, id);
    }

    public async Task<List<T>> GetAll()
    {
        TableClient tableClient = await GetTableClient();
        Pageable<T> entities = tableClient.Query<T>();
        return entities.ToList();
    }

    public async Task Upsert(T entity)
    {
        TableClient tableClient = await GetTableClient();
        await tableClient.UpsertEntityAsync(entity);
    }

    public async Task Delete(string partitionKey, string id)
    {
        TableClient tableClient = await GetTableClient();
        await tableClient.DeleteEntityAsync(partitionKey, id);
    }

    private async Task<TableClient> GetTableClient()
    {
        string? connectionString = _config["StorageConnectionStrings"];
        TableServiceClient serviceClient = new(connectionString);
        TableClient tableClient = serviceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }
}
