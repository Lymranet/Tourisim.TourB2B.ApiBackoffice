//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TourManagementApi.Data;
//using TourManagementApi.Extensions;
//using TourManagementApi.Models.Api;

//namespace TourManagementApi.Controllers.Api
//{
//    [Route("supplier/{partnerSupplierId}/availabilities")]
//    [ApiController]
//    public class ExperienceBankAvailabilitiesController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public ExperienceBankAvailabilitiesController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAvailabilities(
//            string partnerSupplierId,
//            [FromQuery] DateTime dateRangeStart,
//            [FromQuery] DateTime dateRangeEnd,
//            [FromQuery] string[] activityId,
//            [FromQuery] string[] optionId,
//            [FromQuery] int offset = 0)
//        {
//            const int pageSize = 50;

//            var query = _context.Availabilities
//                .Include(a => a.OpeningHours)
//                .Include(a => a.TicketCategoryCapacities)
//                .Where(a => a.PartnerSupplierId == partnerSupplierId &&
//                            a.Date >= dateRangeStart &&
//                            a.Date <= dateRangeEnd);

//            if (activityId?.Any() == true)
//                query = query.Where(a => activityId.Contains(a.ActivityId));

//            if (optionId?.Any() == true)
//                query = query.Where(a => optionId.Contains(a.OptionId));

//            var total = await query.CountAsync();

//            var availabilities = await query
//                .OrderBy(a => a.Date)
//                .Skip(offset)
//                .Take(pageSize)
//                .ToListAsync();

//            var result = new AvailabilityResultDto
//            {
//                links = new LinksDto
//                {
//                    next = offset + pageSize < total
//                        ? $"https://tour.hotelwidget.com/supplier/{partnerSupplierId}/availabilities?offset={offset + pageSize}&dateRangeStart={dateRangeStart:yyyy-MM-dd}&dateRangeEnd={dateRangeEnd:yyyy-MM-dd}"
//                        : null
//                },
//                data = availabilities.Select(a => a.ToDto()).ToList()
//            };

//            return Ok(result);
//        }
//    }
//}
