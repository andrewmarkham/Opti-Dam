using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class PostAssetRequest {
        public string Key { get; set; }
        public string Title { get; set; }

        [JsonPropertyName("folder_id")]
        public string? FolderId { get; set; } = null;// = "75066a38543d11ef89c5b2570a3a6e3c";
    }
}

