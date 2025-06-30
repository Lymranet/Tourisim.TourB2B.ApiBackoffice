using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TourManagementApi.Data;
using TourManagementApi.Helper;
using TourManagementApi.Models;
using TourManagementApi.Models.ViewModels;

namespace TourManagementApi.Controllers.Api
{
    [Route("supplier/12004/activities")]
    [ApiController]
    public class ExperienceBankActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public Dictionary<string, List<int>> categoryMapping = new Dictionary<string, List<int>>
        {
            { "Hiking", new List<int> { 1, 10 } },
            { "Rock Climbing", new List<int> { 1, 5 } },
            { "Rafting", new List<int> { 2 } },
            { "Kayaking", new List<int> { 2 } },
            { "Camping", new List<int> { 10 } },
            { "Zip-lining", new List<int> { 5 } },
            { "Paragliding", new List<int> { 5 } },
            { "Museum", new List<int> { 4 } },
            { "Historical Site", new List<int> { 11 } },
            { "Art Tour", new List<int> { 4, 11 } },
            { "Wildlife", new List<int> { 10 } },
            { "Botanical", new List<int> { 10 } },
            { "Eco Tour", new List<int> { 10, 11 } },
            { "Football", new List<int> { 8 } },
            { "Basketball", new List<int> { 8 } },
            { "Tennis", new List<int> { 8 } },
            { "Wine Tasting", new List<int> { 14 } },
            { "Cooking Class", new List<int> { 14 } },
            { "Street Food", new List<int> { 14 } },
            { "Spa", new List<int> { 11 } },
            { "Yoga", new List<int> { 11 } },
            { "Retreat", new List<int> { 11 } },
        };
        private readonly ILogger<ExperienceBankActivitiesController> _logger;
        public ExperienceBankActivitiesController(ApplicationDbContext context, ILogger<ExperienceBankActivitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities(string partnerSupplierId, [FromQuery] string[] id, [FromQuery] int offset = 0)
        {
            try
            {
                _logger.LogInformation("GetActivities called with partnerSupplierId={partnerSupplierId}, ids={ids}, offset={offset}",
        partnerSupplierId, string.Join(",", id ?? new string[0]), offset);

                // Sadece belirli ID'lerle filtrelenmiş tur listesi
                var query = _context.Activities
                    .Include(a => a.Options)
                    .Include(a => a.MeetingPoints)
                    .Include(a => a.RoutePoints)
                    .Where(a => a.PartnerSupplierId == partnerSupplierId);

                if (id?.Any() == true)
                {
                    // SQL Server 2014 için OPENJSON kullanmamak adına string id'leri int'e parse etecez hata veriyo
                    var idInts = id.Select(int.Parse).ToList();
                    query = query.Where(a => idInts.Contains(a.Id));
                }

                const int pageSize = 50;
                var total = await _context.Activities.Where(a => a.PartnerSupplierId == partnerSupplierId).CountAsync();

                var activityIds = await query.Where(a => a.PartnerSupplierId == partnerSupplierId)
               .Skip(offset)
               .Take(pageSize)
               .Select(a => a.Id)
               .ToListAsync();

                var activities = await _context.Activities.Where(a => a.PartnerSupplierId == partnerSupplierId)
                    //.Where(a => activityIds.Contains(a.Id))
                    .Include(a => a.Options)
                    .Include(a => a.MeetingPoints)
                    .Include(a => a.RoutePoints)
                    .ToListAsync();

                var result = new
                {
                    links = new
                    {
                        next = offset + pageSize < total
                            ? $"https://tours.hotelwidget.com/supplier/12004/activities?offset={offset + pageSize}"
                            : null
                    },
                    data = activities.Select(MapToExperienceBankDto)
                };
                _logger.LogInformation("Total activities found: {total}", total);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetActivities for partnerSupplierId={partnerSupplierId}", partnerSupplierId);
                return StatusCode(500, "Internal server error");
            }
        }

        private object MapToExperienceBankDto(Activity activity)
        {
            try
            {
                //Kategorileri eşleştirme
                List<int> matchedCategoryIds = new();
                //var subcategories = activity.Subcategory?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
                //foreach (var subcat in subcategories)
                //{
                //    if (categoryMapping.TryGetValue(subcat, out var matched))
                //    {
                //        matchedCategoryIds.AddRange(matched);
                //    }
                //}
                var finalCategories = matchedCategoryIds.Distinct().Take(3).ToList();


                // GuestFieldsJson oluşturma Bu kısım, GuestFieldsViewModel sınıfının örneğini kullanarak JSON formatında guest fields oluşturur.
                // Bu, API'nin döndüreceği guest fields bilgisini yapılandırmak için kullanılır.
                activity.GuestFieldsJson = JsonSerializer.Serialize(new List<GuestFieldsViewModel.GuestField>
            {
                new()
                {
                    Code = "email",
                    Label = "E-posta",
                    Type = "text",
                    Required = true,
                    Translations = new List<GuestFieldsViewModel.GuestFieldTranslation>
                    {
                        new() { Language = "en", Label = "Email" }
                    }
                },
                new()
                {
                    Code = "gender",
                    Label = "Cinsiyet",
                    Type = "radio",
                    Required = false,
                    Options = new List<GuestFieldsViewModel.GuestFieldOption>
                    {
                        new()
                        {
                            Key = "m",
                            Value = "Erkek",
                            Translations = new List<GuestFieldsViewModel.GuestFieldOptionTranslation>
                            {
                                new() { Language = "en", Value = "Male" }
                            }
                        },
                        new()
                        {
                            Key = "f",
                            Value = "Kadın",
                            Translations = new List<GuestFieldsViewModel.GuestFieldOptionTranslation>
                            {
                                new() { Language = "en", Value = "Female" }
                            }
                        }
                    },
                    Translations = new List<GuestFieldsViewModel.GuestFieldTranslation>
                    {
                        new() { Language = "en", Label = "Gender" }
                    }
                },
                new()
                {
                    Code = "phone",
                    Label = "Telefon",
                    Type = "text",
                    Required = true,
                    Translations = new List<GuestFieldsViewModel.GuestFieldTranslation>
                    {
                        new() { Language = "en", Label = "Phone" }
                    }
                }
            });

                return new
                {
                    id = activity.Id.ToString(),
                    title = activity.Title,
                    description = activity.Description,
                    highlights = activity.Highlights,
                    itinerary = activity.Itinerary,
                    inclusions = TxtJson.DeserializeStringList(activity.Inclusions) ?? new List<string>(),
                    exclusions = TxtJson.DeserializeStringList(activity.Exclusions) ?? new List<string>(),
                    importantInfo = TxtJson.DeserializeStringList(activity.ImportantInfo) ?? new List<string>(),
                    detailsPageUrl = activity.DetailsUrl,
                    destination = new
                    {
                        countryCode = activity.CountryCode,
                        code = activity.DestinationCode,
                        name = activity.DestinationName
                    },
                    media = new
                    {
                        images = new
                        {
                            header = activity.CoverImage,
                            teaser = activity.PreviewImage,
                            gallery = TxtJson.DeserializeStringList(activity.GalleryImages) ?? new List<string>()
                        },
                        videos = TxtJson.DeserializeStringList(activity.MediaVideos) ?? new List<string>()
                    },
                    rating = activity.Rating != null ? new
                    {
                        averageValue = activity.Rating,
                        totalCount = activity.TotalRatingCount
                    } : null,
                    categories = TxtJson.SerializeIntList(finalCategories),
                    meetingPoints = (activity.MeetingPoints ?? new List<MeetingPoint>())
                        .Select(mp => new
                        {
                            name = mp.Name,
                            latitude = mp.Latitude,
                            longitude = mp.Longitude,
                            address = mp.Address
                        }).ToList(),
                    route = (activity.RoutePoints ?? new List<RoutePoint>())
                        .Select(rp => new
                        {
                            name = rp.Name,
                            latitude = rp.Latitude,
                            longitude = rp.Longitude
                        }).ToList(),
                    guestFields = activity.GuestFieldsJson,
                    addons = activity.Addons != null ? activity.Addons.Select(ad => new
                    {
                        id = ad.Id,
                        title = ad.Title,
                        description = ad.Description,
                        type = ad.Type,
                        price = new
                        {
                            amount = ad.PriceAmount.ToString("0.00"),
                            currency = ad.Currency
                        },
                        translations = ad.AddonTranslations != null
                                    ? ad.AddonTranslations.Select(tr => new
                                    {
                                        language = tr.Language,
                                        title = tr.Title,
                                        description = tr.Description
                                    }).Cast<object>().ToList()
                                    : new List<object>()
                    }).Cast<object>().ToList()
                            : new List<object>(),
                    isActive = activity.Status == "active",
                    cancellationPolicy = new
                    {
                        isFreeCancellation = activity.IsFreeCancellation,
                        refundConditions = activity.CancellationPolicyConditions != null && activity.CancellationPolicyConditions.Any()
                    ? activity.CancellationPolicyConditions.Select(rc => new
                    {
                        minDurationBeforeStartTimeSec = rc.MinDurationBeforeStartTimeSec,
                        refundPercentage = rc.RefundPercentage
                    }).Cast<object>().ToList()
                    : new List<object>()
                    },

                    options = activity.Options.Select(op => new
                    {
                        id = op.Id,
                        name = op.Name,
                        duration = op.Duration,
                        startTime = op.StartTime ?? "00:00:00",
                        openingHours = op.OpeningHours != null ? op.OpeningHours.Select(oh => new
                        {
                            fromTime = oh.FromTime,
                            toTime = oh.ToTime
                        }).Cast<object>().ToList()
                            : new List<object>(),
                        fromDate = op.FromDate,
                        untilDate = op.UntilDate,
                        cutOff = op.CutOff,
                        weekdays = op.Weekdays,
                        canBeBookedAfterStartTime = op.CanBeBookedAfterStartTime,
                        ticketCategories = op.TicketCategories.Select(tc => new
                        {
                            id = tc.Id,
                            name = tc.Name,
                            minSeats = tc.MinSeats,
                            maxSeats = tc.MaxSeats,
                            price = new
                            {
                                type = tc.PriceType,
                                amount = tc.Amount.ToString("0.00"),
                                currency = tc.Currency
                            },
                            type = tc.Type,
                            ageLimit = tc.MinAge != null ? new
                            {
                                minAge = tc.MinAge,
                                maxAge = tc.MaxAge
                            } : null,
                            translations = new List<object>(), // TODO: TicketCategories translateleri de yok mk Assuming translations are not provided in the original model
                        }),
                        translations = new List<object>(), // TODO: Optionların translateleri de yok mk Assuming translations are not provided in the original model
                    }),
                    translations = activity.Translations != null
                            ? activity.Translations.Select(tr => new
                            {
                                language = tr.Language,
                                title = tr.Title,
                                description = tr.Description,
                                highlights = tr.Highlights,
                                itinerary = tr.Itinerary,
                                inclusions = tr.InclusionsJson,
                                exclusions = tr.ExclusionsJson,
                                importantInfo = tr.ImportantInfoJson,
                                detailsPageUrl = "" // Bu alan Translation modelinde yok, sabit bırakılmış
                            }).Cast<object>().ToList()
                            : new List<object>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping activity with Id={id}", activity.Id);
                throw;
            }
        }
    }
}
