using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Models.Api.RezdyConnectModels;
using TourManagementApi.Services;
using TourManagementApi.Services.Rezdy;
using CancellationRequest = TourManagementApi.Models.Api.RezdyConnectModels.CancellationRequest;

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

        //[HttpGet("products")]
        //public IActionResult GetProducts([FromQuery] string apiKey)
        //{
        //    if (!IsApiKeyValid(apiKey))
        //        return Unauthorized("Invalid API Key");

        //    var products = _productService.GetAll();
        //    return Ok(products);
        //}

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

            var allProducts = _productService.GetAll(); // iç sistemdeki ürünler

            // Filtreleme: Belirli ürün istenmişse
            if ((productCodes != null && productCodes.Any()) || (externalProductCodes != null && externalProductCodes.Any()))
            {
                allProducts = allProducts
                    .Where(p => (productCodes != null && productCodes.Contains(p.ProductCode)) ||
                                (externalProductCodes != null && externalProductCodes.Contains(p.ExternalCode)))
                    .ToList();

                if (allProducts.Count != ((productCodes?.Count ?? 0) + (externalProductCodes?.Count ?? 0)))
                    return UnprocessableEntity(new { requestStatus = new { success = false, message = "One or more products not found." } });
            }

            // Pagination
            var pagedProducts = allProducts.Skip(offset).Take(limit).ToList();
            var hasMore = offset + limit < allProducts.Count;

            Response.Headers.Add("pagination-has-more", hasMore ? "true" : "false");

            return Ok(new
            {
                requestStatus = new { success = true },
                products = pagedProducts
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
                sessions = sessions
            });
        }


        //[HttpPost("pricing")]
        //public IActionResult GetPricing([FromQuery] string apiKey, [FromBody] AvailabilityRequest request)
        //{
        //    if (!IsApiKeyValid(apiKey))
        //        return Unauthorized("Invalid API Key");

        //    var pricing = _pricingService.Get(request.ProductCode);
        //    return Ok(pricing);
        //}

        [HttpPut("booking")]
        public async Task<IActionResult> ConfirmBooking([FromQuery] string apiKey, [FromBody] BookingDto request)
        {
            if (apiKey != _validApiKey)
                return Unauthorized("Invalid API Key");

            var result = await _bookingService.ConfirmReservationAsync(request.OrderNumber);

            if (!result)
                return NotFound(new { success = false, message = "Reservation not found or already confirmed." });

            return Ok(new
            {
                bookings = new[]
                {
            new {
                request.OrderNumber,
                Status = "CONFIRMED"
            }
        },
                requestStatus = new { code = "200", message = "Reservation confirmed" }
            });
        }


        [HttpPut("cancellation")]
        public async Task<IActionResult> CancelBooking([FromQuery] string apiKey, [FromBody] BookingDto booking)
        {
            if (apiKey != _validApiKey)
                return Unauthorized("Invalid API Key");

            var status = booking.Status?.ToUpper() ?? "CANCELLED";

            if (status != "CANCELLED" && status != "ABANDONED_CART")
                return BadRequest(new { message = "Invalid status for cancellation." });

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
    [FromBody] BookingDto request)
        {
            if (apiKey != _validApiKey)
                return Unauthorized(new { requestStatus = new { success = false, message = "Invalid API Key" } });

            try
            {
                var result = await _bookingService.CreateProcessingReservationAsync(request);

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
