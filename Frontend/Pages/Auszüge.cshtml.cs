using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaReSy2.Services;
using MaReSy2.ConsumeModels;
using System.Net.Mime;
using System.Data.Common;

namespace MaReSy2.Pages
{
    public class AuszügeModel : PageModel
    {
        private readonly ProductService meinProduktService;
        private readonly SetService setService;

        public AuszügeModel(SetService setService, ProductService productService)
        {
            this.setService = setService;
            this.meinProduktService = productService;
        }

        public async Task<IActionResult> OnGetGeneratePdfProdukteAsync(bool inline = true)
        {
            // API-Daten abrufen über den ProduktService
            var products = await meinProduktService.GetProductsAsync(); // Stelle sicher, dass deine Methode korrekt heißt

            // PDF generieren
            var pdfBytes = GeneratePdf_Products(products);

            // PDF anzeigen oder herunterladen
            return File(pdfBytes, "application/pdf", "Auszug_Produkte.pdf");
        }

        private byte[] GeneratePdf_Products(List<Product> products)
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
                        column.Item().Text("Produkte-Liste").FontSize(18).Bold().AlignCenter();

                        // Daten aus der API anzeigen
                        foreach (var item in products)
                        {
                            column.Item().Text($"ID: {item.productId} - Name: {item.productname}");
                        }
                    });
                });
            }).GeneratePdf();

        }

        public async Task<IActionResult> OnGetGeneratePdfSetsAsync(bool inline = true)
        {
            // API-Daten abrufen über den ProduktService
            var sets = await setService.GetSetsAsync(); // Stelle sicher, dass deine Methode korrekt heißt

            // PDF generieren
            var pdfBytes = GeneratePdf_Sets(sets);

            // PDF anzeigen oder herunterladen

            return File(pdfBytes, "application/pdf", "Auszug_Sets.pdf");
        }

        private byte[] GeneratePdf_Sets(List<Set> sets)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Seiteneinstellungen
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Dunant"));

                    page.Header().Background("#403A60").AlignCenter().Text("SET-Liste").FontSize(24).Bold().FontColor("#FFFFFF").FontFamily("Dunant");

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Spacing(10); // Abstand zwischen den Elementen

                        // Tabelle erstellen
                        column.Item().Table(table =>
                        {
                            // Spalten definieren
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(5); // ID-Spalte
                                columns.ConstantColumn(25);    // Name-Spalte
                                columns.ConstantColumn(50);   
                                columns.ConstantColumn(20);
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
                            foreach (var item in sets)
                            {
                                string setActive = item.setactive ? "Aktiv" : "Inaktiv";
                                table.Cell().Background("#F4F6F6").Padding(5).Text(item.setId.ToString());
                                table.Cell().Background("#F4F6F6").Padding(5).Text(item.setname);
                                
                                table.Cell().Background("#F4F6F6").Padding(5).Text(item.setdescription);
                                table.Cell().Background("#F4F6F6").Padding(5).Text(setActive);
                            }
                        });


                    });

                 /*   page.Footer().Height(50).AlignRight().Row(row =>
                    {
                        row.RelativeItem().AlignRight().Text("Hallo").FontSize(12).FontColor("#403A60");
                        row.RelativeItem()
                       .AlignRight()
                       .Image(@"C:\Users\User\OneDrive - BHAK BHAS Feldbach\MaReSy2\Git\MaReSy2\Frontend\wwwroot\images\Logo_Österreichisches_Rotes_Kreuz_2.jpg");



                    });*/
                });
            }).GeneratePdf();
        }


    }
}