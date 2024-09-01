namespace Foundation.Features.Blocks.LiteratureBlock;

public class LiteratureBlockViewModel
{
    public List<LiteratureDocument> Documents { get; set; }
}

public class LiteratureDocument
{
    public string Title { get; set; }
    //public string Description { get; set; }
    public string Url { get; set; }
}