using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Data;
using TourManagementApi.Models;

namespace TourManagementApi.Controllers.Api
{
    [Route("supplier/{partnerSupplierId}/activities")]
    [ApiController]
    public class ExperienceBankActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExperienceBankActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetActivities(string partnerSupplierId, [FromQuery] string[] id, [FromQuery] int offset = 0)
        //{
        //    // Sadece belirli ID'lerle filtrelenmiş tur listesi
        //    var query = _context.Activities
        //        .Include(a => a.Options)
        //        .Include(a => a.MeetingPoints)
        //        .Include(a => a.RoutePoints)
        //        .Include(a => a.GuestFields)
        //        .Include(a => a.Pricing)
        //        .Include(a => a.SeasonalAvailability)
        //        .Where(a => a.PartnerSupplierId == partnerSupplierId);

        //    if (id?.Any() == true)
        //    {
        //        query = query.Where(a => id.Contains(a.Id.ToString()));
        //    }

        //    const int pageSize = 50;
        //    var total = await query.CountAsync();
        //    var activities = await query
        //        .Skip(offset)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var result = new
        //    {
        //        links = new
        //        {
        //            next = offset + pageSize < total
        //                ? $"https://your-api.com/supplier/{partnerSupplierId}/activities?offset={offset + pageSize}"
        //                : null
        //        },
        //        data = activities.Select(MapToExperienceBankDto)
        //    };

        //    return Ok(result);
        //}

        //private object MapToExperienceBankDto(Activity activity)
        //{
        //    return new
        //    {
        //        id = activity.Id.ToString(),
        //        title = activity.Title,
        //        description = activity.Description,
        //        highlights = activity.Highlights,
        //        itinerary = activity.Itinerary,
        //        language = activity.Language ?? "en",
        //        inclusions = activity.Inclusions ?? new List<string>(),
        //        exclusions = activity.Exclusions ?? new List<string>(),
        //        importantInfo = activity.ImportantInfo ?? new List<string>(),
        //        detailsPageUrl = activity.DetailsUrl,
        //        destination = new
        //        {
        //            countryCode = activity.CountryCode,
        //            code = activity.DestinationCode,
        //            name = activity.DestinationName
        //        },
        //        media = new
        //        {
        //            images = new
        //            {
        //                header = activity.CoverImage,
        //                teaser = activity.PreviewImage,
        //                gallery = activity.GalleryImages ?? new List<string>()
        //            },
        //            videos = activity.VideoUrls ?? new List<string>()
        //        },
        //        rating = activity.Rating != null ? new
        //        {
        //            averageValue = activity.Rating.AverageValue,
        //            totalCount = activity.Rating.TotalCount
        //        } : null,
        //        categories = activity.Categories ?? new List<string>(),
        //        meetingPoints = activity.MeetingPoints?.Select(mp => new
        //        {
        //            name = mp.Name,
        //            latitude = mp.Latitude,
        //            longitude = mp.Longitude,
        //            address = mp.Address
        //        }) ?? new List<object>(),
        //        route = activity.RoutePoints?.Select(rp => new
        //        {
        //            name = rp.Name,
        //            latitude = rp.Latitude,
        //            longitude = rp.Longitude
        //        }) ?? new List<object>(),
        //        guestFields = activity.GuestFields?.Select(gf => new
        //        {
        //            code = gf.Code,
        //            label = gf.Label,
        //            type = gf.Type,
        //            required = gf.Required,
        //            options = gf.Options?.Select(opt => new
        //            {
        //                key = opt.Key,
        //                value = opt.Value,
        //                translations = opt.Translations?.Select(tr => new
        //                {
        //                    language = tr.Language,
        //                    value = tr.Value
        //                }) ?? new List<object>()
        //            }) ?? new List<object>(),
        //            translations = gf.Translations?.Select(tr => new
        //            {
        //                language = tr.Language,
        //                label = tr.Label
        //            }) ?? new List<object>()
        //        }) ?? new List<object>(),
        //        addons = activity.Addons?.Select(ad => new
        //        {
        //            id = ad.Id,
        //            title = ad.Title,
        //            description = ad.Description,
        //            type = ad.Type,
        //            price = new
        //            {
        //                amount = ad.PriceAmount.ToString("0.00"),
        //                currency = ad.Currency
        //            },
        //            translations = ad.Translations?.Select(tr => new
        //            {
        //                language = tr.Language,
        //                title = tr.Title,
        //                description = tr.Description
        //            }) ?? new List<object>()
        //        }) ?? new List<object>(),
        //        isActive = activity.Status == "active",
        //        cancellationPolicy = activity.CancellationPolicy != null ? new
        //        {
        //            isFreeCancellation = activity.CancellationPolicy.IsFreeCancellation,
        //            refundConditions = activity.CancellationPolicy.RefundConditions.Select(rc => new
        //            {
        //                minDurationBeforeStartTimeSec = rc.MinSecondsBeforeStart,
        //                refundPercentage = rc.RefundPercent
        //            })
        //        } : null,
        //        options = activity.Options.Select(op => new
        //        {
        //            id = op.Id,
        //            name = op.Name,
        //            duration = op.DurationISO8601,
        //            startTime = op.StartTime ?? "00:00:00",
        //            openingHours = op.OpeningHours?.Select(oh => new
        //            {
        //                fromTime = oh.FromTime,
        //                toTime = oh.ToTime
        //            }) ?? new List<object>(),
        //            fromDate = op.FromDate,
        //            untilDate = op.UntilDate,
        //            cutOff = op.CutOff,
        //            weekdays = op.Weekdays,
        //            canBeBookedAfterStartTime = op.CanBeBookedAfterStartTime,
        //            ticketCategories = op.TicketCategories.Select(tc => new
        //            {
        //                id = tc.Id,
        //                name = tc.Name,
        //                minSeats = tc.MinSeats,
        //                maxSeats = tc.MaxSeats,
        //                price = new
        //                {
        //                    type = tc.PriceType,
        //                    amount = tc.PriceAmount.ToString("0.00"),
        //                    currency = tc.Currency
        //                },
        //                type = tc.Type,
        //                ageLimit = tc.AgeLimit != null ? new
        //                {
        //                    minAge = tc.AgeLimit.MinAge,
        //                    maxAge = tc.AgeLimit.MaxAge
        //                } : null,
        //                translations = tc.Translations?.Select(tr => new
        //                {
        //                    language = tr.Language,
        //                    name = tr.Name
        //                }) ?? new List<object>()
        //            }),
        //            translations = op.Translations?.Select(tr => new
        //            {
        //                language = tr.Language,
        //                name = tr.Name
        //            }) ?? new List<object>()
        //        }),
        //        translations = activity.Translations?.Select(tr => new
        //        {
        //            language = tr.Language,
        //            title = tr.Title,
        //            description = tr.Description,
        //            highlights = tr.Highlights,
        //            itinerary = tr.Itinerary,
        //            inclusions = tr.Inclusions,
        //            exclusions = tr.Exclusions,
        //            importantInfo = tr.ImportantInfo,
        //            detailsPageUrl = tr.DetailsPageUrl
        //        }) ?? new List<object>()
        //    };
        //}
    }

}
