using Microsoft.EntityFrameworkCore;
using System;
using TourManagementApi.Data;
using TourManagementApi.Models;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public class AvailabilityService
    {
        private readonly TourManagementDbContext _context;

        public AvailabilityService(TourManagementDbContext context)
        {
            _context = context;
        }
//        public List<AvailabilitySessionResponse> GetSessions(string productCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
//        {
//            try
//            {
//                int activityId;
//                int optionId;
//                if (externalProductCode.Contains("-"))
//                {
//                    var id = externalProductCode.Split('-');
//                    activityId = int.Parse(id[0]);
//                    optionId = int.Parse(id[1]);
//                }
//                else
//                {
//                    return new List<AvailabilitySessionResponse>();
//                }

//                // 1) İlgili kayıtları tarihe göre kabaca çek (sadece gün aralığı)
//                // Not: StartTime nvarchar olduğundan SQL tarafında saat bazlı filtreleme yerine
//                // Date alanıyla gün aralığını daraltıyoruz.
//                var fromDate = fromUtc.Date;
//                var toDate = toUtc.Date;

//                var baseQuery = _context.Availabilities
//                    .Include(a => a.Option)
//                        .ThenInclude(o => o.TicketCategories)
//                    .Where(a => a.ActivityId == activityId
//                                && a.OptionId == optionId
//                                && a.Date >= fromDate
//                                && a.Date <= toDate)
//                    .AsEnumerable(); // Saat birleştirme için memory’e alacağız

//                // (Opsiyonel) Çalıştığınız sunucuya göre TZ seçin:
//                // Windows: "Turkey Standard Time"
//                // Linux/macOS: "Europe/Istanbul"
//                TimeZoneInfo tz;
//                try
//                {
//#if WINDOWS
//            tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
//#else
//                    tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
//#endif
//                }
//                catch
//                {
//                    tz = TimeZoneInfo.Utc; // bulunamazsa UTC'ye düş
//                }

//                // 2) Option.StartTime (nvarchar) -> TimeSpan parse
//                string[] formats = { "H\\:mm", "HH\\:mm", "H\\:m", "HH\\:m" };

//                DateTimeOffset ToStartDto(Availability a)
//                {
//                    // a.Date (date) ile saati birleştir
//                    if (a?.Option?.StartTime == null)
//                        return new DateTimeOffset(a.Date, TimeSpan.Zero); // saat yoksa gün 00:00 UTC

//                    if (!TimeSpan.TryParseExact(a.Option.StartTime.Trim(), formats, null, out var tspan))
//                        return new DateTimeOffset(a.Date, TimeSpan.Zero); // parse edemediysek 00:00

//                    // Yerel (İstanbul) tarihe saati ekleyip UTC'ye çevir
//                    // a.Date (DATE) -> 00:00 local, üstüne tspan ekle
//                    var localDateTime = new DateTime(a.Date.Year, a.Date.Month, a.Date.Day, 0, 0, 0, DateTimeKind.Unspecified)
//                                        .Add(tspan);
//                    var localDto = new DateTimeOffset(localDateTime, tz.GetUtcOffset(localDateTime));
//                    return localDto.ToUniversalTime(); // UTC DTO
//                }

//                // 3) Saat birleştir, gerçek aralığa göre filtrele
//                var withComputedTime = baseQuery
//                    .Select(a => new
//                    {
//                        Av = a,
//                        StartDto = ToStartDto(a)
//                    })
//                    .Where(x => x.StartDto.UtcDateTime >= fromUtc && x.StartDto.UtcDateTime <= toUtc)
//                    .ToList();

//                // 4) Response üret
//                var result = withComputedTime.Select(x => new AvailabilitySessionResponse
//                {
//                    ProductCode = productCode,
//                    SessionCode = $"S{x.Av.Id}",
//                    StartTime = x.StartDto.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
//                    EndTime = x.StartDto.UtcDateTime.AddMinutes(60).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
//                    CutoffUnit = "HOURS",
//                    CutoffWindowDuration = x.Av.Option.CutOff,
//                    StartTimeLocal = TimeZoneInfo.ConvertTime(x.StartDto, tz).DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
//                    EndTimeLocal = TimeZoneInfo.ConvertTime(x.StartDto.AddMinutes(60), tz).DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
//                    LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"),
//                    Seats = x.Av.MaximumCapacity,
//                    SeatsAvailable = x.Av.AvailableCapacity,
//                    PriceOptions = x.Av.Option.TicketCategories.Select(tc => new PriceOptionLite
//                    {
//                        Label = tc.Type,
//                        Price = tc.Amount
//                    }).ToList()
//                }).ToList();

//                return result;
//            }
//            catch
//            {
//                throw;
//            }
//        }

        public List<AvailabilitySessionResponse> GetSessions(string productCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
        {
            try
            {
                int activityId;
                int optionId;
                if (externalProductCode.Contains("-"))
                {
                    var id = externalProductCode.Split('-');
                    activityId = int.Parse(id[0]);
                    optionId= int.Parse(id[1]);
                }
                else
                {
                    return new List<AvailabilitySessionResponse>();
                }

                


                var query = _context.Availabilities
                    .Include(a => a.Option)
                        .ThenInclude(o => o.TicketCategories)
                    .Where(a =>
                        a.ActivityId == activityId && 
                        a.OptionId== optionId &&
                        a.StartTime >= fromUtc &&
                        a.StartTime <= toUtc)
                    .ToList();


                var result = query.Select(a => new AvailabilitySessionResponse
                {
                    ProductCode = productCode,
                    SessionCode = $"S{a.Id}",
                    StartTime = a.StartTime?.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                    EndTime = a.StartTime?.UtcDateTime.AddMinutes(60).ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                    CutoffUnit = "HOURS", // Sabit örnek
                    CutoffWindowDuration = a.Option.CutOff, // Örnek: 24 saat önceden kapanır
                    StartTimeLocal = a.StartTime?.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    EndTimeLocal = a.StartTime?.LocalDateTime.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                    LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Seats = a.MaximumCapacity,
                    SeatsAvailable = a.AvailableCapacity//,
                    //PriceOptions = a.Option.TicketCategories.Where(a=>a.SalePrice.HasValue).Select(tc => new PriceOptionLite
                    //{
                    //    Label = tc.Type,
                    //    Price = tc.SalePrice.Value
                    //}).ToList()
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
