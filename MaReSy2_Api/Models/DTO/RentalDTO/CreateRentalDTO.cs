namespace MaReSy2_Api.Models.DTO.RentalDTO
{
    public class CreateRentalDTO
    {
        //setid ?? wenn set
        public int? setId { get; set; }

        //product mit amount, wenn kein set
        public int? productId { get; set; }
        public int? productAmount { get; set; }

        public DateOnly fromDate { get; set; } = DateOnly.MaxValue;
        public DateOnly endDate { get; set; } = DateOnly.MaxValue;


        public string? rentalNote { get; set; }

        //rental for user
       // public int userId { get; set; }



    }
}
