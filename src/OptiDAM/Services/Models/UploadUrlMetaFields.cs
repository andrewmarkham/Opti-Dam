using System.Text.Json.Serialization;

namespace OptiDAM.Services.Models
{
    public class UploadUrlMetaFields {
        public string Key { get; set; } = string.Empty;
        public string? Policy { get; set; }


        [JsonPropertyName("x-amz-algorithm")]
        public string? Algorithm { get; set; }

        [JsonPropertyName("x-amz-credential")]
        public string? Credential { get; set; }

        [JsonPropertyName("x-amz-date")]
        public string? Date { get; set; }

        [JsonPropertyName("x-amz-security-token")]
        public string? SecurityToken { get; set; }

        [JsonPropertyName("x-amz-signature")]
        public string? Signature { get; set; }
    }
}