using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public interface IUserManagementService
    {

        Task<IEnumerable<UserDTO>> GetUsersAsync(string searchOption);

        Task<UserDTO> FindUserAsync(int userId);

        Task<bool?> ChangePasswordAsync(int userId, string newPassword);

        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<List<IdentityResult>> AddUserAsync(string username, string firstname, string lastname, string password, string email, string role);

        Task<List<IdentityResult>> updateUser(int id, UserDTO user);

        /*
         * 
         * check - 1. Alle User abrufen (mit Einschränkungen auf Name, Email und Username) inkl. Rollen
         * check - 2. Einen speziellen User (per ID abrufen) inkl. Rolle
         * check - 3. Benutzer hinzufügen
         * 4. Benutzer aktualisieren
         * check - 5. Benutzerpasswort ändern
         * check - 6. Prüfung ob Benutzername/Email schon in Verwendung ist
         * 
         * 
         */
    }
}
