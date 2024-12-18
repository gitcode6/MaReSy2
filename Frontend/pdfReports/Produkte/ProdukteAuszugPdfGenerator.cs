using MaReSy2.ConsumeModels;
using MaReSy2.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace MaReSy2.pdfReports.Produkte
{
    public class ProdukteAuszugPdfGenerator
    {
        public ProdukteAuszugPdfGenerator()
        {
            
        }
        public void Generate(List<Product> products, MemoryStream memoryStream)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Column(column =>
                    {
                        column.Item().Text("Produkte-Liste").FontSize(18).Bold().AlignCenter();

                        // Daten aus der API anzeigen
                        foreach (var item in products)
                        {
                            column.Item().Text($"ID: {item.productId} - Name: {item.productname}");
                        }
                    });
                });
            }).GeneratePdf(memoryStream);
        }

    }
}
