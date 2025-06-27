using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TourManagementApi.Helper;
using TourManagementApi.Models;

namespace TourManagementApi.Services
{
    public class ExperienceBankService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthorizationHeaderHelper _authHelper;
        private readonly ExperienceBankSettings _settings;
        private readonly ILogger<ExperienceBankService> _logger; 

        public ExperienceBankService(
        IHttpClientFactory httpClientFactory,
        AuthorizationHeaderHelper authHelper,
        IOptions<ExperienceBankSettings> settings,
        ILogger<ExperienceBankService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _authHelper = authHelper;
            _settings = settings.Value;
            _logger = logger;
        }

        private HttpRequestMessage CreateRequest(string requestBody, string method)
        {
            var authHeader = _authHelper.GenerateAuthHeader(
         _settings.PublicKey,
         _settings.SecretKey,
         requestBody);

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader.Substring(6));

            return request;
        }
        public async Task NotifyActivityUpdatedAsync(string activityId, string partnerSupplierId)
        {
            _logger.LogInformation("Notifying ExperienceBank activity.updated for ActivityId: {ActivityId}, PartnerSupplierId: {PartnerSupplierId}",
    activityId, partnerSupplierId);
            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "activity.updated",
                @params = new
                {
                    activityId = activityId,
                    partnerSupplierId = partnerSupplierId
                },
                id = (string)null
            });

            var request = CreateRequest(requestBody, "activity.updated");
            await SendRequestAsync(request);
        }
        public async Task NotifyAvailabilityUpdatedAsync(
            string supplierId,
            string activityId,
            string optionId,
            string localDateTime,
            int availableCapacity,
            int oldCapacity)
        {

            _logger.LogInformation("Calling NotifyAvailabilityUpdatedAsync with SupplierId: {SupplierId}, ActivityId: {ActivityId}, OptionId: {OptionId}, DateTime: {DateTime}, AvailableCapacity: {AvailableCapacity}, OldCapacity: {OldCapacity}",
                supplierId, activityId, optionId, localDateTime, availableCapacity, oldCapacity);


            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "availability.updated",
                @params = new
                {
                    supplierId = supplierId,
                    activityId = activityId,
                    optionId = optionId,
                    localDateTime = localDateTime,
                    availableCapacity = availableCapacity,
                    oldCapacity = oldCapacity
                },
                id = (string)null
            });

            var request = CreateRequest(requestBody, "availability.updated");
            await SendRequestAsync(request);
        }

        private async Task<JObject> SendRequestAsync(HttpRequestMessage request)
        {
            var client = _httpClientFactory.CreateClient();

            //  Request log
            _logger.LogInformation("Sending ExperienceBank request to {Url} with body: {Body}",
                request.RequestUri, await request.Content.ReadAsStringAsync());

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            //  Response log
            _logger.LogInformation("Received ExperienceBank response (StatusCode: {StatusCode}): {Response}",
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("ExperienceBank request failed: {Response}", responseContent);
                throw new Exception($"ExperienceBank request failed: {responseContent}");
            }

            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
            if (result["error"] != null)
            {
                _logger.LogError("ExperienceBank error: {Error}", result["error"]["message"]);
                throw new Exception($"ExperienceBank error: {result["error"]["message"]}");
            }

            return result;
        }


        public async Task NotifyActivityChangedAsync()
        {
            _logger.LogInformation("Calling NotifyActivityChangedAsync");
            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "activity.changed",
                @params = new
                {
                    supplierId = "par_8376ce5d-bc21-4243-907b-d7dc41168756",
                    activityIds = new[] { "12345" }
                },
                id = 1
            });

            var request = CreateRequest(requestBody, "activity.changed");
            await SendRequestAsync(request);
        }

        public async Task NotifyBookingCancelledAsync(string bookingId)
        {
            _logger.LogInformation("Calling NotifyBookingCancelledAsync with BookingId: {BookingId}", bookingId);

            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "booking.cancelled",
                @params = new { bookingId },
                id = (string)null
            });

            var request = CreateRequest(requestBody, "booking.cancelled");
            await SendRequestAsync(request);
        }

        public async Task NotifyTicketAffectedAsync(string ticketId, string status, DateTime date)
        {
            var isoDate = date.ToString("yyyy-MM-ddTHH:mm:sszzz");

               _logger.LogInformation("Calling NotifyTicketAffectedAsync with TicketId: {TicketId}, Status: {Status}, Date: {Date}",
                ticketId, status, isoDate);

            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "ticket.affected",
                @params = new
                {
                    ticketId,
                    status,
                    date = isoDate
                },
                id = (string)null
            });

            var request = CreateRequest(requestBody, "ticket.affected");
            await SendRequestAsync(request);
        }

        public async Task SubscribeWebhookAsync(string eventName, string webhookUrl)
        {
            _logger.LogInformation("Calling SubscribeWebhookAsync with EventName: {EventName}, WebhookUrl: {WebhookUrl}",
                eventName, webhookUrl);

            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "webhook.subscribe",
                @params = new
                {
                    @event = eventName,
                    url = webhookUrl
                },
                id = 1
            });

            var request = CreateRequest(requestBody, "webhook.subscribe");
            await SendRequestAsync(request);
        }

        public async Task UnsubscribeWebhookAsync(string webhookId)
        {
            _logger.LogInformation("Calling UnsubscribeWebhookAsync with WebhookId: {WebhookId}", webhookId);

            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "webhook.unsubscribe",
                @params = new { webhookId },
                id = 1
            });

            var request = CreateRequest(requestBody, "webhook.unsubscribe");
            await SendRequestAsync(request);
        }

        public async Task<JObject> FindWebhooksAsync()
        {
            _logger.LogInformation("Calling FindWebhooksAsync");
            var requestBody = JsonConvert.SerializeObject(new
            {
                jsonrpc = "2.0",
                method = "webhook.find",
                @params = new { },
                id = 1
            });

            var request = CreateRequest(requestBody, "webhook.find");
            return await SendRequestAsync(request);
        }

    }
}
