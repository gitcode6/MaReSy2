namespace MaReSy2_Api.Models.DTO.SingleProductDTO
{
    public class UpdateSingleProductDTO
    {

        public string? SingleProductName { get; set; }
        public string? SingleProductSerialNumber { get; set; }
        public bool? SingleProductActive { get; set; }
        public int? ProductId { get; set; }
    }
}
