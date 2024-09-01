using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class PostAssetContentResponse {
        public string Type { get; set; }
        public string Value { get; set; }

        public List<string> Labels { get; set; } = new List<string>();

        [JsonPropertyName("folder_id")]     
        public string? FolderId { get; set; }

        [JsonPropertyName("file_location")]

        public string FileLocation { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }


        [JsonPropertyName("owner_organization_id")]
        public string OwnerOrganizationId { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }


        [JsonPropertyName("file_extension")]    
        public string FileExtension { get; set; }

        public PostAssetLinksResponse Links { get; set; }

    }
}