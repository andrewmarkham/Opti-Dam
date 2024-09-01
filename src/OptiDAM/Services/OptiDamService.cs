using EPiServer.Core;

using System.Net.Http.Json;
using OptiDAM.Services.Models;

namespace OptiDAM.Services
{
    public class OptiDamService : IOptiDamService 
    {
        private readonly HttpClient _httpClient;
        private readonly IOptiDamAuthService _optiDamAuthService;

        public OptiDamService(HttpClient httpClient, 
                              IOptiDamAuthService optiDamAuthService)
        {
            _httpClient = httpClient;
            _optiDamAuthService = optiDamAuthService;
        }

        public async Task<UploadUrlResponse?> GetUploadUrl() 
        {
            var bearerToken = await _optiDamAuthService.GetBearerToken();
            if (bearerToken == null) {
                return null;
            }
            var request = new HttpRequestMessage(HttpMethod.Get, "/v3/upload-url");
            request.Headers.Add("Authorization", $"Bearer {bearerToken} ");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var uploadResponse = await response.Content.ReadFromJsonAsync<UploadUrlResponse>();
            return uploadResponse;
        }

        public async Task UploadFile(UploadUrlResponse uploadUrl, MediaData mediaData)
        {
            if(uploadUrl.UploadMetaFields == null) {
                return;
            }

            using var content = new MultipartFormDataContent
            {
                // payload
                { new StringContent(uploadUrl.UploadMetaFields.Key), "key" },
                { new StringContent(uploadUrl!.UploadMetaFields!.Policy ?? string.Empty), "policy" },
                { new StringContent(uploadUrl.UploadMetaFields!.Algorithm ?? string.Empty), "x-amz-algorithm" },
                { new StringContent(uploadUrl.UploadMetaFields!.Credential ?? string.Empty), "x-amz-credential" },
                { new StringContent(uploadUrl.UploadMetaFields!.Date ?? string.Empty), "x-amz-date" },
                { new StringContent(uploadUrl.UploadMetaFields!.SecurityToken ?? string.Empty), "x-amz-security-token" },
                { new StringContent(uploadUrl.UploadMetaFields!.Signature ?? string.Empty), "x-amz-signature" },

                                    // file
                { new StreamContent(mediaData.BinaryData.OpenRead()), "file" },
            };

            var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl.Url)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        public async Task<PostAssetResponse?> PostAsset(PostAssetRequest postAssetRequest) {
            var bearerToken = await _optiDamAuthService.GetBearerToken();
            if (bearerToken == null) {
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/v3/assets");
            request.Headers.Add("Authorization", $"Bearer {bearerToken} ");
            request.Content = JsonContent.Create(postAssetRequest);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var uploadResponse = await response.Content.ReadFromJsonAsync<PostAssetResponse>();
            return uploadResponse;
        } 

        public async Task<DamFolderResponse?> GetFolders(int offset, int pageSize) {
            var bearerToken = await _optiDamAuthService.GetBearerToken();
            if (bearerToken == null) {
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/v3/folders?offset={offset}&page_size={pageSize}");
            request.Headers.Add("Authorization", $"Bearer {bearerToken} ");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var uploadResponse = await response.Content.ReadFromJsonAsync<DamFolderResponse>();
            return uploadResponse;
        }

        public async Task<DamFolder?> CreateFolder(DamFolderRequest damFolderRequest) {
            var bearerToken = await _optiDamAuthService.GetBearerToken();
            if (bearerToken == null) {
                return null;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/v3/folders");
            request.Headers.Add("Authorization", $"Bearer {bearerToken} ");
            request.Content = JsonContent.Create(damFolderRequest);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var uploadResponse = await response.Content.ReadFromJsonAsync<DamFolder>();
            return uploadResponse;
        }
    }
}

