using System.Security.Cryptography;
using System.Text;

namespace TourManagementApi.Helper
{
    public  class AuthorizationHeaderHelper
    {
        public  string GenerateAuthorizationHeader(string publicKey, string secretKey, string requestId)
        {
            var base64RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestId));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64RequestId));
            var hmacHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            var combined = $"{publicKey}:{hmacHash}";
            var base64Combined = Convert.ToBase64String(Encoding.UTF8.GetBytes(combined));

            return $"Basic {base64Combined}";
        }

        public  string GenerateAuthHeader(string publicKey, string secretKey, string requestBody)
        {
            var base64Body = Base64UrlEncode(requestBody);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64Body));
                var hash = Convert.ToBase64String(hashBytes);

                var authString = $"{publicKey}:{hash}";
                var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));

                return $"Basic {authBase64}";
            }
        }

        private  string Base64UrlEncode(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var base64 = Convert.ToBase64String(bytes);

            base64 = base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');

            return base64;
        }
    }


}
