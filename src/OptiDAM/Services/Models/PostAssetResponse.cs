using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class PostAssetResponse {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime ModifiedAt { get; set; }

        public PostAssetContentResponse Content { get; set; }    
    }
}