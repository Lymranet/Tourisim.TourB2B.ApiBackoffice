using System.Text.Json;

namespace TourManagementApi.Helper
{
    public class TxtJson
    {
        public static List<string> DeserializeStringList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static List<int> DeserializeIntList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<int>();
            try
            {
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch
            {
                return new List<int>(); 
            }
        }
        public static string SerializeStringList(List<string>? list)
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
        public static string SerializeIntList(List<int>? list)
        {
            if (list == null || list.Count == 0) return string.Empty;
            try
            {
                return JsonSerializer.Serialize(list.Where(x => !string.IsNullOrWhiteSpace(x.ToString())).ToList());
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
