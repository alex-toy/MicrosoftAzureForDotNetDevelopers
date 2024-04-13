using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageApp.Data;
using StorageApp.Services.BlobStorage;
using StorageApp.Services.TableStorage;

namespace StorageApp.Controllers
{
    public class AttendeeController : Controller
    {
        private readonly ITableStorageService<Attendee> _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;

        public AttendeeController(ITableStorageService<Attendee> tableStorageService, IBlobStorageService blobStorageService)
        {
            _tableStorageService = tableStorageService;
            _tableStorageService.TableName = "Attendees";
            _blobStorageService = blobStorageService;
            _blobStorageService.ContainerName = "data";
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
                Attendee data = await _tableStorageService.Get(industry, id);
                await _tableStorageService.Delete(industry, id);
                await _blobStorageService.RemoveBlob(data.ImageName);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
