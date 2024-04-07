namespace StorageApp.Data
{
    public class Attendee : TableEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Industry { get; set; }
    }
}
