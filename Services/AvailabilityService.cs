using Microsoft.EntityFrameworkCore;
using System;
using TourManagementApi.Data;
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
                    PriceOptions = a.Option.TicketCategories.Select(tc => new PriceOptionLite
                    {
                        Label = tc.Type,
                        Price = tc.SalePrice ?? tc.Amount * 1.5,
                    }).ToList()
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
