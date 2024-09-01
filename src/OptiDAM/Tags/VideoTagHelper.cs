using EPiServer;
using EPiServer.Cms.WelcomeIntegration.Core.Internal;
using EPiServer.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace OptiDAM.Tags;

[HtmlTargetElement("video", Attributes = "video-ref")]
public class VideoTagHelper : TagHelper
{
    private readonly IContentLoader _contentLoader;

    public ContentReference? VideoRef { get; set; }

    public VideoTagHelper(IContentLoader contentLoader)
    {
        _contentLoader = contentLoader;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {

        if (VideoRef?.ProviderName?.Equals("dam", StringComparison.OrdinalIgnoreCase) ?? false) {
            var video =  _contentLoader.Get<IContent>(VideoRef, LanguageSelector.AutoDetect());

            if (video is DAMVideoAsset videoData && videoData.DAMUrl != null) {

                output.Content.SetHtmlContent($@"<source src=""{videoData?.DAMUrl}"" type=""{videoData?.MimeType}"" />");
            }
        }
        await base.ProcessAsync(context, output);
    }

    public override void Init(TagHelperContext context) {
        base.Init(context);
    }
}
