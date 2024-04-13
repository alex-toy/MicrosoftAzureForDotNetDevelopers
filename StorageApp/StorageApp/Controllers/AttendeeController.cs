using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageApp.Data;
using StorageApp.Services;

namespace StorageApp.Controllers
{
    public class AttendeeController : Controller
    {
        private readonly IAttendeeStorageService _attendeeStorageService;
        private readonly ITableStorageService<Attendee> _tableStorageService;

        public AttendeeController(IAttendeeStorageService attendeeStorageService, ITableStorageService<Attendee> tableStorageService)
        {
            _attendeeStorageService = attendeeStorageService;
            _tableStorageService = tableStorageService;
        }

        public async Task<ActionResult> Index()
        {
            var data = await _attendeeStorageService.GetAttendees();
            return View(data);
        }

        public async Task<ActionResult> Details(string id, string industry)
        {
            var data = await _attendeeStorageService.GetAttendee(industry, id);
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

                await _attendeeStorageService.UpsertAttendee(attendee);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        
        public async Task<ActionResult> Edit(string id, string industry)
        {
            var data = await _attendeeStorageService.GetAttendee(industry, id);
            return View(data);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Attendee attendee)
        {
            try
            {
                attendee.PartitionKey = attendee.Industry;

                await _attendeeStorageService.UpsertAttendee(attendee);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttendeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
