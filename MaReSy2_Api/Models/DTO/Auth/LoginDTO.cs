using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.Auth
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
