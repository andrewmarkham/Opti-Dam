using EPiServer;
using EPiServer.Cms.WelcomeIntegration.Core;
using EPiServer.Cms.WelcomeIntegration.Core.Internal;
using EPiServer.Cms.WelcomeIntegration.UI.Internal;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Mediachase.BusinessFoundation.Data;
using Mediachase.Commerce.Catalog;
using OptiDAM.Graph;
using OptiDAM.Services;
using OptiDAM.Services.Models;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace OptiDAM.Jobs
{
    [ScheduledPlugIn(
        DisplayName = "Copy Files to Optimizely DAM", 
        Description = "Copy all Files to Optimizely DAM", SortIndex = 10000)]
    [ServiceConfiguration]
    public class TransferImagesToDamJob : ScheduledJobBase
    {
        protected Injected<IBlobFactory> BlobFactory { get; set; }
        private int _fileCount = 0;
        private int _folderCount = 0;
        private readonly StringBuilder _errorText = new StringBuilder();
        
        private IOptiDamService damService;
        private IOptiDamFolderManager damFolderManager;

        public TransferImagesToDamJob()
        {
            IsStoppable = true;
            damService = ServiceLocator.Current.GetInstance<IOptiDamService>();
            damFolderManager = ServiceLocator.Current.GetInstance<IOptiDamFolderManager>();
        }

        public override string Execute()
        {
            OnStatusChanged(string.Format("Starting execution of {0}", this.GetType()));

            ProcessFolder(SiteDefinition.Current.GlobalAssetsRoot);
            
            var status = string.Format($"Uploaded {this._folderCount} folders and {this._fileCount} files");

            return status;

        }

        private void ProcessFolder(ContentReference folderReference, ContentFolder? parentFolder = null)
        {
            _folderCount++;

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var mediaChildrenReferences = contentLoader.GetChildren<IContent>(folderReference);
            var currentFolder = contentLoader.Get<ContentFolder>(folderReference);

            OnStatusChanged(string.Format($"Processing Folder: {currentFolder.Name}, Total Children: {mediaChildrenReferences.Count()}"));

            var currentDamFolder = damFolderManager.GetOrCreateFolder(currentFolder.Name, parentFolder?.Name).Result;
            
            foreach (var child in mediaChildrenReferences)
            {
                if (child is ContentFolder contentFolder)
                {
                    ProcessFolder(contentFolder.ContentLink, currentFolder);
                }

                if (child is MediaData childMedia)
                {
                    ProcessFile(childMedia, currentDamFolder);
                }
            }
        }
        private void ProcessFile(MediaData mediaData, DamFolder parentFolder)
        {
            /*
            https://docs.developers.optimizely.com/content-marketing-platform/docs/dam-assets-migration-using-the-api
            */
            
            _fileCount++;
            var fileTask = Task.Run(async () => {
            
                var uploadUrlResponse = await damService.GetUploadUrl();

                if (uploadUrlResponse ==null || uploadUrlResponse?.UploadMetaFields == null)
                {
                    return;
                }

                await damService.UploadFile(uploadUrlResponse, mediaData);

                var postAssetResponse = await damService.PostAsset(new PostAssetRequest{
                    Key = uploadUrlResponse.UploadMetaFields.Key,
                    Title = mediaData.Name,
                    FolderId = parentFolder.Id
                });

                //TODO: Add meta data (use AI to extract meta data)

                //TODO: Add taxonomy
            });

            fileTask.Wait();

        }
    }


    [ScheduledPlugIn(
        DisplayName = "Migrate Commerce Images Job", 
        Description = "Update Images to reference DAM", SortIndex = 10000)]
    [ServiceConfiguration]
    public class MigrateCommerceImagesJob : ScheduledJobBase
    {
        protected Injected<IBlobFactory> BlobFactory { get; set; }
        private int _fileCount = 0;
        private int _folderCount = 0;
        private readonly StringBuilder _errorText = new StringBuilder();
        
        private IOptiDamService damService;

        private IOptiDamGraphService damGraphService;

        private IOptiDamFolderManager damFolderManager;

        private IContentLoader contentLoader;
        private IContentRepository contentRepository;
        private ReferenceConverter referenceConverter;
        private IRelationRepository relationRepository;
        private IDAMAssetIdentityResolver damAssetIdentityResolver;

        public MigrateCommerceImagesJob()
        {
            IsStoppable = true;
            damService = ServiceLocator.Current.GetInstance<IOptiDamService>();
            damGraphService = ServiceLocator.Current.GetInstance<IOptiDamGraphService>();
            damFolderManager = ServiceLocator.Current.GetInstance<IOptiDamFolderManager>();
            contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            referenceConverter = ServiceLocator.Current.GetInstance<ReferenceConverter>();
            damAssetIdentityResolver = ServiceLocator.Current.GetInstance<IDAMAssetIdentityResolver>();
            relationRepository = ServiceLocator.Current.GetInstance<IRelationRepository>();
        }

        public override string Execute()
        {
            OnStatusChanged(string.Format("Starting execution of {0}", this.GetType()));

            ProcessFolder(referenceConverter.GetRootLink());
            
            var status = string.Format($"Uploaded {this._folderCount} folders and {this._fileCount} files");

            return status;

        }

        private void ProcessFolder(ContentReference folderReference, ContentFolder? parentFolder = null)
        {
            _folderCount++;

            //var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var childrenReferences = contentLoader.GetChildren<IContent>(folderReference);
            //var currentFolder = contentLoader.Get<ContentFolder>(folderReference);

            //OnStatusChanged(string.Format($"Processing Folder: {currentFolder.Name}, Total Children: {mediaChildrenReferences.Count()}"));

            foreach (var child in childrenReferences)
            {
                if (child is CatalogContent contentFolder)
                {
                    ProcessFolder(contentFolder.ContentLink);
                }

                if (child is NodeContent nodeContent)
                {
                    ProcessFolder(nodeContent.ContentLink);
                }

                if (child is EntryContentBase entryContent)
                {
                   ProcessFile(entryContent);
                }
            }
        }
        private void ProcessFile(EntryContentBase entry)
        {

            bool requiresUpdate = false;
            var updateEntry = entry.CreateWritableClone() as EntryContentBase;

            //process images
            var assets = entry.CommerceMediaCollection.ToList();
            
            updateEntry.CommerceMediaCollection.Clear();
            
            foreach (var asset in assets)
            {
                if (asset.AssetLink.ProviderName == "dam")
                {
                    continue;
                }

                var mediaData = contentLoader.Get<IContent>(asset.AssetLink);
                
                var damFile = damGraphService.SearchDamImage(mediaData.Name).Result;

                if (damFile is not null)
                {
                    var damMetaData = new DAMMetadata(mediaData.Name, DAMAssetType.Image);

                    var damAssetIdentity = damAssetIdentityResolver.Get(new Uri(damFile.Url), JsonSerializer.Serialize(damMetaData));

                    updateEntry.CommerceMediaCollection.Add(new CommerceMedia 
                    { 
                        AssetLink = damAssetIdentity.ContentLink,
                        GroupName = "default",
                        AssetType = "episerver.cms.welcomeintegration.core.internal.damimageasset"
                    });

                    requiresUpdate = true;
                }
            }

            if (requiresUpdate)
            {
                contentRepository.Save(updateEntry, SaveAction.Publish | SaveAction.SkipValidation, AccessLevel.NoAccess);
            }
            
            var variants = relationRepository.GetChildren<ProductVariation>(entry.ContentLink);
            foreach (var variant in variants)
            {
                ProcessFile(contentLoader.Get<EntryContentBase>(variant.Child));   
            }
        }
    }

}
