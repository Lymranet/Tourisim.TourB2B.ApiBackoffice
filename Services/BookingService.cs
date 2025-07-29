using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
        public class BookingService
        {
            private readonly ApplicationDbContext _context;

            public BookingService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task OnBookingCreatedAsync(BookingDto booking)
            {
                var activityId = int.Parse(booking.ProductCode); // Ürün kodu ID olarak varsayılıyor
                var reservation = new Reservation
                {
                    ActivityId = activityId,
                    OptionId = _context.Options.FirstOrDefault(x => x.ActivityId == activityId)?.Id ?? 0,
                    ReservationDate = DateTime.UtcNow,
                    ScheduledDate = booking.StartTime,
                    TotalAmount = booking.TotalAmount,
                    Currency = booking.Currency,
                    GuestCount = booking.Participants.Count,
                    ContactName = booking.Customer.FullName,
                    ContactEmail = booking.Customer.Email,
                    ContactPhone = booking.Customer.Phone,
                    Status = "Confirmed",
                    ExperienceBankBookingId = booking.OrderNumber,
                    PartnerBookingId = booking.OrderNumber,
                    PartnerSupplierId = booking.SupplierId,
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

            public async Task OnBookingUpdatedAsync(BookingDto booking)
            {
                // Basit örnek: İleride rezervasyon güncellemesi yapılabilir.
                await Task.CompletedTask;
            }

            public async Task OnBookingCancelledAsync(string orderNumber)
            {
                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.PartnerBookingId == orderNumber);

                if (reservation != null)
                {
                    reservation.IsCancelled = true;
                    reservation.CancelledAt = DateTime.UtcNow;
                    reservation.Status = "Cancelled";

                    await _context.SaveChangesAsync();
                }
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
                ExperienceBankBookingId = Guid.NewGuid().ToString(),
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

        public bool Cancel(string bookingReference, string reason)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.PartnerBookingId == bookingReference);

            if (reservation == null)
                return false;

            reservation.IsCancelled = true;
            reservation.CancelledAt = DateTime.UtcNow;
            reservation.CancelNote = reason;
            reservation.Status = "Cancelled";

            _context.SaveChanges();
            return true;
        }
    }
    }

