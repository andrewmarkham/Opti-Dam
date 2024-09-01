using Microsoft.Extensions.Options;
using OptiDAM.Services.Models;
using Optimizely.Cmp.Client;
using System.Net.Http.Json;

namespace OptiDAM.Services
{
    public class OptiDamAuthService : IOptiDamAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<CmpClientOptions> _options;
        private string? _accessToken = null;
        private DateTime _accessTokenExpiration = DateTime.MinValue;

        public OptiDamAuthService(HttpClient httpClient, IOptions<CmpClientOptions> options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<string?> GetBearerToken()  {

            if (_accessToken != null && DateTime.Now <= _accessTokenExpiration)
            {
                return _accessToken;
            }

            var authResponse = await Authenticate();

            if (authResponse != null)
            {
                _accessToken = authResponse.AccessToken ?? string.Empty;
                _accessTokenExpiration = DateTime.Now.AddSeconds((authResponse.ExpiresIn - 10) ?? 0);
            }

            return _accessToken;
        }

        private async Task<AuthenticationResponse?> Authenticate()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.TokenUrl)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"},
                    {"client_id", _options?.Value?.ClientId ?? string.Empty},
                    {"client_secret", _options?.Value?.ClientSecret ?? string.Empty},
                })
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }
    }
}