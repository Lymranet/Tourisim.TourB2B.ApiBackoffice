using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace TourManagementApi.Controllers.Api
{
    [Route("api/webhooks/experiencebank")]
    [ApiController]
    public class ExperienceBankWebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] JObject payload)
        {
            // Log payload for debugging
            Console.WriteLine(payload.ToString());

            var method = payload["method"]?.ToString();

            if (method == "AvailabilityUpdated")
            {
                var data = payload["params"];
                // TODO: update availability cache
            }
            else if (method == "ActivityUpdated")
            {
                var data = payload["params"];
                // TODO: update activity data
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
