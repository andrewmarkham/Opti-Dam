using OptiDAM.Services.Models;

namespace OptiDAM.Services;

public interface IImageHelper
{
    Task<ImageInfo> BuildImageDetails(string damSrc, string key);
}
