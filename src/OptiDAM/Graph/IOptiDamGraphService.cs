using OptiDAM.Graph.Models;

namespace OptiDAM.Graph;

public interface IOptiDamGraphService
{
    Task<PublicImageAsset?> GetDamImage(string url);
    Task<PublicImageAsset?> SearchDamImage(string filename);
    Task<PublicVideoAsset?> GetDamVideo(string url);

    Task<List<PublicDocumentAsset>?> GetDocuments();
}
