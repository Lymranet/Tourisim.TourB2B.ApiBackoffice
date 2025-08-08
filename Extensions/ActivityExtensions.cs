using System.Text.Json;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Extensions
{
    public static class ActivityExtensions
    {

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

            var cat = option.TicketCategories.FirstOrDefault();
            List<PriceOption> priceOptions = new List<PriceOption>();

            if (cat != null && cat.Type == "Group")
            {
                priceOptions = option.TicketCategories.Select(tc => new PriceOption
                {
                    Label = tc.Type,
                    Price = tc.Amount,
                    SeatsUsed = tc.MaxSeats > 0 ? tc.MaxSeats : 1,
                    MinQuantity = tc.MinSeats > 0 ? tc.MinSeats : (int?)null,
                    MaxQuantity = tc.MaxSeats > 0 ? tc.MaxSeats : (int?)null,
                    PriceGroupType = "TOTAL"
                }).ToList();
            }
            else
            {
                priceOptions = option.TicketCategories.Select(tc => new PriceOption
                {
                    Label = tc.Type,
                    Price = tc.Amount,
                    SeatsUsed = tc.MaxSeats > 0 ? tc.MaxSeats : 1
                    // MinQuantity, MaxQuantity, PriceGroupType otomatik olarak null kalacak ve JSON'a yazılmayacak
                }).ToList();
            }


            // Fiyat opsiyonları



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
            var bookingFields = new List<BookingFieldDto>();
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
                InternalCode = activity.Id + "-" + option.Id.ToString(),
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
