using EPiServer.Core;
using OptiDAM.Services.Models;

namespace OptiDAM.Services
{
    public interface IOptiDamService
    {
        Task<UploadUrlResponse?> GetUploadUrl();
        Task UploadFile(UploadUrlResponse uploadUrl, MediaData mediaData);

        Task<PostAssetResponse?> PostAsset(PostAssetRequest postAssetRequst);

        Task<DamFolderResponse?> GetFolders(int offset, int pageSize);

        Task<DamFolder?> CreateFolder(DamFolderRequest damFolderRequest);
    }
}