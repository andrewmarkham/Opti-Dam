namespace OptiDAM.Graph.Models;

public class PublicVideoAsset 
{
    public string Title { get; set; }
    public string MimeType { get; set; }
    public string AltText { get; set; }
    public string Url { get; set; }

    //public string Width { get; set; }

    //public string Height { get; set; }

    public List<RenditionType> Renditions { get; set; }  
}
