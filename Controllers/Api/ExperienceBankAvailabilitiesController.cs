using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Controllers.Api
{
    [Route("supplier/12004/availabilities")]
    [ApiController]
    public class ExperienceBankAvailabilitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExperienceBankAvailabilitiesController> _logger;

        public ExperienceBankAvailabilitiesController(ApplicationDbContext context, ILogger<ExperienceBankAvailabilitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailabilities(
                string partnerSupplierId,
                [FromQuery] DateTime dateRangeStart,
                [FromQuery] DateTime dateRangeEnd,
                [FromQuery] string[] activityId,
                [FromQuery] string[] optionId,
                [FromQuery] int offset = 0)
            {
            _logger.LogInformation("GetAvailabilities called for PartnerSupplierId: {PartnerSupplierId}, DateRange: {Start} - {End}, Offset: {Offset}", partnerSupplierId, dateRangeStart, dateRangeEnd, offset);

            const int pageSize = 50;
            var dateOnlyRangeStart = DateOnly.FromDateTime(dateRangeStart);
            var dateOnlyRangeEnd = DateOnly.FromDateTime(dateRangeEnd);

            var query = _context.Availabilities
                .Include(a => a.Option)
                    .ThenInclude(o => o.OpeningHours)
                .Include(a => a.TicketCategoryCapacities)
                .Where(a => a.PartnerSupplierId == partnerSupplierId &&
                            a.Date >= dateOnlyRangeStart &&
                            a.Date <= dateOnlyRangeEnd);

            if (activityId?.Any() == true)
                query = query.Where(a => activityId.Contains(a.ActivityId.ToString()));

            if (optionId?.Any() == true)
                query = query.Where(a => optionId.Contains(a.OptionId.ToString()));

            var total = await query.CountAsync();
            _logger.LogInformation("Found total {Total} availabilities.", total);

            var availabilities = await query
                .OrderBy(a => a.Date)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();

            var result = new AvailabilityResultDto
            {
                links = new LinksDto
                {
                    next = offset + pageSize < total
                        ? $"https://tour.hotelwidget.com/supplier/12004/availabilities?offset={offset + pageSize}&dateRangeStart={dateRangeStart:yyyy-MM-dd}&dateRangeEnd={dateRangeEnd:yyyy-MM-dd}"
                        : null
                },
                data = availabilities.Select(a => new AvailabilityResponseDto
                {
                    dateTime = a.StartTime.HasValue
                        ? a.StartTime.Value.ToString("o")
                        : a.Date.ToDateTime(TimeOnly.MinValue).ToString("o"),

                    openingHours = a.Option?.OpeningHours.Select(oh => new OpeningHourDto
                    {
                        fromTime = oh.FromTime,
                        toTime = oh.ToTime
                    }).ToList() ?? new List<OpeningHourDto>(),

                    availableCapacity = a.AvailableCapacity,
                    maximumCapacity = a.MaximumCapacity,
                    activityId = a.ActivityId.ToString(),
                    optionId = a.OptionId.ToString(),

                    ticketCategories = a.TicketCategoryCapacities.Select(tc => new TicketCategoryDto
                    {
                        id = tc.TicketCategoryId.ToString(),
                        availableCapacity = tc.Capacity
                    }).ToList()
                }).ToList()
            };

            _logger.LogInformation("Returning {Count} availabilities in response.", availabilities.Count);
            return Ok(result);
        }
    }
}
