using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using TourManagementApi.Data;
using TourManagementApi.Extensions;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;
using ExtraDto = TourManagementApi.Models.Api.RezdyConnectModels.ExtraDto;
using PickupLocationDto = TourManagementApi.Models.Api.RezdyConnectModels.PickupLocationDto;

namespace TourManagementApi.Services
{
    public class ProductService : IProductService
    {

        private readonly TourManagementDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductService(TourManagementDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public List<RezdyProductDto> GetAll()
        {
            var activities = _context.Activities
                .Include(a => a.ActivityLanguages)
                .Include(a => a.Addons)
                .Include(a => a.PriceCategories)
                .Include(a => a.MeetingPoints)
                .Include(a => a.Options).ThenInclude(o => o.TicketCategories)
                .ToList();

            var apiKey = _configuration["Rezdy:ApiKey"];
            var baseApiUrl = _configuration["Rezdy:BaseUrl"];
            var baseImageUrl = _configuration["Rezdy:BaseImageUrl"]; // <-- yeni satır
            List<RezdyProductDto> list = new List<RezdyProductDto>();
            foreach (var item in activities)
            {
                foreach (var item2 in item.Options)
                {
                    try
                    {
                        list.Add(item2.ToRezdyDto(item,apiKey, baseImageUrl, baseImageUrl));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            return list;

            //return activities.Select(a => a.ToRezdyDto(apiKey, baseImageUrl, baseImageUrl)).ToList();
        }

        public ProductResponseModel? GetByCode(string productCode)
        {
            // Find the RezdyProductDto by productCode
            var rezdyProduct = GetAll().FirstOrDefault(p => p.ProductCode == productCode);
            if (rezdyProduct == null)
                return null;

            // Map RezdyProductDto to ProductResponseModel
            return new ProductResponseModel
            {
                ProductCode = rezdyProduct.ProductCode,
                ExternalCode = rezdyProduct.ExternalCode,
                Title = rezdyProduct.Name,
                Description = rezdyProduct.Description,
                BookingMode = rezdyProduct.BookingMode,
                BarcodeType = rezdyProduct.BarcodeType,
                BarcodeOutputType = rezdyProduct.BarcodeOutputType,
                BookingFields = new List<BookingFieldDto>() // Map as needed if data is available
            };
        }
    }


}
