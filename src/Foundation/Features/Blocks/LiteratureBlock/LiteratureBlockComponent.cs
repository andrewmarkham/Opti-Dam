
using OptiDAM.Graph;

namespace Foundation.Features.Blocks.LiteratureBlock
{
    public class LiteratureBlockComponent : AsyncBlockComponent<LiteratureBlock>
    {
        private readonly IOptiDamGraphService _optiDamGraphService;


        /// <summary>
        /// Constructor
        /// </summary>
        public LiteratureBlockComponent(IOptiDamGraphService optiDamGraphService)
        {
            _optiDamGraphService = optiDamGraphService;
        }

        protected override async Task<IViewComponentResult> InvokeComponentAsync(LiteratureBlock currentBlock)
        {
            try
            {
                var documents = await _optiDamGraphService.GetDocuments();

                var vm = new LiteratureBlockViewModel
                {
                    Documents = documents.Select(x => new LiteratureDocument
                    {
                        Title = x.Fields.FirstOrDefault(f => f.Name == "Heading")?.Values.FirstOrDefault(),
                        Url = x.Url
                    }).ToList()
                };

                return await Task.FromResult(View("~/Features/Blocks/LiteratureBlock/LiteratureBlock.cshtml", vm));

            }
            catch (Exception)
            {
                // The rating service may throw a number of possible exceptions
                // should handle each one accordingly -- see rating service documentation
            }

            return await Task.FromResult(View("~/Features/Blocks/LiteratureBlock/LiteratureBlock.cshtml"));
        }
    }
}
