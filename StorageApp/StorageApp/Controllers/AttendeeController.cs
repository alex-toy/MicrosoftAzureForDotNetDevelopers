using Microsoft.AspNetCore.Mvc;
using StorageAccountHelper;
using StorageApp.Data;

namespace StorageApp.Controllers;

public class AttendeeController : Controller
{
    private readonly TableStorageService<Attendee> _tableStorageService;
    private readonly BlobStorageService _blobStorageService;
    private readonly QueueService _queueService;
    private readonly IConfiguration _config;

    public AttendeeController(IConfiguration config)
    {
        _config = config;
        string? connectionString = _config["StorageConnectionString"];

        _tableStorageService = new TableStorageService<Attendee>(connectionString) { TableName = "Attendees" };
        _blobStorageService = new BlobStorageService(connectionString) { ContainerName = "data" };
        _queueService = new QueueService(connectionString) { QueueName = "attendees" };
    }

    public async Task<ActionResult> Index()
    {
        List<Attendee> data = await _tableStorageService.GetAll();
        foreach (var item in data) item.ImageName = await _blobStorageService.GetBlobUrl(item.ImageName);
        return View(data);
    }

    public async Task<ActionResult> Details(string industry, string id)
    {
        Attendee data = await _tableStorageService.Get(industry, id);
        data.ImageName = await _blobStorageService.GetBlobUrl(data.ImageName);
        return View(data);
    }

    public ActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(Attendee attendee, IFormFile formFile)
    {
        try
        {
            string id = Guid.NewGuid().ToString();
            attendee.PartitionKey = attendee.Industry;
            attendee.RowKey = id;

            if (formFile.Length > 0)
            {
                attendee.ImageName = await _blobStorageService.UploadBlob(formFile, id);
            }
            else
            {
                attendee.ImageName = "default.png";
            }

            await _tableStorageService.Upsert(attendee);

            var message = $"Hello {attendee.Name} create";
            await SendMail(attendee.Email, message);

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    public async Task<ActionResult> Edit(string id, string industry)
    {
        Attendee data = await _tableStorageService.Get(industry, id);
        return View(data);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int id, Attendee attendee, IFormFile formFile)
    {
        try
        {
            if (formFile.Length > 0)
            {
                attendee.ImageName = await _blobStorageService.UploadBlob(formFile, attendee.RowKey);
            }

            attendee.PartitionKey = attendee.Industry;

            await _tableStorageService.Upsert(attendee);

            var message = $"Hello {attendee.Name} edit";
            await SendMail(attendee.Email, message);

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    public async Task<ActionResult> Delete(string id, string industry)
    {
        try
        {
            Attendee attendee = await _tableStorageService.Get(industry, id);
            await _tableStorageService.Delete(industry, id);
            await _blobStorageService.RemoveBlob(attendee.ImageName);

            var message = $"Hello {attendee.Name} edit";
            await SendMail(attendee.Email, message);

            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    private async Task SendMail(string mail, string message)
    {
        var email = new EmailMessage()
        {
            Address = mail,
            Message = message
        };
        await _queueService.SendMessage<EmailMessage>(email);
    }
}
