using System.Text.Json.Serialization;

namespace OptiDAM.Initilization;

public class ImageDimensionOption : Dictionary<string,Dictionary<string, ImageDimension>> {
    public static string ImageDimensions = "ImageDimensions";
}

public class ImageDimension {
    public int Width { get; set; }
    public int Height { get; set; }
}
