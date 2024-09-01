using Microsoft.Extensions.Options;
using OptiDAM.Graph;
using OptiDAM.Initilization;
using OptiDAM.Services.Models;

namespace OptiDAM.Services;

internal class ImageHelper : IImageHelper {

    private readonly IOptiDamGraphService _optiDamGraphService;
    private readonly IOptions<ImageDimensionOption> _imageDimensionOption;

    public ImageHelper(IOptiDamGraphService optiDamGraphService,
                       IOptions<ImageDimensionOption> imageDimensionOption) {
        _optiDamGraphService = optiDamGraphService;
        _imageDimensionOption = imageDimensionOption;
    }

    public async Task<ImageInfo> BuildImageDetails(string damSrc, string key) {
        var imageInfo = new ImageInfo();
        var images = new Dictionary<string, string>();
        var sizes = new Dictionary<string, string>();

        var damImage = await _optiDamGraphService.GetDamImage(damSrc);

        var imageDimension = GetImageDimension(key);

        if (imageDimension != null) {
            foreach (var (k, value) in imageDimension) {
                images.Add(k, damImage?.Renditions.Where(r => r.Width >= value.Width)
                                                  .OrderBy(r => r.Width)
                                                  .Select(r => $"{r.Url}?width={value.Width+1} {value.Width}w")
                                                  .FirstOrDefault() ?? string.Empty);
                if (k == "desktop") continue;

                sizes.Add(k, $"(max-width: {value.Width}px) {value.Width}px");
            }
        }

        if (damImage != null) {
            imageInfo.AltText = damImage?.AltText ?? damImage?.Title ?? string.Empty;
            imageInfo.Src = images["desktop"];
            imageInfo.SrcSets = images.Where(i => !string.IsNullOrEmpty(i.Value))
                                      .Select(i => i.Value)
                                      .ToList();
            imageInfo.Sizes = sizes.Count > 0 ? string.Join(", ", sizes.Select(s => s.Value)) : string.Empty;
        }

        return imageInfo;

    }

    private Dictionary<string,ImageDimension>? GetImageDimension(string key) {
        if (_imageDimensionOption.Value.ContainsKey(key)) {
            return _imageDimensionOption.Value[key];
        }
        return null;
    }
}
