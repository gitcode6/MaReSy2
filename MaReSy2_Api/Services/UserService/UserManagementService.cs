using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.UserDTO;
using MaReSy2_Api.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using System.Security.Claims;

namespace MaReSy2_Api.Services.UserService
{
    public class UserManagementService : IUserManagementService
    {
        private readonly MaReSyDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserManagementService(MaReSyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<APIResponse<string>> AddUserAsync(CreateUserDTO user)
        {
            var result = new APIResponse<string>();

            if (await EmailExistsAsync(user.Email))
            {
                result.Errors.Add(new ErrorDetail { Field = "Email", Error = "Email wird bereits verwendet!" });
            }
            if (await UsernameExistsAsync(user.Username))
            {
                result.Errors.Add(new ErrorDetail { Field = "Username", Error = "Benutzername wird bereits verwendet!" });
            }

            if (!_context.Roles.Select(x => x.Rolename.ToLower()).Contains(user.Role.ToLower()))
            {
                result.Errors.Add(new ErrorDetail { Field = "Role", Error = "Diese Rolle gibt es im System nicht" });
            }


            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }
            // TODO: User-Passwort verschlüsselt in DB speichern

            var DBuser = new User()
            {
                Username = user.Username,
                Email = user.Email,
                Password = PasswordEncrypt.CreatePasswordHash(user.Password),
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = _context.Roles.Where(x => x.Rolename == user.Role).First(),
            };

            var createdUser = await _context.Users.AddAsync(DBuser);

            if (createdUser != null)
            {

                await _context.SaveChangesAsync();
                result.Data = "Benutzer erfolgreich erstellt!";
                result.StatusCode = 200;
            }


            return result;



        }
        public async Task<APIResponse<bool>> ChangePasswordAsync(int userId, string confirmPassword, string newPassword)
        {
            var result = new APIResponse<bool>();


            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "UserId", Error = "Benutzer nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            if (newPassword == confirmPassword)
            {
                
                user.Password = PasswordEncrypt.CreatePasswordHash(newPassword);
                await _context.SaveChangesAsync();
                result.Data = true;
                result.StatusCode = 200;
            }
            else
            {
                result.Data = false;
                result.StatusCode = 400;
                result.Errors.Add(new ErrorDetail { Field = "General: Password match", Error = "Die Passwörter stimmen nicht überein" });
            }

            return result;



        }

        public async Task<APIResponse<UserDTO?>> FindUserAsync(int userId)
        {
            var result = new APIResponse<UserDTO?>();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "UserId", Error = "Benutzer nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }


            result.Data = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role = user.Role?.Rolename ?? string.Empty,
            };

            result.StatusCode = 200;
            return result;
        }

        public async Task<APIResponse<IEnumerable<UserDTO>>> GetUsersAsync(string searchOption)
        {
            var result = new APIResponse<IEnumerable<UserDTO>>();


            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchOption))
            {
                users = users.Where(user =>
                user.Firstname.Contains(searchOption)
                || user.Lastname.Contains(searchOption)
                || user.Email.Contains(searchOption)
                );
            }

            var userList = await users.ToListAsync();

            var userRoles = await _context.Users
                .Where(u => userList.Select(ul => ul.UserId).Contains(u.UserId))
                .Select(u => new { u.UserId, RoleName = u.Role.Rolename })
                .ToListAsync();


            result.Data = userList.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role = userRoles.FirstOrDefault(ur => ur.UserId == user.UserId)?.RoleName ?? string.Empty,
            });

            result.StatusCode = 200;
            return result;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
        }

        public async Task<APIResponse<string>> updateUser(int id, UpdateUserDTO user)
        {
            var result = new APIResponse<string>();


            var existingUser = await _context.Users.FindAsync(id);


            if (existingUser == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "UserId", Error = "Benutzer nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            if (!string.IsNullOrEmpty(user.Email) && await _context.Users.AnyAsync(u => u.Email == user.Email && u.UserId != id))
            {
                result.Errors.Add(new ErrorDetail { Field = "Email", Error = "E-Mail wird bereits verwendet!" });
            }

            if (!string.IsNullOrEmpty(user.Username) && await _context.Users.AnyAsync(u => u.Username == user.Username && u.UserId != id))
            {
                result.Errors.Add(new ErrorDetail { Field = "Username", Error = "Benutzername wird bereits verwendet!" });
            }

            var userRole = user.Role != null ? await _context.Roles.FirstOrDefaultAsync(r => r.Rolename.ToLower() == user.Role.ToLower()) : null;
            if (userRole == null && user.Role != null)
            {
                result.Errors.Add(new ErrorDetail { Field = "Role", Error = "Diese Rolle gibt es im System nicht." });
            }

            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }

            //user.Username = user.Username;
            //user.Email = user.Email;
            //user.Firstname = user.Firstname;
            //user.Lastname = user.Lastname;
            //user.Role = user.Role;


            existingUser!.Username = user.Username ?? existingUser.Username;
            existingUser!.Email = user.Email ?? existingUser.Email;
            existingUser!.Firstname = user.Firstname ?? existingUser.Firstname;
            existingUser!.Lastname = user.Lastname ?? existingUser.Lastname;
            existingUser!.Role = userRole ?? existingUser.Role;
            existingUser!.Password = PasswordEncrypt.CreatePasswordHash(user.Password) ?? existingUser.Password;




            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            result.Data = "Benutzer erfolgreich aktualisiert!";
            result.StatusCode = 200;
            return result;

        }

        public async Task<APIResponse<string>> DeleteUserAsync(int id)
        {
            var result = new APIResponse<string>();


            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "UserId", Error = "Benutzer nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            user.Role = await _context.Roles.FirstOrDefaultAsync(x => x.Rolename.ToLower().Equals("inaktiv"));

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            result.Data = "Benutzer erfolgreich deaktiviert!";
            result.StatusCode = 200;
            return result;
        }

        public int getLoggedInUserId()
        {
            string result = null;

            if (httpContextAccessor.HttpContext != null)
            {
                result = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            }

            return Convert.ToInt32(result);
        }

        public async Task<User> getLoggedInUser()
        {
            int id = getLoggedInUserId();

            User user = await _context.Users.FirstOrDefaultAsync(user => user.UserId == id);

            return user;
        }
    }
}
