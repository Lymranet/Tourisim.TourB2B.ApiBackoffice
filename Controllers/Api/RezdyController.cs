// Controllers/Rezdy/RezdyController.cs
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using TourManagementApi.Services.Rezdy;

namespace TourManagementApi.Controllers
{
    [Route("api/rezdy")]
    [ApiController]
    public class RezdyController : ControllerBase
    {
        private readonly RezdyService _rezdy;
        public RezdyController(RezdyService rezdy) => _rezdy = rezdy;

        [HttpPost("products")]
        public async Task<IActionResult> PublishProduct([FromBody] JObject dto)
        {
            var code = await _rezdy.CreateProductAsync(dto);
            return Ok(new { productCode = code });
        }

        [HttpPost("products/{code}/images")]
        public async Task<IActionResult> UploadImage(string code, IFormFile file)
        {
            await _rezdy.UploadProductImageAsync(code, file.OpenReadStream(), file.FileName);
            return Ok();
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> Book([FromBody] JObject dto)
        {
            var orderNo = await _rezdy.CreateBookingAsync(dto);
            return Ok(new { orderNumber = orderNo });
        }
    }

    // Webhook’ları ayrı controller’da işlemek isteyebilirsiniz:
    [Route("api/webhooks/rezdy")]
    public class RezdyWebhookController : ControllerBase
    {
        [HttpPost]
        public IActionResult Handle([FromBody] JObject payload)
        {
            // booking.created, booking.updated, booking.cancelled
            // aynı ExperienceBank örneğindeki gibi ayrıştırıp ilgili servise yönlendirin
            return Ok();
        }
    }
}
