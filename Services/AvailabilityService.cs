using TourManagementApi.Models.Api.RezdyConnectModels;
using TourManagementApi.Data;
using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Services
{
    public class AvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AvailabilitySessionResponse> GetSessions(string productCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
        {
            int activityId;
            if (!int.TryParse(externalProductCode, out activityId))
                return new List<AvailabilitySessionResponse>();

            var query = _context.Availabilities
                .Include(a => a.Option)
                .Where(a =>
                    a.ActivityId == activityId &&
                    a.StartTime >= fromUtc &&
                    a.StartTime <= toUtc)
                .ToList();

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
                    Currency = tc.Currency,
                    Price = tc.Amount
                }).ToList()
            }).ToList();

            return result;
        }
    }
}
