using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models.Api.ExperinceBank;
using TourManagementApi.Services;

namespace TourManagementApi.Controllers.Api
{
    [ApiController]
    [Route("supplier/12004/booking/{bookingId}/cancellation")]
    [ApiExplorerSettings(GroupName = "experiencebank")]
    public class ExperiencebankBookingCancellationController : ControllerBase
    {
        private readonly TourManagementDbContext _context;
        private readonly ExperienceBankService _experienceBankService;
        private readonly ILogger<ExperiencebankBookingCancellationController> _logger;

        public ExperiencebankBookingCancellationController(
            TourManagementDbContext context,
            ExperienceBankService experienceBankService,
            ILogger<ExperiencebankBookingCancellationController> logger)
        {
            _context = context;
            _experienceBankService = experienceBankService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(
       string partnerSupplierId,
       string bookingId,
       [FromBody] CancelBookingRequest request)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);

            if (reservation == null)
            {
                return Ok(new
                {
                    errors = new[]
                    {
                new {
                    status = "404",
                    code = "BookingNotFound",
                    title = "Booking Not Found",
                    details = $"Booking #{bookingId} not found"
                }
            }
                });
            }

            if (!reservation.IsCancelled)
            {
                reservation.IsCancelled = true;
                reservation.CancelledAt = DateTime.UtcNow;
                reservation.CancelReason = request.Data.Reason;
                reservation.CancelNote = request.Data.Note;

                await _context.SaveChangesAsync();

                try
                {
                    await _experienceBankService.NotifyBookingCancelledAsync(reservation.BookingId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExperienceBank booking.cancelled notification failed.");
                }
            }

            return Ok(new
            {
                data = new
                {
                    partnerBookingId = reservation.Id.ToString()
                }
            });
        }


        [HttpPost("{ticketId}/redeem")]
        public async Task<IActionResult> RedeemTicket(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            ticket.Status = "Redeemed";
            await _context.SaveChangesAsync();

            try
            {
                await _experienceBankService.NotifyTicketAffectedAsync(ticket.ExperienceBankTicketId, "redeemed", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExperienceBank ticket.affected notification failed.");
            }

            return Ok(new { success = true });
        }

    }
}
