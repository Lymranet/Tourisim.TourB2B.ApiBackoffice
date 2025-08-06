using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models;
using TourManagementApi.Models.Api.ExperinceBank;

namespace TourManagementApi.Controllers.Api
{
    [Route("supplier/12004/booking")]
    [ApiExplorerSettings(GroupName = "experiencebank")]
    [ApiController]
    public class ExperienceBankBookingController : ControllerBase
    {
        private readonly TourManagementDbContext _context;
        private readonly ILogger<ExperienceBankBookingController> _logger;

        public ExperienceBankBookingController(TourManagementDbContext context, ILogger<ExperienceBankBookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(string partnerSupplierId, [FromBody] ExperienceBankBookingRequest request)
        {
            _logger.LogInformation("CreateBooking called for PartnerSupplierId: {PartnerSupplierId}, BookingId: {BookingId}", partnerSupplierId, request.Data.BookingId);

            var bookingExists = await _context.Reservations
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.BookingId == request.Data.BookingId);

            if (bookingExists != null)
            {
                _logger.LogWarning("Booking already exists for BookingId: {BookingId}", request.Data.BookingId);

                return Ok(new
                {
                    data = new
                    {
                        partnerBookingId = bookingExists.PartnerBookingId,
                        tickets = bookingExists.Tickets.Select(t => new
                        {
                            ticketId = t.ExperienceBankTicketId,
                            partnerTicketId = t.InternalTicketId,
                            partnerTicketCode = t.TicketCode,
                            partnerTicketCodeType = t.TicketCodeType
                        })
                    }
                });
            }

            // Yeni rezervasyon oluştur
            var newBooking = new Reservation
            {
                PartnerSupplierId = partnerSupplierId,
                BookingId = request.Data.BookingId,
                ContactName = request.Data.Contact.FullName,
                ContactEmail = request.Data.Contact.Email,
                ContactPhone = request.Data.Contact.PhoneNumber,
                PartnerBookingId = Guid.NewGuid().ToString(),
                Tickets = new List<Ticket>()
            };

            foreach (var item in request.Data.BookingItems)
            {
                foreach (var guest in item.Guests)
                {
                    newBooking.Tickets.Add(new Ticket
                    {
                        ExperienceBankTicketId = guest.TicketId,
                        InternalTicketId = $"GUE-{Guid.NewGuid():N}".ToUpper().Substring(0, 24),
                        TicketCode = GenerateRandomCode(),
                        TicketCodeType = "QR_CODE"
                    });
                }
            }

            _context.Reservations.Add(newBooking);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New booking created successfully with PartnerBookingId: {PartnerBookingId}", newBooking.PartnerBookingId);

            return Ok(new
            {
                data = new
                {
                    partnerBookingId = newBooking.PartnerBookingId,
                    tickets = newBooking.Tickets.Select(t => new
                    {
                        ticketId = t.ExperienceBankTicketId,
                        partnerTicketId = t.InternalTicketId,
                        partnerTicketCode = t.TicketCode,
                        partnerTicketCodeType = t.TicketCodeType
                    })
                }
            });
        }

        private string GenerateRandomCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        }
    }
}
