using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Controllers
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    public class ActivitiesApiController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities(string partnerSupplierId, [FromQuery] string[] id, [FromQuery] int offset = 0)
        {
            // Sadece belirli ID'lerle filtrelenmiş tur listesi
            var query = _context.Activities
                .Include(a => a.Options)
                .Include(a => a.MeetingPoints)
                .Include(a => a.RoutePoints)
                .Include(a => a.GuestFields)
                .Include(a => a.Pricing)
                .Include(a => a.SeasonalAvailability)
                .Where(a => a.PartnerSupplierId == partnerSupplierId);

            if (id?.Any() == true)
            {
                query = query.Where(a => id.Contains(a.Id.ToString()));
            }

            const int pageSize = 50;
            var total = await query.CountAsync();
            var activities = await query
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                links = new
                {
                    next = offset + pageSize < total
                        ? $"https://your-api.com/supplier/{partnerSupplierId}/activities?offset={offset + pageSize}"
                        : null
                },
                data = activities.Select(MapToExperienceBankDto)
            };

            return Ok(result);
        }
    }
}
