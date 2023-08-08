using System.Text.Json;
using System.Text.Json.Serialization;

namespace StsGateway.Models
{
    public class OAuth2Entity
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        public static OAuth2Entity? FromJsonString(string json)
        {
            return JsonSerializer.Deserialize<OAuth2Entity>(json);
        }
    }
}
