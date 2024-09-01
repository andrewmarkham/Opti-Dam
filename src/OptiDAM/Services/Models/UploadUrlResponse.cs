using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class UploadUrlResponse {
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("upload_meta_fields")]
        public UploadUrlMetaFields? UploadMetaFields { get; set; }
    }
}