using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class DamFolder {
        public string Id { get; set; }
        public string Name { get; set; }

        [JsonPropertyName("parent_folder_id")]
        public string ParentFolderId { get; set; }

        public string Path { get; set; }


        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime ModifiedAt { get; set; }
        public DamFolderLinks Links { get; set; }
    }

    public class DamFolderResponse {
        public List<DamFolder> Data { get; set; }
    }

    public class DamFolderRequest {
        public string Name { get; set; }

        [JsonPropertyName("parent_folder_id")]
        public string? ParentFolderId { get; set; }
    }
}

