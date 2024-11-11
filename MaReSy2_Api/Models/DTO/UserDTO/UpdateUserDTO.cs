using System.ComponentModel.DataAnnotations;

namespace MaReSy2_Api.Models.DTO.UserDTO
{
    public class UpdateUserDTO
    {
        public string? Username { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}
