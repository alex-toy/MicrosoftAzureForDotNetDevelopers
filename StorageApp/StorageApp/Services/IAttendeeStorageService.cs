using StorageApp.Data;

namespace StorageApp.Services
{
    public interface IAttendeeStorageService
    {
        Task DeleteAttendee(string industry, string id);
        Task<Attendee> GetAttendee(string industry, string id);
        Task<List<Attendee>> GetAttendees();
        Task UpsertAttendee(Attendee attendee);
    }
}