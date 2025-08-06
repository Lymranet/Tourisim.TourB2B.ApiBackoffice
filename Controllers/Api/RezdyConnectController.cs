using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.Rezdy;
using TourManagementApi.Models.Api.TourManagementApi.Models.Api.Rezdy;
using TourManagementApi.Services;
using TourManagementApi.Services.Rezdy;

namespace TourManagementApi.Controllers.Api
{
    [ApiController]
    [Route("api/rezdyconnect")]
    [ApiExplorerSettings(GroupName = "rezdy")]
    public class RezdyConnectController : ControllerBase
    {

        private readonly IProductService _productService;
        private readonly AvailabilityService _availabilityService;
        private readonly Services.BookingService _bookingService;
        private readonly string _validApiKey;

        public RezdyConnectController(
            IProductService productService,
            AvailabilityService availabilityService,
             IConfiguration configuration,
            Services.BookingService bookingService)
        {
            _productService = productService;
            _availabilityService = availabilityService;
            _bookingService = bookingService;
            _validApiKey = configuration["Rezdy:ApiKey"];
        }

        private bool IsApiKeyValid(string? apiKey) => apiKey == _validApiKey;

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

        //TODO: Errorcodes 
        // TODO  : Rezervasyon ve booking de gelen dataları kontrol et
        [HttpPost("reservation")]
        public async Task<IActionResult> CreateReservation(
            [FromQuery] string apiKey,
            [FromQuery] string externalProductCode,
            [FromBody] RezdyBookingRequest request)
        {
            if (apiKey != _validApiKey)
                return Unauthorized(new
                {
                    requestStatus = new { success = false, message = "Invalid API Key" }
                });

            try
            {
                var req = await _bookingService.CreateReservationAsync(request, externalProductCode);
                var responseBooking = new BookingResponseBooking
                {
                    OrderNumber = req.OrderNumber,
                    Comments = req.Comments,
                    ResellerComments = req.ResellerComments,
                    BarcodeType = req.Fields?.FirstOrDefault()?.BarcodeType ?? "QR_CODE",
                    Fields = req.Fields?.Select(f => new BookingRequestField
                    {
                        Label = f.Label,
                        Value = f.Value,
                        BarcodeType = f.BarcodeType
                    }).ToList() ?? new(),
                    Items = req.Items?.Select(i => new BookingResponseItem
                    {
                        Participants = i.Participants?.Select(p => new BookingResponseParticipant
                        {
                            Fields = p.Fields.Select(f => new BookingRequestField
                            {
                                Label = f.Label,
                                Value = f.Value,
                                BarcodeType = f.BarcodeType
                            }).ToList()
                        }).ToList() ?? new()
                    }).ToList() ?? new(),
                    RequestStatus = new Models.Api.TourManagementApi.Models.Api.Rezdy.RequestStatus
                    {
                        Code = "SUCCESS",
                        Message = "Reservation created",
                        HttpStatus = 200
                    }
                };

                return Ok(new
                {
                    requestStatus = new { success = true },
                    bookings = new[] { responseBooking }
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

        [HttpPut("booking")]
        public async Task<IActionResult> ConfirmBooking([FromQuery] string apiKey,
        [FromBody] RezdyBookingRequest request)
        {
            // Geçersiz API Key
            if (apiKey != _validApiKey)
            {
                return Ok(new
                {
                    bookings = new object[0],
                    requestStatus = new { code = "401", message = "Unauthorized - Invalid API Key" },
                    error = new
                    {
                        errorCode = "RC_INVALID_API_KEY",
                        errorMessage = "The API key provided is invalid."
                    }
                });
            }

            // Status kontrolü
            if (request.Status != "CONFIRMED")
            {
                return Ok(new
                {
                    bookings = new object[0],
                    requestStatus = new { code = "400", message = "Bad Request - Invalid Status" },
                    error = new
                    {
                        errorCode = "RC_INVALID_DATA",
                        errorMessage = "Status must be 'CONFIRMED'.",
                        fields = new[]
                        {
                    new {
                        field = "Status",
                        message = "Must be 'CONFIRMED'."
                    }
                }
                    }
                });
            }

            // Rezervasyon onayı
            var result = await _bookingService.ConfirmBookingAsync(request);
            if (!result)
            {
                return Ok(new
                {
                    bookings = new object[0],
                    requestStatus = new { code = "404", message = "Reservation not found or already confirmed." },
                    error = new
                    {
                        errorCode = "RC_RESERVATION_NOT_FOUND",
                        errorMessage = "Reservation not found or already confirmed."
                    }
                });
            }

            // Booking response oluştur
            var item = request.Items.FirstOrDefault();

            var bookingResponse = new
            {
                orderNumber = request.OrderNumber,
                barcodeType = "CODE_128",
                comments = request.Comments,
                resellerComments = "",
                fields = request.Fields?.Select(f => new
                {
                    barcodeType = "CODE_128",
                    label = f.Label,
                    value = f.Value
                }),
                items = new[] {
                    new {
                        productCode = item?.ProductCode,
                        totalQuantity = item?.TotalQuantity ?? 0,
                        participants = item?.Participants?.Select(p => new {
                            fields = p.Fields?.Select(pf => new {
                                barcodeType = "CODE_128",
                                label = pf.Label,
                                value = pf.Value
                            })
                        })
                    }
                }
            };

            return Ok(new
            {
                bookings = new[] { bookingResponse }
                // Başarılı ise requestStatus ve error alanları dönülmez.
            });
        }

        [HttpPut("cancellation")]
        public async Task<IActionResult> CancelBooking([FromQuery] string apiKey, [FromBody] RezdyBookingRequest booking)
        {
            if (apiKey != _validApiKey)
            {
                return Unauthorized(new
                {
                    requestStatus = new { code = "401", message = "Invalid API Key" },
                    error = new
                    {
                        errorCode = "RC_INVALID_API_KEY",
                        errorMessage = "Provided API Key is not valid."
                    }
                });
            }

            var status = "CANCELLED";

            var result = await _bookingService.CancelReservationAsync(booking.OrderNumber, status);

            if (!result)
            {
                return NotFound(new
                {
                    requestStatus = new { code = "404", message = "Booking not found." },
                    error = new
                    {
                        errorCode = "RC_RESERVATION_NOT_FOUND",
                        errorMessage = "Booking not found or already cancelled."
                    }
                });
            }

            // Sadece boş bir JSON nesnesi dönülüyor
            return Ok(new { });
        }
    }
}
