using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TourManagementApi.Data;

namespace TourManagementApi.Controllers.Api
{

    public class WebhookTicketCategoryDto
    {
        public string ticketCategoryId { get; set; }
        public int? availableCapacity { get; set; }
    }

    [Route("api/webhooks/experiencebank")]
    [ApiController]
    public class ExperienceBankWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExperienceBankWebhookController> _logger;

        public ExperienceBankWebhookController(ApplicationDbContext context, ILogger<ExperienceBankWebhookController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] JObject payload)
        {
            _logger.LogInformation("Webhook received: {Payload}", payload.ToString());

            var method = payload["method"]?.ToString();

            if (method == "AvailabilityUpdated")
            {
                _logger.LogInformation("Processing AvailabilityUpdated webhook.");
                var data = payload["params"];
                string activityId = data["activityId"]?.ToString();
                string optionId = data["optionId"]?.ToString();
                int availableCapacity = data["availableCapacity"]?.ToObject<int>() ?? 0;

                // Webhook ticketCategories
                var ticketCategories = data["ticketCategories"]?.ToObject<List<WebhookTicketCategoryDto>>();

                // DB'den availability bul
                var availability = await _context.Availabilities
                    .Include(a => a.TicketCategoryCapacities)
                    .FirstOrDefaultAsync(a =>
                        a.ActivityId.ToString() == activityId &&
                        a.OptionId.ToString() == optionId);

                if (availability != null)
                {
                    availability.AvailableCapacity = availableCapacity;

                    if (ticketCategories != null)
                    {
                        foreach (var tc in ticketCategories)
                        {
                            // ticketCategoryId string -> int dönüşüm
                            if (int.TryParse(tc.ticketCategoryId, out int tcId))
                            {
                                var existingTc = availability.TicketCategoryCapacities
                                    .FirstOrDefault(x => x.TicketCategoryId == tcId);

                                if (existingTc != null && tc.availableCapacity.HasValue)
                                {
                                    existingTc.Capacity = tc.availableCapacity.Value;
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            else if (method == "ActivityUpdated")
            {
                _logger.LogInformation("Processing ActivityUpdated webhook.");
                var data = payload["params"];
                string activityId = data["activityId"]?.ToString();

                var activity = await _context.Activities
                    .FirstOrDefaultAsync(a => a.Id.ToString() == activityId);

                if (activity != null)
                {
                    // Örnek: UpdatedAt field varsa güncelle
                    activity.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                _logger.LogWarning("Unhandled webhook method: {Method}", method);
            }

            return Ok(new
            {
                jsonrpc = "2.0",
                result = new { },
                id = payload["id"]
            });
        }

    }

}
