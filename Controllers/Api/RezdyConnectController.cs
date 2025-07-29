using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Models.Api.RezdyConnectModels;
using TourManagementApi.Services;
using TourManagementApi.Services.Rezdy;
using CancellationRequest = TourManagementApi.Models.Api.RezdyConnectModels.CancellationRequest;


namespace TourManagementApi.Controllers.Api
{


    [ApiController]
    [Route("api/rezdyconnect")]
    public class RezdyConnectController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly AvailabilityService _availabilityService;
        private readonly PricingService _pricingService;
        private readonly Services.BookingService _bookingService;

        public RezdyConnectController(
            IProductService productService,
            AvailabilityService availabilityService,
            PricingService pricingService,
            Services.BookingService bookingService)
        {
            _productService = productService;
            _availabilityService = availabilityService;
            _pricingService = pricingService;
            _bookingService = bookingService;
        }



        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            var products = _productService.GetAll(); // Replace with your actual service
            return Ok(products);
        }

        [HttpGet("products/{productCode}")]
        public IActionResult GetProductDetail(string productCode)
        {
            var product = _productService.GetByCode(productCode);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("availability")]
        public IActionResult GetAvailability([FromBody] AvailabilityRequest request)
        {
            var availability = _availabilityService.Get(request); // Replace accordingly
            return Ok(availability);
        }

        [HttpPost("pricing")]
        public IActionResult GetPricing([FromBody] AvailabilityRequest request)
        {
            var pricing = _pricingService.Get(request.ProductCode); // Replace accordingly
            return Ok(pricing);
        }

        [HttpPost("booking")]
        public IActionResult CreateBooking([FromBody] BookingRequest request)
        {
            var result = _bookingService.Create(request); // Replace accordingly
            return Ok(new { success = true, bookingId = result.BookingId });
        }

        [HttpPost("cancellation")]
        public IActionResult CancelBooking([FromBody] CancellationRequest request)
        {
            var result = _bookingService.Cancel(request.BookingReference, request.Reason);
            return Ok(new { success = result });
        }
    }

}
