namespace MaReSy2.ConsumeModels
{
    public class Rental
    {
        public int rentalId { get; set; }
        public RentalUserDTO user { get; set; }
        public int? setId { get; set; }
        public string? setname { get; set; }
        public DateOnly rentalStart { get; set; }
        public DateOnly rentalEnd { get; set; }


        public DateTime rentalAnforderung { get; set; }
        public DateTime? rentalFreigabe { get; set; }
        public RentalUserDTO? rentalFreigabeUser { get; set; }
        public DateTime? rentalAblehnung { get; set; }
        public RentalUserDTO? rentalAblehnungUser { get; set; }
        public DateTime? rentalAuslieferung { get; set; }
        public RentalUserDTO? rentalAuslieferungUser { get; set; }
        public DateTime? rentalZuereck { get; set; }
        public RentalUserDTO? rentalZuereckUser { get; set; }
        public DateTime? rentalStornierung { get; set; }
        public string status { get; set; }
        public string? rentalNote { get; set; }
        public List<SingleProductRental>? singleProducts { get; set; }
    }

    public class SingleProductRental
    {
        public int singleProductId { get; set; }
        public string singleProductNumber { get; set; }
        public string singleProductName { get; set; }
        public string singleProductCategory { get; set; }
    }

    public class RentalUserDTO
    {
        public int userId { get; set; }

        public string username { get; set; }
    }
}
