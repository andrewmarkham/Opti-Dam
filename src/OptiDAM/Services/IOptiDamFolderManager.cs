using OptiDAM.Services.Models;

namespace OptiDAM.Services
{
    public interface IOptiDamFolderManager
    {
        Task<DamFolder?> GetOrCreateFolder(string currentFolder, string? parentFolder);
        DamFolder? GetFolder(string currentFolder);
    }
}