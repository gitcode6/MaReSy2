namespace MaReSy2_Api.Models.DTO.ProductDTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string Productname { get; set; } = null!;

        public string? Productdescription { get; set; }

        public bool Productactive { get; set; }

        public string? ProductimageLink { get; set; }

        //public int Productamount { get; set; }
    }
}
