using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.SetDTO
{
    public class CreateSetDTO
    {
        [Required]
        public string Setname { get; set; }

        public string? Setdescription { get; set; }

        [Required]
        public bool Setactive { get; set; }


        public List<SetProductAssignDTO>? setProductAssignDTOs { get; set; }


    }
}
