using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.UserDTO
{
    public class CreateUserDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
