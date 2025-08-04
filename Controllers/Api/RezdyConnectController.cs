using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;
using TourManagementApi.Services;
using TourManagementApi.Services.Rezdy;
using BookingDto = TourManagementApi.Models.Api.RezdyConnectModels.BookingDto;
using CancellationRequest = TourManagementApi.Models.Api.RezdyConnectModels.CancellationRequest;
using CustomerDto = TourManagementApi.Models.Api.RezdyConnectModels.CustomerDto;

namespace TourManagementApi.Controllers.Api
{
    [ApiController]
    [Route("api/rezdyconnect")]
    [ApiExplorerSettings(GroupName = "rezdy")]
    public class RezdyConnectController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly AvailabilityService _availabilityService;
        private readonly PricingService _pricingService;
        private readonly Services.BookingService _bookingService;
        private readonly string _validApiKey;

        public RezdyConnectController(
            IProductService productService,
            AvailabilityService availabilityService,
            PricingService pricingService,
             IConfiguration configuration,
            Services.BookingService bookingService)
        {
            _productService = productService;
            _availabilityService = availabilityService;
            _pricingService = pricingService;
            _bookingService = bookingService;
            _validApiKey = configuration["Rezdy:ApiKey"];
        }

        private bool IsApiKeyValid(string? apiKey) => apiKey == _validApiKey;
        public BookingDto MapRezdyToBookingDto(RezdyBookingDto rezdy)
        {
            var item = rezdy.Items.FirstOrDefault();

            return new BookingDto
            {
                OrderNumber = rezdy.OrderNumber,
                Status = rezdy.Status,
                ProductCode = item?.ProductCode,
                ExternalProductCode = item?.ExternalProductCode,
                StartTime = item?.StartTime ?? DateTime.MinValue,
                Currency = rezdy.TotalCurrency,
                TotalAmount = rezdy.TotalAmount,
                SupplierId = rezdy.SupplierId,
                Customer = new CustomerDto
                {
                    FullName = rezdy.Customer.Name,
                    Phone = rezdy.Customer.Mobile,
                    Email = rezdy.Customer.Email
                },
                Participants = item?.Participants != null
                    ? item.Participants.Select(p => new ParticipantDto
                    {
                        FirstName = p.Fields.FirstOrDefault(f => f.Label == "First Name")?.Value,
                        LastName = p.Fields.FirstOrDefault(f => f.Label == "Last Name")?.Value,
                        Email = p.Fields.FirstOrDefault(f => f.Label == "Email")?.Value,
                        Phone = p.Fields.FirstOrDefault(f => f.Label == "Mobile")?.Value,
                        Fields = p.Fields.Select(f => new BookingField
                        {
                            Label = f.Label,
                            Value = f.Value
                        }).ToList(),
                        TicketCategory = item.Quantities?.FirstOrDefault()?.OptionLabel, // varsayım: her katılımcı için aynı
                        CommissionType = "NETT" // Default olarak ayarlanıyor
                    }).ToList()
                    : new List<ParticipantDto>(),

                Quantities = item?.Quantities != null
                    ? item.Quantities.Select(q => new QuantityDto
                    {
                        OptionLabel = q.OptionLabel,
                        OptionPrice = q.OptionPrice,
                        Value = q.Value,
                        CommissionType = "NETT"
                    }).ToList()
                    : new List<QuantityDto>(),

                PickupLocation = item?.PickupLocation != null
                    ? new PickupLocationDto
                    {
                        LocationName = item.PickupLocation.LocationName,
                        Address = item.PickupLocation.Address,
                        PickupTime = item.PickupLocation.PickupTime
                    }
                    : null,

                Subtotal = item?.Subtotal ?? 0,
                TotalItemTax = item?.TotalItemTax ?? 0
            };
        }



        [HttpGet("products")]
        public IActionResult GetProducts(
    [FromQuery] string apiKey,
    [FromQuery] int offset = 0,
    [FromQuery] int limit = 100,
    [FromQuery(Name = "productCode")] List<string>? productCodes = null,
    [FromQuery(Name = "externalProductCode")] List<string>? externalProductCodes = null)
        {
            if (apiKey != _validApiKey)
                return Unauthorized("Invalid API Key");

            List<RezdyProductDto> allProducts = _productService.GetAll();
            var matchedProducts = new List<RezdyProductDto>();

            // externalProductCode ile filtre
            if (externalProductCodes != null && externalProductCodes.Any())
            {
                foreach (var code in externalProductCodes)
                {
                    var match = allProducts.FirstOrDefault(p => p.InternalCode == code);
                    if (match == null)
                        return UnprocessableEntity(new
                        {
                            requestStatus = new
                            {
                                success = false,
                                message = $"Product with externalProductCode '{code}' not found."
                            }
                        });

                    matchedProducts.Add(match);
                }
            }


            // Hiçbir filtre yoksa tüm ürünler
            if (!(productCodes?.Any() ?? false) && !(externalProductCodes?.Any() ?? false))
            {
                matchedProducts = allProducts;
            }

            // Pagination
            var paged = matchedProducts.Skip(offset).Take(limit).ToList();
            var hasMore = offset + limit < matchedProducts.Count;
            Response.Headers.Add("pagination-has-more", hasMore ? "true" : "false");

            return Ok(new
            {
                requestStatus = new { success = true },
                products = paged
            });
        }

        [HttpGet("products/{productCode}")]
        public IActionResult GetProductDetail(string productCode, [FromQuery] string apiKey)
        {
            if (!IsApiKeyValid(apiKey))
                return Unauthorized("Invalid API Key");

            var product = _productService.GetByCode(productCode);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("availability")]
        public IActionResult GetAvailability(
       [FromQuery] string apiKey,
       [FromQuery] string productCode,
       [FromQuery] string externalProductCode,
       [FromQuery] DateTime from,
       [FromQuery] DateTime to,
       [FromQuery] string fromLocal,
       [FromQuery] string toLocal)
        {
            if (apiKey != _validApiKey)
                return Unauthorized(new { requestStatus = new { success = false, message = "Invalid API Key" } });

            var sessions = _availabilityService.GetSessions(productCode, externalProductCode, from, to);

            return Ok(new
            {
                requestStatus = new { success = true },
                sessions
            });
        }

        [HttpPut("booking")]
        public async Task<IActionResult> ConfirmBooking([FromQuery] string apiKey, [FromBody] RezdyBookingDto request)
        {
            if (apiKey != _validApiKey)
                return Unauthorized("Invalid API Key");

            if (request.Status != "CONFIRMED")
                return BadRequest(new { message = "Status must be CONFIRMED for confirmation call." });

            var result = await _bookingService.ConfirmReservationAsync(request.OrderNumber);

            if (!result)
                return NotFound(new { success = false, message = "Reservation not found or already confirmed." });

            var item = request.Items.FirstOrDefault();

            return Ok(new
            {
                bookings = new[] {
            new {
                request.OrderNumber,
                Status = "CONFIRMED",
                BarcodeType = request.BarcodeType,
                Comments = request.Comments,
                Fields = request.Fields,
                Items = new[] {
                    new {
                        ProductCode = item?.ProductCode,
                        Participants = item?.Participants,
                        TotalQuantity = item?.TotalQuantity ?? 0
                    }
                }
            }
        },
                requestStatus = new { code = "200", message = "Reservation confirmed" }
            });
        }


        [HttpPut("cancellation")]
        public async Task<IActionResult> CancelBooking([FromQuery] string apiKey, [FromBody] RezdyBookingDto booking)
        {
            if (apiKey != _validApiKey)
                return Unauthorized("Invalid API Key");

            var status = "CANCELLED"; // Doğru değer

            var result = await _bookingService.CancelReservationAsync(booking.OrderNumber, status);

            if (!result)
                return NotFound(new { message = "Booking not found." });

            return Ok(new
            {
                requestStatus = new { code = "200", message = "Booking cancelled" }
            });
        }

        [HttpPost("reservation")]
        public async Task<IActionResult> CreateProcessingReservation(
       [FromQuery] string apiKey,
       [FromBody] RezdyBookingDto request)
        {
            if (apiKey != _validApiKey)
                return Unauthorized(new { requestStatus = new { success = false, message = "Invalid API Key" } });

            try
            {
                var mapped = MapRezdyToBookingDto(request);
                var result = await _bookingService.CreateProcessingReservationAsync(mapped);

                return Ok(new
                {
                    requestStatus = new { success = true },
                    bookings = new[] { result }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    requestStatus = new { success = false, message = ex.Message }
                });
            }
        }

        [HttpPost("confirmation")]
        public IActionResult ConfirmBooking([FromQuery] string apiKey, [FromBody] ConfirmationRequest request)
        {
            if (apiKey != _validApiKey)
                return Unauthorized("Invalid API Key");

            var result = _bookingService.ConfirmReservationAsync(request.ReservationId);
            return Ok(new { success = result });
        }

    }
}
