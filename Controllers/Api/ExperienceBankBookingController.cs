using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using TourManagementApi.Data;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Controllers.Api
{
    [Route("supplier/{partnerSupplierId}/booking")]
    [ApiController]
    public class ExperienceBankBookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExperienceBankBookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(string partnerSupplierId, [FromBody] ExperienceBankBookingRequest request)
        {
            var bookingExists = await _context.Reservations
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.ExperienceBankBookingId == request.Data.BookingId);

            if (bookingExists != null)
            {
                // Booking zaten varsa: aynı yanıtı tekrar döndüriyorum
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
                ExperienceBankBookingId = request.Data.BookingId,
                ContactName = request.Data.Contact.FullName,
                ContactEmail = request.Data.Contact.Email,
                ContactPhone = request.Data.Contact.PhoneNumber,
                PartnerBookingId = Guid.NewGuid().ToString(), // Örnek ID
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
                        TicketCode = GenerateRandomCode(), // QR code ekleme Todo: buna bakcaz gibi
                        TicketCodeType = "QR_CODE"
                    });
                }
            }

            _context.Reservations.Add(newBooking);
            await _context.SaveChangesAsync();

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
