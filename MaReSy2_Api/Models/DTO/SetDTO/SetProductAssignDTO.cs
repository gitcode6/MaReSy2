using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.SetDTO
{
    public class SetProductAssignDTO
    {
        [Required]
        public int productId { get; set; }

        [Required]
        public int productAmount { get; set; }
    }
}
