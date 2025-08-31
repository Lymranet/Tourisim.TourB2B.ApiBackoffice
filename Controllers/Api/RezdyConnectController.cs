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
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey != _validApiKey)
            {
                return StatusCode(403, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_AUTH_ERROR",
                            errorMessage = "Invalid ApiKey"
                        }
                    }
                });
            }

            try
            {
                List<RezdyProductDto> allProducts = _productService.GetAll();
                var matchedProducts = new List<RezdyProductDto>();

                if (externalProductCodes != null && externalProductCodes.Any())
                {
                    foreach (var code in externalProductCodes)
                    {
                        var match = allProducts.FirstOrDefault(p => p.InternalCode == code);
                        if (match == null)
                        {
                            return UnprocessableEntity(new
                            {
                                requestStatus = new
                                {
                                    error = new
                                    {
                                        errorCode = "RC_INVALID_PRODUCT",
                                        errorMessage = $"Product '{code}' does not exist in the system"
                                    }
                                }
                            });
                        }

                        matchedProducts.Add(match);
                    }
                }
                else
                {
                    matchedProducts = allProducts;
                }

                var pagedProducts = matchedProducts
                    .Skip(offset)
                    .Take(limit)
                    .ToList();

                return Ok(new
                {
                    requestStatus = new { success = true },
                    products = pagedProducts
                });
            }
            catch (TimeoutException tex)
            {
                return StatusCode(500, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INTERNAL_ERROR",
                            errorMessage = $"Connection timeout while performing the request"
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_ENDPOINT_UNAVAILABLE",
                            errorMessage = "Product Endpoint can not process the request at the moment"
                        }
                    }
                });
            }
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
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey != _validApiKey)
            {
                return StatusCode(403, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_AUTH_ERROR",
                            errorMessage = "Invalid ApiKey"
                        }
                    }
                });
            }

            try
            {
                // Ürün var mı kontrolü
                bool productExists = _productService.Exists(productCode, externalProductCode);
                if (!productExists)
                {
                    string productIdentifier = !string.IsNullOrEmpty(productCode) ? productCode : externalProductCode;
                    return StatusCode(422, new
                    {
                        requestStatus = new
                        {
                            error = new
                            {
                                errorCode = "RC_INVALID_PRODUCT",
                                errorMessage = $"Product '{productIdentifier}' does not exist in the system"
                            }
                        }
                    });
                }

                var sessions = _availabilityService.GetSessions(productCode, externalProductCode, from, to);

                return Ok(new
                {
                    requestStatus = new { success = true },
                    sessions
                });
            }
            catch (TimeoutException)
            {
                return StatusCode(500, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INTERNAL_ERROR",
                            errorMessage = "Connection timeout while performing the request"
                        }
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(503, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_ENDPOINT_UNAVAILABLE",
                            errorMessage = "Availability Endpoint can not process the request at the moment"
                        }
                    }
                });
            }
        }


        //TODO: Errorcodes 
        [HttpPost("reservation")]
        public async Task<IActionResult> CreateReservation(
           [FromQuery] string apiKey,
           [FromQuery] string externalProductCode,
           [FromBody] RezdyBookingRequest request)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey != _validApiKey)
            {
                return StatusCode(403, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_AUTH_ERROR",
                            errorMessage = "Invalid ApiKey"
                        }
                    }
                });
            }

            if (string.IsNullOrWhiteSpace(externalProductCode))
            {
                return BadRequest(new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INVALID_DATA",
                            errorMessage = "Missing query parameters",
                            fields = new[]
                            {
                        new
                        {
                            label = "externalProductCode",
                            reason = "externalProductCode is required"
                        }
                    }
                        }
                    }
                });
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INVALID_DATA",
                            errorMessage = "Missing request body",
                            fields = new[]
                            {
                        new
                        {
                            label = "request",
                            reason = "Request body is required"
                        }
                    }
                        }
                    }
                });
            }

            try
            {
                var result = await _bookingService.CreateReservationAsync(request, externalProductCode);

                // Örnek hatalar - bu kısım senin servisindeki iş mantığına göre çalışmalı:
                if (!_productService.Exists("",externalProductCode))
                {
                    return StatusCode(422, new
                    {
                        requestStatus = new
                        {
                            error = new
                            {
                                errorCode = "RC_INVALID_PRODUCT",
                                errorMessage = $"Product {externalProductCode} does not exist in the system"
                            }
                        }
                    });
                }

                //if (!result.HasAvailability)
                //{
                //    return StatusCode(422, new
                //    {
                //        requestStatus = new
                //        {
                //            error = new
                //            {
                //                errorCode = "RC_NO_AVAILABILITY",
                //                errorMessage = "No of tickets requested is more than the available seats present",
                //                seatsAvailable = result.SeatsAvailable
                //            }
                //        }
                //    });
                //}

                //if (result.HasInvalidPriceOption)
                //{
                //    return StatusCode(422, new
                //    {
                //        requestStatus = new
                //        {
                //            error = new
                //            {
                //                errorCode = "RC_INVALID_PRICE_OPTION",
                //                errorMessage = $"Product {externalProductCode} does not have the priceOptions supported as below",
                //                priceOption = result.InvalidPriceOptions.Select(po => new
                //                {
                //                    po.Label,
                //                    po.Min,
                //                    po.Max
                //                })
                //            }
                //        }
                //    });
                //}

                //if (result.QuantityBelowMinimum)
                //{
                //    return StatusCode(422, new
                //    {
                //        requestStatus = new
                //        {
                //            error = new
                //            {
                //                errorCode = "RC_MINIMUM_QUANTITY_REQUIRED",
                //                errorMessage = $"Reservation of product {externalProductCode} requires at least {result.MinimumQuantity} participants",
                //                quantityRequiredMin = result.MinimumQuantity
                //            }
                //        }
                //    });
                //}

                var responseBooking = new BookingResponseBooking
                {
                    OrderNumber = result.OrderNumber,
                    Comments = result.Comments,
                    ResellerComments = result.ResellerComments,
                    BarcodeType = result.Fields?.FirstOrDefault()?.BarcodeType ?? "QR_CODE",
                    Fields = result.Fields?.Select(f => new BookingRequestField
                    {
                        Label = f.Label,
                        Value = f.Value,
                        BarcodeType = f.BarcodeType
                    }).ToList() ?? new(),
                    Items = result.Items?.Select(i => new BookingResponseItem
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
            catch (TimeoutException)
            {
                return StatusCode(500, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INTERNAL_ERROR",
                            errorMessage = "Connection timeout while performing the request"
                        }
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(503, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_ENDPOINT_UNAVAILABLE",
                            errorMessage = "Reservation Endpoint can not process the request at the moment"
                        }
                    }
                });
            }
        }


        [HttpPut("booking")]
        public async Task<IActionResult> ConfirmBooking(
          [FromQuery] string apiKey,
          [FromBody] RezdyBookingRequest request)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey != _validApiKey)
            {
                return StatusCode(403, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_AUTH_ERROR",
                            errorMessage = "Invalid ApiKey"
                        }
                    }
                });
            }

            if (request == null || string.IsNullOrWhiteSpace(request.OrderNumber))
            {
                return BadRequest(new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INVALID_DATA",
                            errorMessage = "Missing query parameters",
                            fields = new[]
                            {
                        new
                        {
                            label = "OrderNumber",
                            reason = "Missing OrderNumber"
                        }
                    }
                        }
                    }
                });
            }

            if (request.Status != "CONFIRMED")
            {
                return BadRequest(new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INVALID_DATA",
                            errorMessage = "Status must be 'CONFIRMED'.",
                            fields = new[]
                            {
                        new
                        {
                            label = "Status",
                            reason = "Must be 'CONFIRMED'."
                        }
                    }
                        }
                    }
                });
            }

            try
            {
                // Reservation onayı
                var result = await _bookingService.ConfirmBookingAsync(request);

                if (result == null)
                {
                    return StatusCode(422, new
                    {
                        requestStatus = new
                        {
                            error = new
                            {
                                errorCode = "RC_INVALID_ORDER",
                                errorMessage = $"Order with the reservation reference '{request.OrderNumber}' does not exist"
                            }
                        }
                    });
                }

                //if (!result.ProductExists)
                //{
                //    return StatusCode(422, new
                //    {
                //        requestStatus = new
                //        {
                //            error = new
                //            {
                //                errorCode = "RC_INVALID_PRODUCT",
                //                errorMessage = $"Product '{result.ProductCode}' does not exist in the system"
                //            }
                //        }
                //    });
                //}

                //if (result.ExceedsMaxQuantity)
                //{
                //    return StatusCode(422, new
                //    {
                //        requestStatus = new
                //        {
                //            error = new
                //            {
                //                errorCode = "RC_MAXIMUM_QUANTITY_REACHED",
                //                errorMessage = $"Booking of product {result.ProductCode} reached beyond the limit of {result.MaxQuantity} participants",
                //                quantityRequiredMax = result.MaxQuantity
                //            }
                //        }
                //    });
                //}

                // Başarılı yanıt
                var item = request.Items?.FirstOrDefault();

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
                    items = new[]
                    {
                new
                {
                    productCode = item?.ProductCode,
                    totalQuantity = item?.TotalQuantity ?? 0,
                    participants = item?.Participants?.Select(p => new
                    {
                        fields = p.Fields?.Select(pf => new
                        {
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
                    // Başarılı durumda requestStatus veya error dönülmüyor.
                });
            }
            catch (TimeoutException)
            {
                return StatusCode(500, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_INTERNAL_ERROR",
                            errorMessage = "Connection timeout while performing the request"
                        }
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(503, new
                {
                    requestStatus = new
                    {
                        error = new
                        {
                            errorCode = "RC_ENDPOINT_UNAVAILABLE",
                            errorMessage = "Booking Endpoint can not process the request at the moment"
                        }
                    }
                });
            }
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

            var result = await _bookingService.CancelReservationAsync(booking);

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
