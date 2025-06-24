using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Controllers.Api
{
    [ApiController]
    [Route("supplier/{partnerSupplierId}/booking/{bookingId}/cancellation")]
    public class BookingCancellationController : ControllerBase
    {
    //    private readonly ApplicationDbContext _context;

    //    public BookingCancellationController(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> CancelBooking(
    //        string partnerSupplierId,
    //        string bookingId,
    //        [FromBody] CancelBookingRequest request)
    //    {
    //        var reservation = await _context.Reservations
    //            .FirstOrDefaultAsync(r => r.ExperienceBankBookingId == bookingId);

    //        if (reservation == null)
    //        {
    //            return Ok(new
    //            {
    //                errors = new[]
    //                {
    //                new {
    //                    status = "404",
    //                    code = "BookingNotFound",
    //                    title = "Booking Not Found",
    //                    details = $"Booking #{bookingId} not found"
    //                }
    //            }
    //            });
    //        }

    //        if (!reservation.IsCancelled)
    //        {
    //            reservation.IsCancelled = true;
    //            reservation.CancelledAt = DateTime.UtcNow;
    //            reservation.CancelReason = request.Data.Reason;
    //            reservation.CancelNote = request.Data.Note;

    //            await _context.SaveChangesAsync();
    //        }

    //        return Ok(new
    //        {
    //            data = new
    //            {
    //                partnerBookingId = reservation.Id.ToString()
    //            }
    //        });
    //    }
    }
}
