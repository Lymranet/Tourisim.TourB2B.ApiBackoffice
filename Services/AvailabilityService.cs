using Microsoft.EntityFrameworkCore;
using System;
using TourManagementApi.Data;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public class AvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }


        //public List<AvailabilitySessionResponse> GetSessions(string productCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
        //{
        //    if (!int.TryParse(externalProductCode, out int activityId))
        //        return new List<AvailabilitySessionResponse>();

        //    var sessions = _context.Availabilities
        //        .Where(a => a.ActivityId == activityId &&
        //                    a.StartTime.HasValue &&
        //                    a.StartTime.Value.UtcDateTime >= fromUtc &&
        //                    a.StartTime.Value.UtcDateTime <= toUtc)
        //        .Select(a => new AvailabilitySessionResponse
        //        {
        //            StartTime = a.StartTime.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
        //            EndTime = a.StartTime.Value.UtcDateTime.AddHours(2).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
        //            AllDay = false,
        //            Seats = a.MaximumCapacity,
        //            SeatsAvailable = a.AvailableCapacity,
        //            PriceOptions = a.Option.TicketCategories.Select(tc => new PriceOption
        //            {
        //                Label = tc.Type,
        //                Price = tc.Amount
        //            }).ToList()
        //        })
        //        .ToList();

        //    return sessions;
        //}



        //public List<AvailabilitySessionResponse> GetSessions(string productCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
        //{
        //    if (!int.TryParse(externalProductCode, out int activityId))
        //        return new List<AvailabilitySessionResponse>();

        //    var tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

        //    // Veritabanındaki Date + StartTime'ı UTC'ye çevirerek filtre uygula
        //    var query = _context.Availabilities
        //        .Include(a => a.Option)
        //            .ThenInclude(o => o.TicketCategories)
        //        .AsEnumerable() // Date + StartTime hesaplaması için in-memory çalışmalı
        //        .Where(a =>
        //        {
        //            if (a.StartTime == null)
        //                return false;

        //            var timeOnly = TimeOnly.FromTimeSpan(a.StartTime.Value.LocalDateTime.TimeOfDay);
        //            // Date + StartTime saat bilgisi birleştiriliyor
        //            var localDateTime = a.Date.ToDateTime(timeOnly);

        //            // UTC'ye çevriliyor
        //            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, tz);

        //            return utcDateTime >= fromUtc && utcDateTime <= toUtc;
        //        })
        //        .ToList();

        //    // Result dönüşü
        //    var result = query.Select(a =>
        //    {
        //        var localStart = a.Date.ToDateTime(
        //                TimeOnly.FromTimeSpan(a.StartTime.Value.LocalDateTime.TimeOfDay)
        //            );
        //        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
        //        var endUtc = startUtc.AddMinutes(60); // Örnek: 1 saatlik seans

        //        var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tz);
        //        var endLocal = startLocal.AddMinutes(60);

        //        return new AvailabilitySessionResponse
        //        {
        //            ProductCode = productCode,
        //            SessionCode = $"S{a.Id}",

        //            StartTime = startUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        //            EndTime = endUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),

        //            StartTimeLocal = startLocal.ToString("yyyy-MM-dd HH:mm:ss"),
        //            EndTimeLocal = endLocal.ToString("yyyy-MM-dd HH:mm:ss"),

        //            Seats = a.MaximumCapacity,
        //            SeatsAvailable = a.AvailableCapacity,

        //            AllDay = false, // Gerekirse dinamik yap
        //            CutoffUnit = "HOURS", // Sabit örnek
        //            CutoffWindowDuration = 24, // Örnek: 24 saat önceden kapanır
        //            LastUpdated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),

        //            PriceOptions = a.Option.TicketCategories.Select(tc => new PriceOption
        //            {
        //                Label = tc.Type,
        //                Price = tc.Amount
        //            }).ToList()
        //        };
        //    }).ToList();

        //    return result;
        //}
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
                    SeatsAvailable = a.AvailableCapacity,
                    PriceOptions = a.Option.TicketCategories.Select(tc => new PriceOption
                    {
                        Label = tc.Type,
                        //Currency = tc.Currency,
                        Price = tc.Amount
                    }).ToList()
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public List<AvailabilitySessionResponse> GetSessions2(string productCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
        {
            if (!int.TryParse(externalProductCode, out int activityId))
                return new List<AvailabilitySessionResponse>();

            var tz = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            var fromDateOnly = DateOnly.FromDateTime(fromUtc);
            var toDateOnly = DateOnly.FromDateTime(toUtc);
            var query = _context.Availabilities
                 .Include(a => a.Option)
                     .ThenInclude(o => o.TicketCategories)
                 .Where(a => a.ActivityId == activityId && a.Date >= fromDateOnly && a.Date <= toDateOnly)
                 .ToList();


            //var query = _context.Availabilities
            //    .Include(a => a.Option)
            //        .ThenInclude(o => o.TicketCategories)
            //    .AsEnumerable() // EF Core'da Date + StartTime hesaplaması için in-memory geçiş
            //    .Where(a =>
            //    {
            //        if (a.StartTime == null)
            //            return false;

            //        var combinedDateTime = new DateTime(
            //            a.Date.Year, a.Date.Month, a.Date.Day,
            //            a.StartTime.Value.Hour, a.StartTime.Value.Minute, a.StartTime.Value.Second,
            //            DateTimeKind.Local); // Veritabanındaki zaman yerel zamansa

            //        var utcDateTime = combinedDateTime.ToUniversalTime();
            //        return utcDateTime >= fromUtc && utcDateTime <= toUtc;
            //    })
            //.ToList();

            //var result = query.Select(a =>
            //{
            //    var startUtc = a.StartTime?.UtcDateTime ?? DateTime.MinValue;
            //    var endUtc = startUtc.AddMinutes(60); // 1 saatlik örnek süre

            //    var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tz);
            //    var endLocal = startLocal.AddMinutes(60);

            //    return new AvailabilitySessionResponse
            //    {
            //        ProductCode = productCode,
            //        SessionCode = $"S{a.Id}",

            //        StartTime = startUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            //        EndTime = endUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),

            //        StartTimeLocal = startLocal.ToString("yyyy-MM-dd HH:mm:ss"),
            //        EndTimeLocal = endLocal.ToString("yyyy-MM-dd HH:mm:ss"),

            //        Seats = a.MaximumCapacity,
            //        SeatsAvailable = a.AvailableCapacity,

            //        PriceOptions = a.Option.TicketCategories.Select(tc => new PriceOption
            //        {
            //            Label = tc.Type,
            //            Price = tc.Amount
            //        }).ToList()
            //    };
            //}).ToList();

            //return result;



            var result = query.Select(a => new AvailabilitySessionResponse
            {
                ProductCode = productCode,
                SessionCode = $"S{a.Id}",
                StartTime = a.StartTime?.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",
                EndTime = a.StartTime?.UtcDateTime.AddMinutes(60).ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "",

                StartTimeLocal = a.StartTime?.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                EndTimeLocal = a.StartTime?.LocalDateTime.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss") ?? "",

                Seats = a.MaximumCapacity,
                SeatsAvailable = a.AvailableCapacity,
                PriceOptions = a.Option.TicketCategories.Select(tc => new PriceOption
                {
                    Label = tc.Type,
                    //Currency = tc.Currency,
                    Price = tc.Amount
                }).ToList()
            }).ToList();

            return result;
        }
    }
}
