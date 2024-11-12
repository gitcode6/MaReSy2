namespace MaReSy2_Api.Models.DTO.ProductDTO
{
    public class UpdateProductDTO
    {
        public string? Productname { get; set; }
        public string? Productdescription { get; set; }
        public bool? Productactive { get; set; }
        public int? Productamount { get; set; }
    }

}
