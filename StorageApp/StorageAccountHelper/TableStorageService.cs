using Azure;
using Azure.Data.Tables;
using StorageApp.Data;

namespace StorageAccountHelper;

public class TableStorageService<T>  where T : Entity, ITableEntity
{
    public string TableName { get; set; } = string.Empty;

    private readonly string _connectionString;

    public TableStorageService(string connectionString)
    {
        _connectionString = connectionString;
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
        TableServiceClient serviceClient = new(_connectionString);
        TableClient tableClient = serviceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync();
        return tableClient;
    }
}
