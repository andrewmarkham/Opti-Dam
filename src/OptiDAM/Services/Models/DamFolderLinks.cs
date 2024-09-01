using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class DamFolderLinks {
        public string Self { get; set; }

        [JsonPropertyName("parent_folder")]
        public string ParentFolder { get; set; }

        [JsonPropertyName("child_folders")]
        public string ChildFolders { get; set; }

        public string Assets { get; set; }
    }
}

