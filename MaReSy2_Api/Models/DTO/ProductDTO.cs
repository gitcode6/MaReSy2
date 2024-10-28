namespace MaReSy2_Api.Models.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string Productname { get; set; } = null!;

        public string? Productdescription { get; set; }


        public int Productactive { get; set; }

        public int Productamount { get; set; }
    }
}
