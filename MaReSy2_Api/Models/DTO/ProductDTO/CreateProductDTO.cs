using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.ProductDTO
{
    public class CreateProductDTO
    {
        //[Required]
        public string Productname { get; set; }

        //[Required]
        public string Productdescription { get; set; }

        //[Required]
        public bool Productactive { get; set; }

    }
}
