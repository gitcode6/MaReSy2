namespace MaReSy2_Api.Models.DTO.RentalDTO
{
    public class RentalDTO
    {
        //fortlaufende ID
        public int rentalId { get; set; }

        //Reservierungs-User
        public RentalUserDTO user { get; set; }


        //Set-Daten, falls Set reserviert
        public int? setId { get; set; }
        public string? setname { get; set; }


        //Reservierungsdaten
        public DateOnly rentalStart { get; set; }
        public DateOnly rentalEnd { get; set; }

        //Anforderungsdatum
        public DateTime rentalAnforderung { get; set; }

        //Freigabedatum und Freigabeuser
        public DateTime? rentalFreigabe { get; set; }
        public RentalUserDTO? rentalFreigabeUser { get; set; }

        //Ablehnungsdatum und Ablehnungsuser
        public DateTime? rentalAblehnung { get; set; }
        public RentalUserDTO? rentalAblehnungUser { get; set; }

        //Auslieferungsdatum und Auslieferungsuser
        public DateTime? rentalAuslieferung {  get; set; }
        public RentalUserDTO? rentalAuslieferungUser { get; set; }


        //Zurückdatum und Rücknahmeuser
        public DateTime? rentalZuereck {  get; set; }
        public RentalUserDTO? rentalZuereckUser { get; set; }

        //Datum der Stornierung
        public DateTime? rentalStornierung {  get; set; }

        //Reservierungsstatus
        public string status { get; set; } = String.Empty;

        //Reservierungsanmerkung
        public string? rentalNote { get; set; }
        public List<RentalSingleProductDTO> singleProducts { get; set; }




    }
}
