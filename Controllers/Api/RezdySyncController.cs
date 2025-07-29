using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Services;

namespace TourManagementApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
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
    }

}
