using MaReSy2_Api.Models.DTO.UserDTO;

namespace MaReSy2_Api.Models.DTO.Auth
{
    public class LoginSuccess
    {
        public string Token { get; set; }

        public UserDTO.UserDTO User { get; set; }
    }
}
