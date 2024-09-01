using EPiServer;
using EPiServer.Cms.WelcomeIntegration.Core.Internal;
using EPiServer.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Identity.Client.Extensibility;
using OptiDAM.Services;
using SixLabors.ImageSharp;
using Wangkanai.Detection.Services;

namespace OptiDAM.Tags;

[HtmlTargetElement("img", Attributes = "image-ref", TagStructure = TagStructure.WithoutEndTag)]
public class ImageTagHelper : TagHelper
{
    private readonly IContentLoader _contentLoader;
    private readonly IImageHelper _imageHelper;
    private readonly IDetectionService _detectionService;

    public ContentReference? ImageRef { get; set; }
    public string? Key { get; set; }

    public ImageTagHelper(IContentLoader contentLoader, 
                          IImageHelper imageHelper,
                          IDetectionService detectionService)
    {
        _contentLoader = contentLoader;
        _imageHelper = imageHelper;
        _detectionService = detectionService;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {

        if (ImageRef?.ProviderName?.Equals("dam", StringComparison.OrdinalIgnoreCase) ?? false) {
            var image =  _contentLoader.Get<IContent>(ImageRef, LanguageSelector.AutoDetect());

            if (image is DAMImageAsset imageData && imageData.DAMUrl != null) {

                var imageInfo = await _imageHelper.BuildImageDetails(imageData.DAMUrl.ToString(), Key);

                output.Attributes.SetAttribute("src", imageInfo.Src.Split(" ")[0]);
                output.Attributes.SetAttribute("srcset", string.Join(", ", imageInfo.SrcSets));
                output.Attributes.SetAttribute("sizes", imageInfo.Sizes);
                output.Attributes.SetAttribute("alt", imageInfo.AltText);
            }
        }
        await base.ProcessAsync(context, output);
    }

    public override void Init(TagHelperContext context) {
        base.Init(context);
    }
}
