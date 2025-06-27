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

        public ExperienceBankService(
        IHttpClientFactory httpClientFactory,
        AuthorizationHeaderHelper authHelper,
        IOptions<ExperienceBankSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _authHelper = authHelper;
            _settings = settings.Value;
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
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"ExperienceBank request failed: {responseContent}");

            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
            if (result["error"] != null)
                throw new Exception($"ExperienceBank error: {result["error"]["message"]}");

            return result;
        }

        public async Task NotifyActivityChangedAsync()
        {
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
    }
}



//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using TourManagementApi.Helper;
//using TourManagementApi.Models;
//namespace TourManagementApi.Services
//{
//    public class ExperienceBankService
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly AuthorizationHeaderHelper _authHelper;
//        private readonly ExperienceBankSettings _settings;

//        public ExperienceBankService(
//            IHttpClientFactory httpClientFactory,
//            AuthorizationHeaderHelper authHelper,
//            IOptions<ExperienceBankSettings> settings)
//        {
//            _httpClientFactory = httpClientFactory;
//            _authHelper = authHelper;
//            _settings = settings.Value;
//        }

//        private HttpRequestMessage CreateRequest(string requestBody, string method)
//        {
//            var authHeader = AuthorizationHeaderHelper.GenerateAuthHeader(
//                _settings.PublicKey,
//                _settings.SecretKey,
//                requestBody);

//            var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl)
//            {
//                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
//            };
//            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            return request;
//        }

//        private async Task<JObject> SendRequestAsync(HttpRequestMessage request)
//        {
//            var client = _httpClientFactory.CreateClient();
//            var response = await client.SendAsync(request);
//            var responseContent = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//                throw new Exception($"ExperienceBank request failed: {responseContent}");

//            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
//            if (result["error"] != null)
//                throw new Exception($"ExperienceBank error: {result["error"]["message"]}");

//            return result;
//        }

//        public async Task NotifyActivityChangedAsync()
//        {
//            var publicKey = "pub_f7bb2323480b5c4f825410424a676247bd7579fcff";
//            var secretKey = "sec_a0ea4f1497499bed4cbbf2ce4f1a73acd7a6d2fbe1";

//            var requestBody = JsonConvert.SerializeObject(new
//            {
//                jsonrpc = "2.0",
//                method = "activity.changed",
//                @params = new
//                {
//                    supplierId = "par_8376ce5d-bc21-4243-907b-d7dc41168756",
//                    activityIds = new[] { "12345" }
//                },
//                id = 1
//            });
//            AuthorizationHeaderHelper hlp= new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1");
//            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
//            request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);

//            var response = await _httpClient.SendAsync(request);
//            response.EnsureSuccessStatusCode();

//            var responseContent = await response.Content.ReadAsStringAsync();
//            // Handle response if needed
//        }

//        public async Task NotifyAvailabilityUpdatedAsync(string supplierId, string activityId, string optionId, string localDateTime, int availableCapacity, int oldCapacity)
//        {
//            var publicKey = "pub_..."; // configden al
//            var secretKey = "sec_...";

//            var requestBodyObj = new
//            {
//                jsonrpc = "2.0",
//                method = "availability.updated",
//                @params = new
//                {
//                    supplierId = supplierId,
//                    activityId = activityId,
//                    optionId = optionId,
//                    localDateTime = localDateTime,
//                    availableCapacity = availableCapacity,
//                    oldCapacity = oldCapacity
//                },
//                id = (string)null
//            };

//            var requestBody = JsonConvert.SerializeObject(requestBodyObj);

//            AuthorizationHeaderHelper hlp = new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1")
//            {
//                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
//            };
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            var response = await _httpClient.SendAsync(request);
//            var responseContent = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"ExperienceBank availability.updated failed: {responseContent}");
//            }

//            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
//            if (result["error"] != null)
//            {
//                throw new Exception($"ExperienceBank returned error: {result["error"]["message"]}");
//            }
//        }



//        public async Task NotifyActivityUpdatedAsync(string activityId, string partnerSupplierId)
//        {
//            var publicKey = "pub_..."; // Config veya Secret Manager'dan al
//            var secretKey = "sec_...";

//            var requestBodyObj = new
//            {
//                jsonrpc = "2.0",
//                method = "activity.updated",
//                @params = new
//                {
//                    activityId = activityId,
//                    partnerSupplierId = partnerSupplierId
//                },
//                id = (string)null
//            };

//            var requestBody = JsonConvert.SerializeObject(requestBodyObj);

//            AuthorizationHeaderHelper hlp = new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1");
//            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            var response = await _httpClient.SendAsync(request);

//            var responseContent = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                // Log failure or throw exception as needed
//                throw new Exception($"ExperienceBank notification failed: {responseContent}");
//            }

//            // Optional: Parse response to check for "result" or "error"
//            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
//            if (result["error"] != null)
//            {
//                throw new Exception($"ExperienceBank returned error: {result["error"]["message"]}");
//            }
//        }

//        public async Task NotifyBookingCancelledAsync(string bookingId)
//        {
//            var publicKey = "pub_..."; // configden al
//            var secretKey = "sec_...";

//            var requestBodyObj = new
//            {
//                jsonrpc = "2.0",
//                method = "booking.cancelled",
//                @params = new
//                {
//                    bookingId = bookingId
//                },
//                id = (string)null
//            };

//            var requestBody = JsonConvert.SerializeObject(requestBodyObj);

//            AuthorizationHeaderHelper hlp = new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1");
//            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            var response = await _httpClient.SendAsync(request);

//            var responseContent = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                // log failure or throw exception as needed
//                throw new Exception($"ExperienceBank booking.cancelled failed: {responseContent}");
//            }

//            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
//            if (result["error"] != null)
//            {
//                throw new Exception($"ExperienceBank returned error: {result["error"]["message"]}");
//            }
//        }
//        public async Task NotifyTicketAffectedAsync(string ticketId, string status, DateTime date)
//        {
//            var publicKey = "pub_..."; // configden al
//            var secretKey = "sec_...";

//            var isoDate = date.ToString("yyyy-MM-ddTHH:mm:sszzz");

//            var requestBodyObj = new
//            {
//                jsonrpc = "2.0",
//                method = "ticket.affected",
//                @params = new
//                {
//                    ticketId = ticketId,
//                    status = status, // created, cancelled, redeemed
//                    date = isoDate
//                },
//                id = (string)null
//            };

//            var requestBody = JsonConvert.SerializeObject(requestBodyObj);

//            AuthorizationHeaderHelper hlp = new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1");
//            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            var response = await _httpClient.SendAsync(request);

//            var responseContent = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"ExperienceBank ticket.affected failed: {responseContent}");
//            }

//            var result = JsonConvert.DeserializeObject<JObject>(responseContent);
//            if (result["error"] != null)
//            {
//                throw new Exception($"ExperienceBank returned error: {result["error"]["message"]}");
//            }
//        }

//        public async Task SubscribeWebhookAsync(string eventName, string webhookUrl)
//        {
//            var publicKey = "pub_..."; // configden al
//            var secretKey = "sec_...";

//            var requestBodyObj = new
//            {
//                jsonrpc = "2.0",
//                method = "webhook.subscribe",
//                @params = new
//                {
//                    @event = eventName,
//                    url = webhookUrl
//                },
//                id = 1
//            };

//            var requestBody = JsonConvert.SerializeObject(requestBodyObj);

//            AuthorizationHeaderHelper hlp = new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var client = _httpClientFactory.CreateClient(); // IHttpClientFactory kullan
//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1")
//            {
//                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
//            };
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            var response = await client.SendAsync(request);
//            var content = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"Webhook Subscribe failed: {content}");
//            }

//            var result = JsonConvert.DeserializeObject<JObject>(content);
//            if (result["error"] != null)
//            {
//                throw new Exception($"ExperienceBank error: {result["error"]["message"]}");
//            }
//        }

//        public async Task UnsubscribeWebhookAsync(string webhookId)
//        {
//            var publicKey = "pub_..."; // configden al
//            var secretKey = "sec_...";

//            var requestBodyObj = new
//            {
//                jsonrpc = "2.0",
//                method = "webhook.unsubscribe",
//                @params = new
//                {
//                    webhookId = webhookId
//                },
//                id = 1
//            };

//            var requestBody = JsonConvert.SerializeObject(requestBodyObj);

//            AuthorizationHeaderHelper hlp = new AuthorizationHeaderHelper();
//            var authHeader = hlp.GenerateAuthHeader(publicKey, secretKey, requestBody);

//            var client = _httpClientFactory.CreateClient(); // IHttpClientFactory kullan
//            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.experiencebank.io/v1")
//            {
//                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
//            };
//            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader.Substring(6));

//            var response = await client.SendAsync(request);
//            var content = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"Webhook Unsubscribe failed: {content}");
//            }

//            var result = JsonConvert.DeserializeObject<JObject>(content);
//            if (result["error"] != null)
//            {
//                throw new Exception($"ExperienceBank error: {result["error"]["message"]}");
//            }

//            Console.WriteLine("Webhook unsubscribed successfully.");
//        }


//    }

//}
