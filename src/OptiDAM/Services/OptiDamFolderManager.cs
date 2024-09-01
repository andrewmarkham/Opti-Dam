using OptiDAM.Services.Models;

namespace OptiDAM.Services
{
    public class OptiDamFolderManager : IOptiDamFolderManager
    {
        private readonly IOptiDamService _damService;
        private List<DamFolder> _damFolders;

        public OptiDamFolderManager(IOptiDamService damService)
        {
            _damService = damService;
            var foldersResult = _damService.GetFolders(0, 100).Result;
            _damFolders = foldersResult != null ? foldersResult.Data : [];
        }

        public async Task<DamFolder?> GetOrCreateFolder(string currentFolder, string? parentFolder)
        {
            var currentDamFolder = this.GetFolder(currentFolder);
            var parentDamFolder = parentFolder != null ? this.GetFolder(parentFolder) : null;

            if (currentDamFolder == null)
            {
                var postFolderResponse = await _damService.CreateFolder(new DamFolderRequest
                {
                    Name = currentFolder,
                    ParentFolderId = parentDamFolder?.Id
                });

                if (postFolderResponse != null)
                    _damFolders.Add(postFolderResponse);

                return postFolderResponse;
            }

            return currentDamFolder;
        }

        public DamFolder? GetFolder(string currentFolder) {
            return _damFolders.FirstOrDefault(x => x.Name == currentFolder);
        }
    }
}