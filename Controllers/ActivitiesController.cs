using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models;
using Newtonsoft.Json;

namespace TourManagementApi.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // MVC Actions
        public async Task<IActionResult> Index()
        {
            try
            {
                var activities = await _context.Activities
                    .Include(a => a.Location)
                    .ToListAsync();

                return View(activities);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching activities: {ex.Message}");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Activity());
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.Location)
                .Include(a => a.TimeSlots)
                .Include(a => a.MeetingPoints)
                .Include(a => a.GuestFields)
                .Include(a => a.Options)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            Activity? activity = null;

            // 1. JSON ile mi geldi?
            if (Request.ContentType != null && Request.ContentType.Contains("application/json"))
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    activity = JsonConvert.DeserializeObject<Activity>(body);
                }
            }
            else
            {
                // 2. Klasik form submit ile mi geldi?
                activity = new Activity();
                await TryUpdateModelAsync(activity);
            }

            if (activity == null)
                return BadRequest("Veri alınamadı.");

            // Eğer timeSlots boşsa, default bir zaman dilimi ekle (isteğe bağlı)
            if (activity.TimeSlots == null || activity.TimeSlots.Count == 0)
            {
                activity.TimeSlots = new List<TourManagementApi.Models.Common.TimeSlot>
                {
                    new TourManagementApi.Models.Common.TimeSlot
                    {
                        StartTime = "09:00",
                        EndTime = "17:00",
                        DaysOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" }
                    }
                };
            }

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            // JSON istekse JSON dön, klasik submit ise redirect
            if (Request.ContentType != null && Request.ContentType.Contains("application/json"))
                return Json(new { success = true, id = activity.Id });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound();
            activity.Status = status;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            return await _context.Activities.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
            {
                return NotFound();
            }

            return activity;
        }

        [HttpPost]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(int id, Activity activity)
        {
            if (id != activity.Id)
            {
                return BadRequest();
            }

            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
} 