using System.Text;

namespace MaReSy2_Api.Utils
{
    public static class PasswordEncrypt
    {
        public static string? CreatePasswordHash(string password)
        {
            if (password == null) return null;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
