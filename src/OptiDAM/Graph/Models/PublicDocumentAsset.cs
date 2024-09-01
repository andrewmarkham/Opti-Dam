namespace OptiDAM.Graph.Models;

public class PublicDocumentAsset 
{
    public string Title { get; set; }
    public string MimeType { get; set; }
    public string Url { get; set; }

    public List<Field> Fields { get; set; }
}

public class Field 
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> Values { get; set; }
}
