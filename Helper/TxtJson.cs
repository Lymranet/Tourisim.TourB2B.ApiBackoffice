using System.Text.Json;

namespace TourManagementApi.Helper
{
    public class TxtJson
    {
        public static List<string> DeserializeList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>(); // hatalı json varsa boş liste dön
            }
        }
        public static string SerializeList(List<string>? list)
        {
            if (list == null || list.Count == 0) return string.Empty;
            try
            {
                return JsonSerializer.Serialize(list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
