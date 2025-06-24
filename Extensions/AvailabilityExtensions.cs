using TourManagementApi.Models;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Extensions
{
    public static class AvailabilityExtensions
    {
        public static AvailabilityResponseDto ToDto(this Availability availability)
        {
            return new AvailabilityResponseDto
            {
                dateTime = availability.StartTime.HasValue
                    ? availability.StartTime.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")
                    : availability.Date.ToString("yyyy-MM-ddT00:00:00zzz"),
                openingHours = availability.OpeningHours?.Select(oh => new OpeningHourDto
                {
                    fromTime = oh.FromTime,
                    toTime = oh.ToTime
                }).ToList() ?? new List<OpeningHourDto>(),
                availableCapacity = availability.AvailableCapacity,
                maximumCapacity = availability.MaximumCapacity,
                activityId = availability.ActivityId,
                optionId = availability.OptionId,
                ticketCategories = availability.TicketCategoryCapacities?.Select(tc => new TicketCategoryDto
                {
                    id = tc.TicketCategoryId,
                    availableCapacity = tc.Capacity
                }).ToList() ?? new List<TicketCategoryDto>()
            };
        }
    }
}
