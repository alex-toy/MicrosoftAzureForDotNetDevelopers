using Azure.Data.Tables;
using StorageApp.Data;

namespace StorageApp.Services.TableStorage
{
    public interface ITableStorageService<T> where T : Entity, ITableEntity
    {
        string TableName { get; set; }

        Task Delete(string partitionKey, string id);
        Task<T> Get(string partitionKey, string id);
        Task<List<T>> GetAll();
        Task Upsert(T entity);
    }
}