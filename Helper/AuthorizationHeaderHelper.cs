using System.Security.Cryptography;
using System.Text;

namespace TourManagementApi.Helper
{
    public static class AuthorizationHeaderHelper
    {
        public static string GenerateAuthorizationHeader(string publicKey, string secretKey, string requestId)
        {
            var base64RequestId = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestId));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(base64RequestId));
            var hmacHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            var combined = $"{publicKey}:{hmacHash}";
            var base64Combined = Convert.ToBase64String(Encoding.UTF8.GetBytes(combined));

            return $"Basic {base64Combined}";
        }
    }


}
