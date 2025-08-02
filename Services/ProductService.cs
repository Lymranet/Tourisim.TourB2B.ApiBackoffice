using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using TourManagementApi.Data;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;
using ExtraDto = TourManagementApi.Models.Api.RezdyConnectModels.ExtraDto;
using PickupLocationDto = TourManagementApi.Models.Api.RezdyConnectModels.PickupLocationDto;

namespace TourManagementApi.Services
{
    public class ProductService : IProductService
    {

        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductService(ApplicationDbContext context, HttpClient httpClient, IConfiguration configuration)
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
                BookingFields = new List<BookingField>() // Map as needed if data is available
            };
        }
    }

    public static class ActivityExtensions
    {
        public static RezdyProductDto ToRezdyActivityDto(this Activity activity, string apiKey, string baseApiUrl, string baseImageUrl)
        {
            try
            {
                // Resimler
                var images = new List<ImageItem>();
                if (!string.IsNullOrWhiteSpace(activity.CoverImage))
                    images.Add(new ImageItem { ItemUrl = CombineUrl(baseImageUrl, activity.CoverImage) });

                if (!string.IsNullOrWhiteSpace(activity.PreviewImage))
                    images.Add(new ImageItem { ItemUrl = CombineUrl(baseImageUrl, activity.PreviewImage) });

                if (!string.IsNullOrWhiteSpace(activity.GalleryImages))
                {
                    var gallery = JsonSerializer.Deserialize<List<string>>(activity.GalleryImages);
                    images.AddRange(gallery.Select(url => new ImageItem { ItemUrl = CombineUrl(baseImageUrl, url) }));
                }

                // Diller
                var languages = activity.ActivityLanguages.Select(l => l.LanguageCode).ToList();
                if (!languages.Any()) languages.Add("en_us");


                //var priceOptions = option.TicketCategories
                //    .Select(tc => new PriceOptionDto
                //    {
                //        Label = tc.Type,
                //        Price = tc.Amount,
                //        SeatsUsed = tc.MaxSeats > 0 ? tc.MaxSeats : 1 // MaxSeats varsa kullan, yoksa 1 olarak ayarla
                //    }).ToList();

                // Fiyat Opsiyonları
                var priceOptions = activity.Options
                    .SelectMany(o => o.TicketCategories)
                    .Select(tc => new PriceOptionDto
                    {
                        Label = tc.Type,
                        Price = tc.Amount,
                        SeatsUsed = tc.MaxSeats > 0 ? tc.MaxSeats : 1
                    }).ToList();

                // En düşük fiyat reklam fiyatı olarak kullanılabilir
                var advertisedPrice = priceOptions.Min(p => p.Price);

                // Pickup noktaları
                var pickupLocations = activity.MeetingPoints.Select(mp => new PickupLocationDto
                {
                    Address = mp.Address,
                    Latitude = double.TryParse(mp.Latitude, out var lat) ? lat : 0,
                    Longitude = double.TryParse(mp.Longitude, out var lon) ? lon : 0,
                    LocationName = mp.Name,
                    PickupInstructions = "Meet at location"
                }).ToList();

                // Extra (addon) ürünler
                var extras = activity.Addons.Select(a => new ExtraDto
                {
                    Name = a.Title,
                    Description = a.Description ?? "",
                    Price = a.PriceAmount,
                    Quantity = 10,
                    Image = new ImageItem { ItemUrl = $"{baseImageUrl}/default-addon.jpg" } // isteğe bağlı
                }).ToList();

                // Booking alanları (GuestFieldsJson'dan)
                var bookingFields = new List<BookingField>();

                if (!string.IsNullOrWhiteSpace(activity.GuestFieldsJson))
                {
                    try
                    {
                        bookingFields = JsonSerializer.Deserialize<List<BookingField>>(activity.GuestFieldsJson) ?? new List<BookingField>();
                    }
                    catch
                    {
                        bookingFields = GetDefaultBookingFields();
                    }
                }

                // Eğer deserialize sonrası da boşsa, yine default kullan
                if (bookingFields == null || !bookingFields.Any())
                {
                    bookingFields = GetDefaultBookingFields();
                }

                // Rezdy konum bilgisi (tek meetingPoint ya da Destination bilgisiyle dolgu yapılabilir)
                var mainLocation = activity.MeetingPoints.FirstOrDefault();
                var locationAddress = new LocationAddressDto
                {
                    AddressLine = mainLocation?.Address ?? activity.DestinationName,
                    City = activity.DestinationName,
                    CountryCode = activity.CountryCode.ToUpper(),
                    PostCode = "0000",
                    State = "UNKNOWN"
                };

                // Tax bilgisi örnek (geliştirilebilir)
                var taxes = new List<TaxDto>
                {
                    new TaxDto
                    {
                        Label = "VAT",
                        TaxFeeType = "TAX",
                        TaxType = "PERCENT",
                        TaxPercent = 20,
                        PriceInclusive = true,
                        Compound = false
                    }
                };
                //Kapasiteler
                var allCategories = activity.Options
                .SelectMany(o => o.TicketCategories)
                .ToList();

                var quantityRequired = allCategories.Any();
                var quantityRequiredMin = quantityRequired ? allCategories.Min(tc => tc.MinSeats > 0 ? tc.MinSeats : 1) : 1;
                var quantityRequiredMax = quantityRequired ? allCategories.Max(tc => tc.MaxSeats > 0 ? tc.MaxSeats : 10) : 10;

                //Lablerlar
                var firstType = activity.Options
                    .SelectMany(o => o.TicketCategories)
                    .Select(tc => tc.Type)
                    .FirstOrDefault();

                var unitLabel = string.IsNullOrWhiteSpace(firstType) ? "Passenger" : firstType;

                // Ekstra bilgiler

                var resultdto = new RezdyProductDto
                {
                    InternalCode = activity.Id.ToString(),
                   ProductCode = $"P{activity.Id:00000}",
                    ExternalCode = activity.Id.ToString(),
                    Name = activity.Title,
                    Label = activity.Label,
                    ShortDescription = activity.Description.Length > 240 ? activity.Description.Substring(0, 240) : activity.Description,
                    Description = activity.Description,
                    DurationMinutes = activity.Duration,
                    BookingMode = "INVENTORY", // Alternatif: "NO_DATE", "DATE_ENQUIRY"
                    ConfirmMode = "AUTOCONFIRM",
                    BarcodeType = "QR_CODE",
                    BarcodeOutputType = "PARTICIPANT",
                    Languages = languages,
                    Images = images,
                    PriceOptions = priceOptions,
                    Extras = extras,
                    PickupLocations = pickupLocations,
                    AdvertisedPrice = advertisedPrice,
                    QuantityRequired = quantityRequired,
                    QuantityRequiredMin = quantityRequiredMin,
                    QuantityRequiredMax = quantityRequiredMax,
                    UnitLabel = unitLabel,
                    UnitLabelPlural = unitLabel.EndsWith("s") ? unitLabel : unitLabel + "s",
                    ProductType = "ACTIVITY",
                    BookingFields = bookingFields,
                    Terms = !string.IsNullOrWhiteSpace(activity.ImportantInfo)
                        ? activity.ImportantInfo
                        : "Please refer to our general terms and conditions.",

                    AdditionalInformation = string.Join("\n\n", new[]
                        {
                        activity.Highlights,
                        activity.ImportantInfo,
                        activity.Inclusions,
                        activity.Exclusions
                    }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    LocationAddress = locationAddress,
                    Taxes = taxes,
                    RezdyConnectSettings = new RezdyConnectSettings
                    {
                        ExternalAvailabilityApi = $"{baseApiUrl}/api/rezdyconnect/availability?apiKey={apiKey}&productCode={activity.Id}",
                        ExternalReservationApi = $"{baseApiUrl}/api/rezdyconnect/reservation?apiKey={apiKey}",
                        ExternalBookingApi = $"{baseApiUrl}/api/rezdyconnect/booking?apiKey={apiKey}",
                        ExternalCancellationApi = $"{baseApiUrl}/api/rezdyconnect/cancellation?apiKey={apiKey}"
                    }
                };
                return resultdto;
            }
            catch (Exception ex)
            {
                throw;
                //return null; // Hata durumunda null döndür
            }
        }

        public static RezdyProductDto ToRezdyDto(this Option option, Activity activity, string apiKey, string baseApiUrl, string baseImageUrl)
        {
            // Görseller
            var images = new List<ImageItem>();
            if (!string.IsNullOrWhiteSpace(activity.CoverImage))
                images.Add(new ImageItem { ItemUrl = CombineUrl(baseImageUrl, activity.CoverImage) });

            if (!string.IsNullOrWhiteSpace(activity.PreviewImage))
                images.Add(new ImageItem { ItemUrl = CombineUrl(baseImageUrl, activity.PreviewImage) });

            if (!string.IsNullOrWhiteSpace(activity.GalleryImages))
            {
                try
                {
                    var gallery = JsonSerializer.Deserialize<List<string>>(activity.GalleryImages);
                    images.AddRange(gallery.Select(url => new ImageItem { ItemUrl = CombineUrl(baseImageUrl, url) }));
                }
                catch { /* geçilebilir */ }
            }

            // Diller (Rezdy uyumlu)
            var languages = activity.ActivityLanguages.Select(l => l.LanguageCode).ToList();
            if (!languages.Any()) languages.Add("en_us");

            // Fiyat opsiyonları
            var priceOptions = option.TicketCategories
                .Select(tc => new PriceOptionDto
                {
                    Label = tc.Type,
                    Price = tc.Amount,
                    SeatsUsed = tc.MaxSeats > 0 ? tc.MaxSeats : 1
                }).ToList();

            // Reklam fiyatı (min fiyat)
            var advertisedPrice = priceOptions.Any() ? priceOptions.Min(p => p.Price) : 0;

            // Pickup noktaları
            var pickupLocations = activity.MeetingPoints.Select(mp => new PickupLocationDto
            {
                Address = mp.Address,
                Latitude = double.TryParse(mp.Latitude, out var lat) ? lat : 0,
                Longitude = double.TryParse(mp.Longitude, out var lon) ? lon : 0,
                LocationName = mp.Name,
                PickupInstructions = "Meet at location"
            }).ToList();

            // Ekstralar
            var extras = activity.Addons.Select(a => new ExtraDto
            {
                Name = a.Title,
                Description = a.Description ?? "",
                Price = a.PriceAmount,
                Quantity = 10,
                Image = new ImageItem { ItemUrl = $"{baseImageUrl}/default-addon.jpg" }
            }).ToList();

            // BookingFields
            var bookingFields = new List<BookingField>();
            //if (!string.IsNullOrWhiteSpace(activity.GuestFieldsJson))
            //{
            //    try
            //    {
            //        bookingFields = JsonSerializer.Deserialize<List<BookingField>>(activity.GuestFieldsJson) ?? new List<BookingField>();
            //    }
            //    catch { }
            //}
            //if (!bookingFields.Any())
            //{
            //    bookingFields = GetDefaultBookingFields();
            //}

            // Adres bilgisi
            var mainLocation = activity.MeetingPoints.FirstOrDefault();
            var locationAddress = new LocationAddressDto
            {
                AddressLine = mainLocation?.Address ?? activity.DestinationName,
                City = activity.DestinationName,
                CountryCode = activity.CountryCode.ToLower(),
                PostCode = "0000",
                State = "UNKNOWN"
            };

            // Tax sabit örnek
            var taxes = new List<TaxDto>
            {
                new TaxDto
                {
                    Label = "VAT",
                    TaxFeeType = "TAX",
                    TaxType = "PERCENT",
                    TaxPercent = 10,
                    PriceInclusive = true,
                    Compound = false
                }
            };

            // Quantity kuralları
            var quantityRequired = option.TicketCategories.Any();
            var quantityRequiredMin = quantityRequired ? option.TicketCategories.Min(tc => tc.MinSeats > 0 ? tc.MinSeats : 1) : 1;
            var quantityRequiredMax = quantityRequired ? option.TicketCategories.Max(tc => tc.MaxSeats > 0 ? tc.MaxSeats : 10) : 10;

            // Label
            var unitLabel = option.TicketCategories.Select(tc => tc.Type).FirstOrDefault() ?? "Passenger";

            // Option bazlı ProductCode ve Name
            var productCode = $"P{activity.Id:00000}_{option.Id:00000}";
            var productName = $"{activity.Title} - {option.Name}";

            // Ek bilgi alanı
            var additionalInformation = string.Join("\n\n", new[]
            {
        activity.Highlights,
        activity.ImportantInfo,
        activity.Inclusions,
        activity.Exclusions
    }.Where(x => !string.IsNullOrWhiteSpace(x)));

            // DTO oluştur
            return new RezdyProductDto
            {
                InternalCode = activity.Id +"-"+option.Id.ToString(),
                ProductCode = productCode,
                ExternalCode = activity.Id + "-" + option.Id.ToString(),
                Name = productName,
                Label = activity.Label,
                ShortDescription = activity.Description.Length > 240 ? activity.Description.Substring(0, 240) : activity.Description,
                Description = activity.Description,
                DurationMinutes = activity.Duration,
                BookingMode = "INVENTORY",
                ConfirmMode = "AUTOCONFIRM",
                BarcodeType = "QR_CODE",
                BarcodeOutputType = "PARTICIPANT",
                Languages = languages,
                Images = images,
                PriceOptions = priceOptions,
                Extras = extras,
                PickupLocations = pickupLocations,
                AdvertisedPrice = advertisedPrice,
                QuantityRequired = quantityRequired,
                QuantityRequiredMin = quantityRequiredMin,
                QuantityRequiredMax = quantityRequiredMax,
                UnitLabel = unitLabel,
                UnitLabelPlural = unitLabel.EndsWith("s") ? unitLabel : unitLabel + "s",
                ProductType = "ACTIVITY",
                BookingFields = bookingFields,
                Terms = !string.IsNullOrWhiteSpace(activity.ImportantInfo) ? activity.ImportantInfo : "Please refer to our terms.",
                AdditionalInformation = additionalInformation,
                LocationAddress = locationAddress,
                Taxes = taxes,
                RezdyConnectSettings = new RezdyConnectSettings
                {
                    ExternalAvailabilityApi = $"{baseApiUrl}/api/rezdyconnect/availability?apiKey={apiKey}&productCode={productCode}",
                    ExternalReservationApi = $"{baseApiUrl}/api/rezdyconnect/reservation?apiKey={apiKey}",
                    ExternalBookingApi = $"{baseApiUrl}/api/rezdyconnect/booking?apiKey={apiKey}",
                    ExternalCancellationApi = $"{baseApiUrl}/api/rezdyconnect/cancellation?apiKey={apiKey}"
                }
            };
        }


        private static List<BookingField> GetDefaultBookingFields()
        {
            return new List<BookingField>
            {
                new BookingField
                {
                    Label = "First Name",
                    FieldType = "String",
                    RequiredPerParticipant = true,
                    RequiredPerBooking = false,
                    VisiblePerParticipant = true,
                    VisiblePerBooking = false
                },
                new BookingField
                {
                    Label = "Last Name",
                    FieldType = "String",
                    RequiredPerParticipant = true,
                    RequiredPerBooking = false,
                    VisiblePerParticipant = true,
                    VisiblePerBooking = false
                },
                new BookingField
                {
                    Label = "Email",
                    FieldType = "String",
                    RequiredPerParticipant = false,
                    RequiredPerBooking = true,
                    VisiblePerParticipant = false,
                    VisiblePerBooking = true
                },
                new BookingField
                {
                    Label = "Phone",
                    FieldType = "Phone",
                    RequiredPerParticipant = false,
                    RequiredPerBooking = true,
                    VisiblePerParticipant = false,
                    VisiblePerBooking = true
                },
                new BookingField
                {
                    Label = "I agree to receive marketing emails",
                    FieldType = "Boolean",
                    RequiredPerParticipant = false,
                    RequiredPerBooking = false,
                    VisiblePerParticipant = false,
                    VisiblePerBooking = true
                }
            };
        }



        public static RezdyProductDto ToRezdyDto1(this Activity activity, string apiKey, string baseApiUrl, string baseImageUrl)
        {
            var images = new List<ImageItem>();

            if (!string.IsNullOrWhiteSpace(activity.CoverImage))
                images.Add(new ImageItem { ItemUrl = CombineUrl(baseImageUrl, activity.CoverImage) });

            if (!string.IsNullOrWhiteSpace(activity.PreviewImage))
                images.Add(new ImageItem { ItemUrl = CombineUrl(baseImageUrl, activity.PreviewImage) });

            if (!string.IsNullOrWhiteSpace(activity.GalleryImages))
            {
                var gallery = JsonSerializer.Deserialize<List<string>>(activity.GalleryImages);
                images.AddRange(gallery.Select(url => new ImageItem { ItemUrl = CombineUrl(baseImageUrl, url) }));
            }



            return new RezdyProductDto
            {
                InternalCode = activity.Id.ToString(),
                Name = activity.Title,
                ShortDescription = activity.Description.Substring(0, Math.Min(200, activity.Description.Length)),
                Description = activity.Description,
                DurationMinutes = activity.Duration,
                Label = activity.Label,
                SupplierId = activity.PartnerSupplierId ?? "UNKNOWN",
                Languages = new List<string>() { "en_us" },//activity.ActivityLanguages.Select(l => l.LanguageCode).ToList(),
                Images = images,
                PriceOptions = activity.Options
                    .SelectMany(o => o.TicketCategories)
                    .Select(tc => new PriceOptionDto
                    {
                        Label = tc.Type,
                        Price = tc.Amount,
                        //Currency = tc.Currency,
                        SeatsUsed = 1
                    })
                .ToList(),
                Extras = activity.Addons.Select(a => new ExtraDto
                {
                    Name = a.Title,
                    Description = a.Description ?? "",
                    Price = a.PriceAmount,
                    Quantity = 10,
                    Image = new ImageItem { ItemUrl = $"{baseImageUrl}/default-addon.jpg" } // opsiyonel
                }).ToList(),
                PickupLocations = activity.MeetingPoints.Select(mp => new PickupLocationDto
                {
                    Address = mp.Address,
                    Latitude = double.TryParse(mp.Latitude, out var lat) ? lat : 0,
                    Longitude = double.TryParse(mp.Longitude, out var lon) ? lon : 0,
                    LocationName = mp.Name,
                    PickupInstructions = "Meet at location"
                }).ToList(),
                RezdyConnectSettings = new RezdyConnectSettings
                {
                    ExternalAvailabilityApi = $"{baseApiUrl}/api/rezdyconnect/availability?apiKey={apiKey}&productCode={activity.Id}",
                    ExternalReservationApi = $"{baseApiUrl}/api/rezdyconnect/reservation?apiKey={apiKey}",
                    ExternalBookingApi = $"{baseApiUrl}/api/rezdyconnect/booking?apiKey={apiKey}",
                    ExternalCancellationApi = $"{baseApiUrl}/api/rezdyconnect/cancellation?apiKey={apiKey}"
                }
            };
        }

        // Yardımcı metot:
        private static string CombineUrl(string baseUrl, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return baseUrl;

            return relativePath.StartsWith("http")
                ? relativePath
                : $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }
    }
}
