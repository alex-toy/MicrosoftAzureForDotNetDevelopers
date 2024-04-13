using Microsoft.AspNetCore.Mvc;
using StorageApp.Data;
using StorageApp.Services.TableStorage;

namespace StorageApp.Controllers
{
    public class AttendeeController : Controller
    {
        private readonly ITableStorageService<Attendee> _tableStorageService;

        public AttendeeController(ITableStorageService<Attendee> tableStorageService)
        {
            _tableStorageService = tableStorageService;
            _tableStorageService.TableName = "Attendees";
        }

        public async Task<ActionResult> Index()
        {
            List<Attendee> data = await _tableStorageService.GetAll();
            return View(data);
        }

        public async Task<ActionResult> Details(string industry, string id)
        {
            Attendee data = await _tableStorageService.Get(industry, id);
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Attendee attendee)
        {
            try
            {
                attendee.PartitionKey = attendee.Industry;
                attendee.RowKey = Guid.NewGuid().ToString();

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
        public async Task<ActionResult> Edit(int id, Attendee attendee)
        {
            try
            {
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
                await _tableStorageService.Delete(industry, id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
