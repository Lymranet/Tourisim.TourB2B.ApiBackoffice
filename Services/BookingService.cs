using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TourManagementApi.Data;
using TourManagementApi.Extensions;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.Rezdy;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public class BookingService
    {
        private readonly TourManagementDbContext _context;
        private readonly ILogger<BookingService> _logger;
        public BookingService(TourManagementDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ConfirmBookingAsync(RezdyBookingRequest booking)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(booking?.OrderNumber))
                    return false;

                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.OrderNumber == booking.OrderNumber && b.Status == "PROCESSING");

                if (existingBooking == null)
                    return false;

                existingBooking.Status = booking.Status;
                existingBooking.DateConfirmed = DateTime.UtcNow;

                ObjectMapperExtensions.MapWithFallback(booking, existingBooking);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking confirmation failed.");
            }

            return false;
        }

        public async Task<RezdyBookingRequest> CreateReservationAsync(RezdyBookingRequest booking, string externalProductCode)
        {
            try
            {
                var newBooking = new Booking
                {
                    ApiKey = booking.ApiKey,
                    Comments = booking.Comments,
                    Commission = booking.Commission,
                    Coupon = booking.Coupon,
                    Status = booking.Status,
                    Code = booking.Code,
                    DateConfirmed = ParseDate(booking.DateConfirmed),
                    DateCreated = ParseDate(booking.DateCreated),
                    DatePaid = ParseDate(booking.DatePaid),
                    DateReconciled = ParseDate(booking.DateReconciled),
                    DateUpdated = ParseDate(booking.DateUpdated),
                    InternalNotes = booking.InternalNotes,
                    OrderNumber = booking.OrderNumber,
                    PaymentOption = booking.PaymentOption,
                    ResellerAlias = booking.ResellerAlias,
                    ResellerComments = booking.ResellerComments,
                    ResellerId = booking.ResellerId,
                    ResellerName = booking.ResellerName,
                    ResellerReference = booking.ResellerReference,
                    ResellerSource = booking.ResellerSource,
                    SendNotifications = booking.SendNotifications,
                    Source = booking.Source,
                    SourceChannel = booking.SourceChannel,
                    SourceReferrer = booking.SourceReferrer,
                    SupplierAlias = booking.SupplierAlias,
                    SupplierId = booking.SupplierId,
                    SupplierName = booking.SupplierName,
                    Surcharge = booking.Surcharge,
                    TotalAmount = booking.TotalAmount,
                    TotalCurrency = booking.TotalCurrency,
                    TotalDue = booking.TotalDue,
                    TotalPaid = booking.TotalPaid,
                    BarcodeType = booking.BarcodeType
                };

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                // CreatedBy
                if (booking.CreatedBy is not null)
                {
                    _context.BookingCreatedBies.Add(new BookingCreatedBy
                    {
                        BookingId = newBooking.BookingId,
                        Code = booking.CreatedBy.Code,
                        Email = booking.CreatedBy.Email,
                        FirstName = booking.CreatedBy.FirstName,
                        LastName = booking.CreatedBy.LastName
                    });
                }

                // Customer
                if (booking.Customer is not null)
                {
                    _context.BookingCustomers.Add(new BookingCustomer
                    {
                        BookingId = newBooking.BookingId,
                        AboutUs = booking.Customer.AboutUs,
                        AddressLine = booking.Customer.AddressLine,
                        AddressLine2 = booking.Customer.AddressLine2,
                        City = booking.Customer.City,
                        CompanyName = booking.Customer.CompanyName,
                        CountryCode = booking.Customer.CountryCode,
                        Dob = booking.Customer.Dob,
                        Email = booking.Customer.Email,
                        Fax = booking.Customer.Fax,
                        FirstName = booking.Customer.FirstName,
                        Gender = booking.Customer.Gender,
                        CustomerExternalId = booking.Customer.Id,
                        LastName = booking.Customer.LastName,
                        Marketing = booking.Customer.Marketing,
                        MiddleName = booking.Customer.MiddleName,
                        Mobile = booking.Customer.Mobile,
                        Name = booking.Customer.Name,
                        Newsletter = booking.Customer.Newsletter,
                        Phone = booking.Customer.Phone,
                        PostCode = booking.Customer.PostCode,
                        PreferredLanguage = booking.Customer.PreferredLanguage,
                        Skype = booking.Customer.Skype,
                        State = booking.Customer.State,
                        Title = booking.Customer.Title
                    });
                }

                // Credit Card
                if (booking.CreditCard is not null)
                {
                    _context.BookingCreditCards.Add(new BookingCreditCard
                    {
                        BookingId = newBooking.BookingId,
                        CardToken = booking.CreditCard.CardToken,
                        CardType = booking.CreditCard.CardType,
                        ExpiryMonth = booking.CreditCard.ExpiryMonth,
                        ExpiryYear = booking.CreditCard.ExpiryYear,
                        CardName = booking.CreditCard.CardName,
                        CardNumber = booking.CreditCard.CardNumber,
                        CardSecurityNumber = booking.CreditCard.CardSecurityNumber
                    });
                }

                // Fields
                if (booking.Fields is not null)
                {
                    foreach (var field in booking.Fields)
                    {
                        _context.BookingFields.Add(new BookingField
                        {
                            BookingId = newBooking.BookingId,
                            Label = field.Label,
                            Value = field.Value,
                            BarcodeType = field.BarcodeType
                        });
                    }
                }

                // Payments
                if (booking.Payments is not null)
                {
                    foreach (var payment in booking.Payments)
                    {
                        _context.BookingPayments.Add(new BookingPayment
                        {
                            BookingId = newBooking.BookingId,
                            Amount = payment.Amount,
                            Currency = payment.Currency,
                            Date = payment.Date,
                            Label = payment.Label,
                            Recipient = payment.Recipient,
                            Type = payment.Type
                        });
                    }
                }

                // ResellerUser
                if (booking.ResellerUser is not null)
                {
                    _context.BookingResellerUsers.Add(new BookingResellerUser
                    {
                        BookingId = newBooking.BookingId,
                        Code = booking.ResellerUser.Code,
                        Email = booking.ResellerUser.Email,
                        FirstName = booking.ResellerUser.FirstName,
                        LastName = booking.ResellerUser.LastName
                    });
                }

                // Items & alt ilişkileri
                foreach (var item in booking.Items)
                {
                    var itemEntity = new BookingItem
                    {
                        BookingId = newBooking.BookingId,
                        ProductCode = item.ProductCode,
                        TotalQuantity = item.TotalQuantity,
                        ProductName = item.ProductName,
                        Amount = item.Amount,
                        Subtotal = item.Subtotal,
                        TotalItemTax = item.TotalItemTax,
                        StartTime = item.StartTime,
                        StartTimeLocal = item.StartTimeLocal,
                        EndTime = item.EndTime,
                        EndTimeLocal = item.EndTimeLocal,
                        ExternalProductCode = item.ExternalProductCode,
                        TransferFrom = item.TransferFrom,
                        TransferTo = item.TransferTo,
                        TransferReturn = item.TransferReturn
                    };

                    _context.BookingItems.Add(itemEntity);
                    await _context.SaveChangesAsync();

                    if (item.Quantities is not null)
                    {
                        foreach (var q in item.Quantities)
                        {
                            _context.BookingQuantities.Add(new BookingQuantity
                            {
                                ItemId = itemEntity.ItemId,
                                OptionLabel = q.OptionLabel,
                                OptionPrice = q.OptionPrice,
                                OptionSeatsUsed = q.OptionSeatsUsed,
                                Value = q.Value
                            });
                        }
                    }

                    if (item.Extras is not null)
                    {
                        foreach (var x in item.Extras)
                        {
                            _context.BookingExtras.Add(new BookingExtra
                            {
                                ItemId = itemEntity.ItemId,
                                Name = x.Name,
                                Description = x.Description,
                                ExtraPriceType = x.ExtraPriceType,
                                Price = x.Price,
                                Quantity = x.Quantity
                            });
                        }
                    }

                    if (item.Vouchers is not null)
                    {
                        foreach (var v in item.Vouchers)
                        {
                            _context.BookingVouchers.Add(new BookingVoucher
                            {
                                ItemId = itemEntity.ItemId,
                                Code = v.Code,
                                ExpiryDate = v.ExpiryDate,
                                InternalNotes = v.InternalNotes,
                                InternalReference = v.InternalReference,
                                IssueDate = v.IssueDate,
                                Status = v.Status,
                                Value = v.Value,
                                ValueType = v.ValueType
                            });
                        }
                    }

                    if (item.Participants is not null)
                    {
                        foreach (var p in item.Participants)
                        {
                            var partEntity = new BookingParticipant
                            {
                                ItemId = itemEntity.ItemId
                            };

                            _context.BookingParticipants.Add(partEntity);
                            await _context.SaveChangesAsync();

                            foreach (var pf in p.Fields)
                            {
                                _context.BookingParticipantFields.Add(new BookingParticipantField
                                {
                                    ParticipantId = partEntity.ParticipantId,
                                    Label = pf.Label,
                                    Value = pf.Value,
                                    BarcodeType = pf.BarcodeType
                                });
                            }
                        }
                    }

                    if (item.PickupLocation is not null)
                    {
                        _context.BookingPickupLocations.Add(new BookingPickupLocation
                        {
                            ItemId = itemEntity.ItemId,
                            LocationName = item.PickupLocation.LocationName,
                            Address = item.PickupLocation.Address,
                            Latitude = item.PickupLocation.Latitude,
                            Longitude = item.PickupLocation.Longitude,
                            MinutesPrior = item.PickupLocation.MinutesPrior,
                            PickupInstructions = item.PickupLocation.PickupInstructions,
                            PickupTime = item.PickupLocation.PickupTime
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rezdy booking insert failed.");
            }

            return booking;
        }

        // Yardımcı fonksiyon:
        private static DateTime? ParseDate(string? input)
        {
            return DateTime.TryParse(input, out var dt) ? dt : null;
        }

        public async Task<bool> CancelReservationAsync(RezdyBookingRequest booking)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(booking?.OrderNumber))
                {
                    _logger.LogWarning("CancelReservationAsync: OrderNumber is null or empty.");
                    return false;
                }

                var existingBooking = await _context.Bookings
                     .FirstOrDefaultAsync(b => b.OrderNumber == booking.OrderNumber);

                if (existingBooking == null)
                {
                    _logger.LogWarning("CancelReservationAsync: Booking not found for OrderNumber = {OrderNumber}", booking.OrderNumber);
                    return false;
                }

                // Zaten CANCELLED durumundaysa tekrar işlem yapılmaz
                if (string.Equals(existingBooking.Status, "CANCELLED", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("CancelReservationAsync: Booking with OrderNumber = {OrderNumber} is already CANCELLED.", booking.OrderNumber);
                    return true;
                }

                existingBooking.Status = booking.Status ?? "CANCELLED";
                existingBooking.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("CancelReservationAsync: Booking with OrderNumber = {OrderNumber} successfully marked as CANCELLED.", booking.OrderNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CancelReservationAsync: Failed to cancel booking with OrderNumber = {OrderNumber}", booking?.OrderNumber);
                return false;
            }
        }


    }
}

