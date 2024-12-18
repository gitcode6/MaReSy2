using MaReSy2.ConsumeModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace MaReSy2.Services
{
    public class ProduktAuszugsGenerator : IPdfAuszugsGenerator
    {
        private readonly List<Product> products;

        public ProduktAuszugsGenerator(List<Product> products)
        {
            this.products = products;
        }

        public byte[] GeneratePdf()
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Column(column =>
                    {
                        column.Item().Text("Produkt-Liste").FontSize(18).Bold().AlignCenter();

                        // Daten aus der API anzeigen
                        foreach (var item in products)
                        {
                            column.Item().Text($"ID: {item.productId} - Name: {item.productname}");
                        }
                    });
                });
            }).GeneratePdf();
        }
    }
}
