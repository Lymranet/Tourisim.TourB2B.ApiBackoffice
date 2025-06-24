using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Data;

namespace TourManagementApi.Controllers.Api
{
    [Route("supplier/{partnerSupplierId}/availabilities")]
    [ApiController]
    public class ExperienceBankAvailabilitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExperienceBankAvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAvailabilities(
        //    string partnerSupplierId,
        //    [FromQuery] DateTime dateRangeStart,
        //    [FromQuery] DateTime dateRangeEnd,
        //    [FromQuery] string[] activityId,
        //    [FromQuery] string[] optionId,
        //    [FromQuery] int offset = 0)
        //{
        //    const int pageSize = 50;

        //    var query = _context.Availabilities
        //        .Where(a => a.PartnerSupplierId == partnerSupplierId &&
        //                    a.Date >= dateRangeStart &&
        //                    a.Date <= dateRangeEnd);

        //    if (activityId?.Any() == true)
        //        query = query.Where(a => activityId.Contains(a.ActivityId));

        //    if (optionId?.Any() == true)
        //        query = query.Where(a => optionId.Contains(a.OptionId));

        //    var total = await query.CountAsync();
        //    var availabilities = await query
        //        .OrderBy(a => a.Date)
        //        .Skip(offset)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var result = new
        //    {
        //        links = new
        //        {
        //            next = offset + pageSize < total
        //                ? $"https://your-api.com/supplier/{partnerSupplierId}/availabilities?offset={offset + pageSize}&dateRangeStart={dateRangeStart:yyyy-MM-dd}&dateRangeEnd={dateRangeEnd:yyyy-MM-dd}"
        //                : null
        //        },
        //        data = availabilities.Select(a => new
        //        {
        //            dateTime = a.StartTime.HasValue
        //                ? a.StartTime.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")
        //                : a.Date.ToString("yyyy-MM-ddT00:00:00zzz"),
        //            openingHours = a.OpeningHours?.Select(oh => new
        //            {
        //                fromTime = oh.FromTime,
        //                toTime = oh.ToTime
        //            }) ?? new List<object>(),
        //            availableCapacity = a.AvailableCapacity,
        //            maximumCapacity = a.MaximumCapacity,
        //            activityId = a.ActivityId,
        //            optionId = a.OptionId,
        //            ticketCategories = a.TicketCategoryCapacities?.Select(tc => new
        //            {
        //                id = tc.TicketCategoryId,
        //                availableCapacity = tc.Capacity
        //            }) ?? new List<object>()
        //        })
        //    };

        //    return Ok(result);
        //}
    }

}
