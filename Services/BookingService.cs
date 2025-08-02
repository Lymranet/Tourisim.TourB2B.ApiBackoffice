using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;
using BookingDto = TourManagementApi.Models.Api.RezdyConnectModels.BookingDto;

namespace TourManagementApi.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingService> _logger;
        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ConfirmReservationAsync(string orderNumber)
        {
            try
            {
                var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.PartnerBookingId == orderNumber && r.Status == "Processing");

                if (reservation == null)
                    return false;

                reservation.Status = "Confirmed";
                reservation.ReservationDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking failed.");
                _logger.LogError(ex.Message, "Detail.");
            }
            return false;
        }


        public async Task<BookingDto> CreateProcessingReservationAsync(BookingDto booking)
        {
            try
            {
                var activityId = int.Parse(booking.ExternalProductCode); // Doğrudan BookingDto içinden geliyor

                var reservation = new Reservation
                {
                    ActivityId = activityId,
                    OptionId = _context.Options.FirstOrDefault(x => x.ActivityId == activityId)?.Id ?? 0,
                    ReservationDate = DateTime.UtcNow,
                    ScheduledDate = booking.StartTime,
                    TotalAmount = booking.TotalAmount,
                    Currency = booking.Currency, // ✨ Corrected
                    GuestCount = booking.Participants.Count,
                    ContactName = booking.Customer.FullName,
                    ContactEmail = booking.Customer.Email,
                    ContactPhone = booking.Customer.Phone,
                    Status = "Processing",
                    BookingId = booking.OrderNumber,
                    PartnerBookingId = booking.OrderNumber,
                    PartnerSupplierId = booking.SupplierId.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    IsCancelled = false
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                foreach (var participant in booking.Participants)
                {
                    var guest = new ReservationGuest
                    {
                        ReservationId = reservation.Id,
                        FirstName = participant.FirstName,
                        LastName = participant.LastName,
                        Email = participant.Email,
                        PhoneNumber = participant.Phone,
                        TicketCategory = participant.TicketCategory,
                        Occupancy = 1,
                        AdditionalFieldsJson = "{}",
                        AddonsJson = "[]",
                        GuestName = $"{participant.FirstName} {participant.LastName}",
                        Age = 0,
                        TicketId = Guid.NewGuid().ToString()
                    };

                    _context.ReservationGuests.Add(guest);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reservation notification failed.");
                _logger.LogError(ex.Message, "Detail.");
            }
            return booking;
        }

        public BookingCreateResponse Create(BookingRequest request)
        {
            var reservation = new Reservation
            {
                ActivityId = int.Parse(request.ProductCode),
                OptionId = request.OptionId,
                ReservationDate = DateTime.UtcNow,
                ScheduledDate = request.ScheduledDate,
                GuestCount = request.GuestCount,
                TotalAmount = request.TotalAmount,
                Currency = request.Currency,
                ContactName = request.ContactName,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                BookingId = Guid.NewGuid().ToString(),
                PartnerBookingId = request.BookingReference,
                PartnerSupplierId = request.SupplierId,
                Notes = request.Notes ?? "",
                IsCancelled = false
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return new BookingCreateResponse
            {
                BookingId = reservation.Id,
                BookingReference = reservation.PartnerBookingId,
                Status = reservation.Status
            };
        }

        public async Task<bool> CancelReservationAsync(string orderNumber, string status = "CANCELLED")
        {
            try
            {
                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.PartnerBookingId == orderNumber);

                if (reservation == null)
                    return false;

                reservation.Status = status;
                reservation.IsCancelled = true;
                reservation.CancelledAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cancel reservation failed for {OrderNumber}", orderNumber);
                return false;
            }
        }


    }
}

