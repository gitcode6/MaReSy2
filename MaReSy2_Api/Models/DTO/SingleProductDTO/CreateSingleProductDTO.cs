using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.SingleProductDTO
{
    public class CreateSingleProductDTO
    {
        [Required]
        public string SingleProductName { get; set; }
        
        [Required]
        public string SingleProductSerialNumber { get; set; }

        [Required]
        public bool SingleProductActive { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
