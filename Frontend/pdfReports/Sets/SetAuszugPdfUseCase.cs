using MaReSy2.ConsumeModels;
using MaReSy2.pdfReports.Produkte;
using MaReSy2.Services;
using QuestPDF.Fluent;

namespace MaReSy2.pdfReports.Sets
{
    public class SetAuszugPdfUseCase
    {
        private readonly SetService setService;
        private readonly SetsAuszugPdfGenerator pdfGenerator;

        public SetAuszugPdfUseCase(SetService setService, SetsAuszugPdfGenerator pdfGenerator)
        {
            this.setService = setService;
            this.pdfGenerator = pdfGenerator;
        }

        public async Task<MemoryStream> GenerateSetsAuszugPdfAsync()
        {
            var sets = await setService.GetSetsAsync();
            List<Set> sets1 = new List<Set>
{
    new Set { setId = 1, setname = "Übungset 25", setdescription = "Dies ist für die Übung.", setactive = false },
    new Set { setId = 2, setname = "Beatmungskurs", setdescription = "Dies ist ein Set für den Beatmungskurs", setactive = true },
    new Set { setId = 3, setname = "Testset", setdescription = "Dies ist ein Testset", setactive = false },
    new Set { setId = 4, setname = "Defi-Set", setdescription = "Defibrillator Set für den Notfall", setactive = true },
    new Set { setId = 5, setname = "Wundversorgung Set", setdescription = "Set für Wundversorgung", setactive = true },
    new Set { setId = 6, setname = "Blutdruck Set", setdescription = "Set für Blutdruckmessungen", setactive = true },
    new Set { setId = 7, setname = "Sauerstoff Set", setdescription = "Set für Sauerstoffversorgung", setactive = true },
    new Set { setId = 8, setname = "Intubation Set", setdescription = "Set für Intubation", setactive = false },
    new Set { setId = 9, setname = "Chirurgie Set", setdescription = "Set für chirurgische Eingriffe", setactive = true },
    new Set { setId = 10, setname = "Verbandsmaterial Set", setdescription = "Set für Verbandmaterial", setactive = false },
    new Set { setId = 11, setname = "Notfall Set", setdescription = "Set für Notfälle", setactive = true },
    new Set { setId = 12, setname = "Kreislauf Set", setdescription = "Set zur Unterstützung des Kreislaufs", setactive = false },
    new Set { setId = 13, setname = "Kälte Set", setdescription = "Set für Kältebehandlungen", setactive = true },
    new Set { setId = 14, setname = "Pflaster Set", setdescription = "Set für Pflaster und Verbände", setactive = true },
    new Set { setId = 15, setname = "Wärme Set", setdescription = "Set für Wärmetherapien", setactive = true },
    new Set { setId = 16, setname = "Schmerztherapie Set", setdescription = "Set für Schmerzbehandlungen", setactive = true },
    new Set { setId = 17, setname = "Blutspende Set", setdescription = "Set für Blutspenden", setactive = false },
    new Set { setId = 18, setname = "Impf Set", setdescription = "Set für Impfungen", setactive = true },
    new Set { setId = 19, setname = "Reanimations Set", setdescription = "Set für Reanimationsmaßnahmen", setactive = true },
    new Set { setId = 20, setname = "Medizinische Ausrüstung Set", setdescription = "Set für medizinische Ausrüstung", setactive = false },

    new Set { setId = 21, setname = "Notfallmedizin Set", setdescription = "Set für Notfallmedizin", setactive = true },
    new Set { setId = 22, setname = "Transfusion Set", setdescription = "Set für Bluttransfusionen", setactive = true },
    new Set { setId = 23, setname = "Labor Set", setdescription = "Set für Laboruntersuchungen", setactive = false },
    new Set { setId = 24, setname = "EKG Set", setdescription = "Set für EKG-Untersuchungen", setactive = true },
    new Set { setId = 25, setname = "Röntgen Set", setdescription = "Set für Röntgenuntersuchungen", setactive = true },
    new Set { setId = 26, setname = "Spritzen Set", setdescription = "Set für Spritzen und Injektionen", setactive = true },
    new Set { setId = 27, setname = "Immunisierung Set", setdescription = "Set für Immunisierungen", setactive = false },
    new Set { setId = 28, setname = "Schnelltest Set", setdescription = "Set für Schnelltests", setactive = true },
    new Set { setId = 29, setname = "Krankenhaus Set", setdescription = "Set für Krankenhausbedarf", setactive = true },
    new Set { setId = 30, setname = "Chirurgie Notfall Set", setdescription = "Set für chirurgische Notfälle", setactive = false },
    new Set { setId = 31, setname = "Neonatal Set", setdescription = "Set für die Neonatalversorgung", setactive = true },
    new Set { setId = 32, setname = "Anästhesie Set", setdescription = "Set für Anästhesie", setactive = false },
    new Set { setId = 33, setname = "Urologie Set", setdescription = "Set für urologische Untersuchungen", setactive = true },
    new Set { setId = 34, setname = "Endoskopie Set", setdescription = "Set für Endoskopie", setactive = true },
    new Set { setId = 35, setname = "Dialyse Set", setdescription = "Set für Dialysebehandlungen", setactive = false },
    new Set { setId = 36, setname = "Infusion Set", setdescription = "Set für Infusionen", setactive = true },
    new Set { setId = 37, setname = "Gips Set", setdescription = "Set für Gipsverbände", setactive = true },
    new Set { setId = 38, setname = "Fieber Set", setdescription = "Set für Fieberbehandlungen", setactive = false },
    new Set { setId = 39, setname = "Schlaganfall Set", setdescription = "Set für Schlaganfall-Notfälle", setactive = true },
    new Set { setId = 40, setname = "Mutter-Kind Set", setdescription = "Set für die Betreuung von Mutter und Kind", setactive = true }
};



            var memoryStream = new MemoryStream();
            pdfGenerator.Generate(sets, memoryStream);

            return memoryStream;
        }
    }
}
