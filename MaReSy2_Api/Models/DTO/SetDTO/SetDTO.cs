using MaReSy2_Api.Models.DTO.ProductDTO;

namespace MaReSy2_Api.Models.DTO.SetDTO
{

    public class SetDTO
    {
        public int SetId { get; set; }

        public string Setname { get; set; }

        public string? Setdescription { get; set; }

        public bool Setactive { get; set; }

        public string? SetimageLink { get; set; }

        public List<AssignedProductDTO>? Products { get; set; }

    }
}
