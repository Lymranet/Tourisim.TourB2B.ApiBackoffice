using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesApiController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityExportDto>>> GetActivities()
        {
            var activities = await _context.Activities
                .Include(a => a.Location)
                .Include(a => a.TimeSlots)
                .Include(a => a.Options)
                .Include(a => a.MeetingPoints)
                .Select(a => new ActivityExportDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Category = a.Category,
                    Duration = a.Duration,
                    CountryCode = a.CountryCode,
                    DestinationCode = a.DestinationCode,
                    DestinationName = a.DestinationName,
                    CoverImage = a.CoverImage,
                    TimeSlots = a.TimeSlots,
                    Price = a.Pricing?.AdultPrice ?? 0,
                    Currency = a.Pricing?.Currency
                })
                .ToListAsync();

            return Ok(activities);
        }

    }
}
