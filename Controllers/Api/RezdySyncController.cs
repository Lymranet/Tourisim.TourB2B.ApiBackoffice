using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Services;

namespace TourManagementApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "rezdy")]
    public class RezdySyncController : ControllerBase
    {
        private readonly RezdySyncService _syncService;

        public RezdySyncController(RezdySyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("sync-activities")]
        public async Task<IActionResult> SyncActivities()
        {
            await _syncService.SyncActivitiesToRezdy();
            return Ok("Aktiviteler Rezdy'e gönderildi.");
        }

        [HttpPost("trigger-update")]
        public async Task<IActionResult> TriggerUpdate([FromQuery] string rezdyCode, [FromQuery] string externalCode)
        {
            await _syncService.TriggerProductUpdateNotificationAsync(rezdyCode, externalCode);
            return Ok("Ürün güncelleme bildirimi başarıyla gönderildi.");
        }
        [HttpPost("trigger-availability-update")]
        public async Task<IActionResult> TriggerAvailabilityUpdate([FromQuery] string rezdyCode, [FromQuery] string externalCode)
        {
            await _syncService.TriggerAvailabilityUpdateNotificationAsync(rezdyCode, externalCode, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
            return Ok("Rezdy'ye availability update bildirimi gönderildi.");
        }
    }
}
