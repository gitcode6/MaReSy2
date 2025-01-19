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
                    // Seiteneinstellungen
                    page.Margin(0);
                    page.Size(PageSizes.A4.Landscape());
                    //page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Dunant"));

                    page.Header().Background("#403A60").AlignCenter().Text("Produkt-Liste").FontSize(24).Bold().FontColor("#FFFFFF").FontFamily("Dunant");

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(10); // Abstand zwischen den Elementen

                        // Tabelle erstellen
                        column.Item().Table(table =>
                        {
                            // Spalten definieren
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(10); // ID-Spalte
                                columns.RelativeColumn(30);    // Name-Spalte
                                columns.RelativeColumn(40);
                                columns.RelativeColumn(20);
                            });

                            // Tabellenkopf
                            table.Header(header =>
                            {
                                header.Cell().Background("#D6EAF8").Padding(5).Text("ID")
                                    .FontSize(14).Bold().FontColor("#1B4F72");

                                header.Cell().Background("#D6EAF8").Padding(5).Text("Name")
                                    .FontSize(14).Bold().FontColor("#1B4F72");

                                header.Cell().Background("#D6EAF8").Padding(5).Text("Beschreibung")
                                    .FontSize(14).Bold().FontColor("#1B4F72");

                                header.Cell().Background("#D6EAF8").Padding(5).Text("Status")
                                    .FontSize(14).Bold().FontColor("#1B4F72");
                            });

                            // Zeilen
                            foreach (var item in products)
                            {
                                string setActive = item.productactive ? "Aktiv" : "Inaktiv";
                                table.Cell().Background("#F4F6F6").Padding(5).Text(item.productId.ToString());
                                table.Cell().Background("#F4F6F6").Padding(5).Text(item.productname);

                                table.Cell().Background("#F4F6F6").Padding(5).Text(item.productdescription);
                                table.Cell().Background("#F4F6F6").Padding(5).Text(setActive);
                            }
                        });

                        //page.Footer().Height(80).Column(column =>
                        //{
                        //    // Top Section: Logo
                        //    column.Item().AlignCenter().Image(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Logo_Österreichisches_Rotes_Kreuz_2.jpg")).FitWidth();

                        //    // Bottom Red Bar with White Text
                        //    column.Item()
                        //        .PaddingTop(5)
                        //        .Background(Colors.Red.Lighten1)
                        //        .PaddingVertical(5)
                        //        .Row(row =>
                        //        {
                        //            row.RelativeItem()
                        //                .AlignCenter()
                        //                .Text("ÖSTERREICHISCHES JUGENDROTKREUZ | WWW.JUGENDROTKREUZ.AT")
                        //                .FontColor(Colors.White)
                        //                .FontSize(9)
                        //                .Bold();

                        //        });
                        //});
                    });

                });
            }).GeneratePdf(memoryStream);
        }

    }
}
