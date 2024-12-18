using MaReSy2.Services;

namespace MaReSy2.pdfReports.Produkte
{
    public class ProduktAuszugPdfUseCase
    {
        private readonly ProductService productService;
        private readonly ProdukteAuszugPdfGenerator pdfGenerator;

        public ProduktAuszugPdfUseCase(ProductService productService, ProdukteAuszugPdfGenerator pdfGenerator)
        {
            this.productService = productService;
            this.pdfGenerator = pdfGenerator;
        }

        public async Task<MemoryStream> GenerateProdukteAuszugPdfAsync()
        {
            var products = await  productService.GetProductsAsync();

            var memoryStream = new MemoryStream();
            pdfGenerator.Generate(products, memoryStream);

            return memoryStream;
        }
    }
}
