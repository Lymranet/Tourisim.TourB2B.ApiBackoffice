using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TourManagementApi.Models.ViewModels
{
    public class GuestFieldsViewModel
    {
        public int ActivityId { get; set; }
        public List<GuestField> GuestFields { get; set; } = new List<GuestField>();

        public class GuestField
        {
            [JsonPropertyName("code")]
            public string Code { get; set; } = string.Empty;

            [JsonPropertyName("label")]
            public string Label { get; set; } = string.Empty;

            [JsonPropertyName("type")]
            public string Type { get; set; } = "text";

            [JsonPropertyName("required")]
            public bool Required { get; set; }

            [JsonPropertyName("options")]
            public List<GuestFieldOption> Options { get; set; } = new List<GuestFieldOption>();

            [JsonPropertyName("translations")]
            public List<GuestFieldTranslation> Translations { get; set; } = new List<GuestFieldTranslation>();
        }

        public class GuestFieldOption
        {
            [JsonPropertyName("key")]
            public string Key { get; set; } = string.Empty;

            [JsonPropertyName("value")]
            public string Value { get; set; } = string.Empty;

            [JsonPropertyName("translations")]
            public List<GuestFieldOptionTranslation> Translations { get; set; } = new List<GuestFieldOptionTranslation>();
        }

        public class GuestFieldTranslation
        {
            [JsonPropertyName("language")]
            public string Language { get; set; } = string.Empty;

            [JsonPropertyName("label")]
            public string Label { get; set; } = string.Empty;
        }

        public class GuestFieldOptionTranslation
        {
            [JsonPropertyName("language")]
            public string Language { get; set; } = string.Empty;

            [JsonPropertyName("value")]
            public string Value { get; set; } = string.Empty;

            [JsonPropertyName("label")]
            public string Label { get; set; } = string.Empty;
        }
    }
} 