using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TourManagementApi.Models.Api.Rezdy;
using TourManagementApi.Services.Rezdy;

namespace TourManagementApi.Controllers
{
    /// <summary>
    /// Rezdy Webhook Controller
    /// </summary>
    [Route("api/webhooks/rezdy")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "rezdy")]
    public class RezdyWebhookController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public RezdyWebhookController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> Handle([FromBody] JObject payload)
        {
            var eventType = payload["event"]?.ToString();
            var data = payload["data"] as JObject;

            switch (eventType)
            {
                case "booking.created":
                    var created = data.ToObject<BookingDto>();
                    await _bookingService.OnBookingCreatedAsync(created);
                    break;

                case "booking.updated":
                    var updated = data.ToObject<BookingDto>();
                    await _bookingService.OnBookingUpdatedAsync(updated);
                    break;

                case "booking.cancelled":
                    var orderNo = data["orderNumber"]?.ToString();
                    await _bookingService.OnBookingCancelledAsync(orderNo);
                    break;
            }

            return Ok();
        }
    }
}
