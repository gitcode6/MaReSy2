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
using MaReSy2.pdfReports.Produkte;
using MaReSy2.pdfReports.Sets;

namespace MaReSy2.Pages
{
    public class AuszügeModel : PageModel
    {
        private readonly ProduktAuszugPdfUseCase produktAuszugPdfUseCase;
        private readonly SetAuszugPdfUseCase setAuszugPdfUseCase;

        public AuszügeModel(ProduktAuszugPdfUseCase produktAuszugPdfUseCase, SetAuszugPdfUseCase setAuszugPdfUseCase)
        {
            this.produktAuszugPdfUseCase = produktAuszugPdfUseCase;
            this.setAuszugPdfUseCase = setAuszugPdfUseCase;
        }

        public async Task<IActionResult> OnGetDownloadProduktePdfAsync()
        {
            var memoryStream = await produktAuszugPdfUseCase.GenerateProdukteAuszugPdfAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/pdf", $"{/*Produkte-Auszug-{DateTime.Now.ToString("dd-MM-yyyy")}_*/Guid.NewGuid()}.pdf");
        }


        public async Task<IActionResult> OnGetDownloadSetsPdfAsync()
        {
            var memoryStream = await setAuszugPdfUseCase.GenerateSetsAuszugPdfAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/pdf", $"{/*Sets-Auszug-{DateTime.Now.ToString("dd-mm-yyyy")}_*/Guid.NewGuid()}.pdf");
        }
    }
}