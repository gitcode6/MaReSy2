namespace MaReSy2_Api.Models.DTO.SetDTO
{
    public class AssignedProductDTO
    {
        public int ProductId { get; set; }
        public string Productname { get; set; }
        public int Productamount { get; set; }

        public string? Productdescription { get; set; }
    }
}
