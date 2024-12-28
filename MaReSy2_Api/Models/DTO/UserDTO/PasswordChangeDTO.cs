using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.UserDTO
{
    public class PasswordChangeDTO
    {
        [Required]
        public string newPassword { get; set; }

        [Required]
        public string confirmPassword { get; set; }
    }
}
