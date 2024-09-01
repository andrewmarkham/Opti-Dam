using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OptiDAM.Graph.Models;
using OptiDAM.Initilization;

namespace OptiDAM.Graph;

public class OptiDamGraphService: IOptiDamGraphService {

    private const int cacheTime = 30;

    private const string baseEndPoint = "https://cg.optimizely.com/content/v2?auth={0}&cache=true";
    private readonly GraphQLHttpClient _graphQLClient;
    private readonly IMemoryCache _memoryCache;

    public OptiDamGraphService(IMemoryCache memoryCache, IOptions<OptiDamGraphOption> options) {

        var endPoint = string.Format(baseEndPoint, options.Value.AuthKey);
        _graphQLClient = new GraphQLHttpClient(endPoint, new SystemTextJsonSerializer());
        _memoryCache = memoryCache;
    }
    public async Task<PublicImageAsset?> GetDamImage(string url) {

        var hash = url;
        if (_memoryCache.TryGetValue(hash, out PublicImageAsset image)) {
            return image;
        }

        var imageRequest = new GraphQLRequest{
            Query = imageQuery,
            OperationName = "ImageQuery",
            Variables = new { url = url }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<PublicImageAssetResponseType>(imageRequest);

        _memoryCache.Set(hash, graphQLResponse.Data.PublicImageAsset.Items.FirstOrDefault(), TimeSpan.FromMinutes(cacheTime));

        return graphQLResponse.Data.PublicImageAsset.Items.FirstOrDefault();
    }

    public async Task<PublicImageAsset?> SearchDamImage(string filename) {

        var imageRequest = new GraphQLRequest{
            Query = imageSearchQuery,
            OperationName = "ImageSearchQuery",
            Variables = new { filename = filename }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<PublicImageAssetResponseType>(imageRequest);

  
        return graphQLResponse.Data.PublicImageAsset.Items.FirstOrDefault();
    }

    public async Task<PublicVideoAsset?> GetDamVideo(string url) {

        var hash = url;
        if (_memoryCache.TryGetValue(hash, out PublicVideoAsset video)) {
            return video;
        }

        var videoRequest = new GraphQLRequest{
            Query = videoQuery,
            OperationName = "VideoQuery",
            Variables = new { url = url }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<PublicVideoAssetResponseType>(videoRequest);

        _memoryCache.Set(hash, graphQLResponse.Data.PublicVideoAsset.Items.FirstOrDefault(), TimeSpan.FromMinutes(cacheTime));

        return graphQLResponse.Data.PublicVideoAsset.Items.FirstOrDefault();
    }

    public async Task<List<PublicDocumentAsset>?> GetDocuments() {

        var documentRequest = new GraphQLRequest{
            Query = documentQuery,
            OperationName = "DocumentQuery"
            //,
            //Variables = new { url = url }
        };

        var graphQLResponse = await _graphQLClient.SendQueryAsync<PublicDocumentAssettResponseType>(documentRequest);

        return graphQLResponse.Data.PublicRawFileAsset.Items;
    }


    #region Queries
    
        GraphQLQuery imageQuery = new("""
                query ImageQuery($url: String) {
                    PublicImageAsset(where: { Url: { eq: $url } }) {
                        items {
                            Title
                            AltText
                            Url
                            Width
                            Height
                            MimeType
                            Renditions {
                                Name
                                Url
                                Height
                                Width
                            }
                        }
                    }
                }
            """);

        GraphQLQuery imageSearchQuery = new("""
                query ImageSearchQuery($filename: String) {
                    PublicImageAsset(where: { Title: { eq: $filename } }) {
                        items {
                            Title
                            AltText
                            Url
                            Width
                            Height
                            MimeType
                            Renditions {
                                Name
                                Url
                                Height
                                Width
                            }
                        }
                    }
                }
            """);

        GraphQLQuery videoQuery = new("""
                query VideoQuery($url: String) {
                PublicVideoAsset(where: { Url: { eq: $url } }) {
                    items {
                    AltText
                    MimeType
                    Title
                    Url
                    Renditions {
                        Height
                        Name
                        Url
                        Width
                    }
                    }
                }
                }
            """);   

        GraphQLQuery documentQuery = new("""
                query DocumentQuery {
                PublicRawFileAsset {
                    items {
                        Title
                        Url
                        MimeType
                        Fields {
                            Id
                            Name
                            Type
                            Values
                        }
                    }
                }
                }
            """);
    #endregion
}
