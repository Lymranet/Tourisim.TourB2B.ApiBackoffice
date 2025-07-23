using Newtonsoft.Json;
using System;

namespace TourManagementApi.Models.Api
{
    public class RezdyApiModels
    {
    }

    public class AvailabilityRequest
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("sessions")]
        public Session[] Sessions { get; set; }
    }

    public class Session
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
    public class AvailabilityResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public AvailabilityData ResponseData { get; set; }
    }

    public class AvailabilityData
    {
        [JsonProperty("availability")]
        public AvailabilityItem[] Availability { get; set; }
    }

    public class AvailabilityItem
    {
        [JsonProperty("sessionId")]
        public int SessionId { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("quantityAvailable")]
        public int QuantityAvailable { get; set; }

        [JsonProperty("quantitySold")]
        public int QuantitySold { get; set; }

        [JsonProperty("price")]
        public PriceDetail Price { get; set; }
    }

    public class PriceDetail
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class RequestStatus
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class ProductCreateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("priceOptions")]
        public PriceOption[] PriceOptions { get; set; }

        [JsonProperty("locations")]
        public Location[] Locations { get; set; }
    }

    public class PriceOption
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("priceType")]
        public string PriceType { get; set; }
    }

    public class Location
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
    public class ProductResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public ProductResponseData ResponseData { get; set; }
    }

    public class ProductResponseData
    {
        [JsonProperty("product")]
        public ProductDetail Product { get; set; }
    }

    public class ProductDetail
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        // gerekirse ek alanlar...
    }


    public class BookingCreateRequest
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("sessionId")]
        public int SessionId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        // gerekirse başka müşteri bilgileri...
    }

    public class BookingResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public BookingResponseData ResponseData { get; set; }
    }

    public class BookingResponseData
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("booking")]
        public BookingDto Booking { get; set; }
    }
    public class BookingDto
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("sessionId")]
        public int SessionId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        // gerekirse müşteri ve ödeme detaylarını ekleyin...
    }
    public class ImageUploadResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public ImageUploadResponseData ResponseData { get; set; }
    }

    public class ImageUploadResponseData
    {
        [JsonProperty("imageId")]
        public int ImageId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

}
