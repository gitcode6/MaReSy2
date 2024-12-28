using System.Security.Claims;
using MaReSy2_Api.Models.DTO.UserDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MaReSy2_Api.Services.UserService
{
    public interface IUserManagementService
    {

        Task<APIResponse<IEnumerable<UserDTO>>> GetUsersAsync(string searchOption);

        Task<APIResponse<UserDTO?>> FindUserAsync(int userId);

        Task<APIResponse<bool>>  ChangePasswordAsync(int userId, string confirmPassword, string newPassword);


        int getLoggedInUserId();
        Task<User> getLoggedInUser();


        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<APIResponse<string>> AddUserAsync(CreateUserDTO user);

        Task<APIResponse<string>> updateUser(int id, UpdateUserDTO user);
        Task<APIResponse<string>> DeleteUserAsync(int id);

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
