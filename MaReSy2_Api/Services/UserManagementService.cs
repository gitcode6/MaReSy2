
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.UserDTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;

namespace MaReSy2_Api.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly MaReSyDbContext _context;

        public UserManagementService()
        {
        }

        public UserManagementService(MaReSyDbContext context)
        {
            _context = context;
        }

        public async Task<List<IdentityResult>> AddUserAsync(CreateUserDTO user)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            if (await EmailExistsAsync(user.Email))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Email wird bereits verwendet!" }));
            }
            if(await UsernameExistsAsync(user.Username))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Benutzername wird bereits verwendet" }));
            }

            if (!_context.Roles.Select(x => x.Rolename.ToLower()).Contains(user.Role.ToLower()))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Diese Rolle gibt es im System nicht" }));
            }

            if(errors.Count > 0)
            {
                return errors;
            }
            // TODO: User-Passwort verschlüsselt in DB speichern

            var DBuser = new User()
            {
                Username = user.Username,
                Email = user.Email,
                Password = user.Password,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Role = _context.Roles.Where(x=>x.Rolename == user.Role).First(),
            };

            var createdUser = await _context.Users.AddAsync(DBuser);

            if(createdUser != null)
            {

                await _context.SaveChangesAsync();
                errors.Add(IdentityResult.Success);
            }
            else
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Beim erstellen des Benutzers ist ein Fehler aufgetreten!" }));
            }

            return errors;
        }

        public async Task<bool?> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
            {
                return null;
            }

            user.Password = newPassword;
            var result = await _context.SaveChangesAsync();

            if(result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

           

        }

        public async Task<UserDTO> FindUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return null;
            }


            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role = user.Role?.Rolename ?? string.Empty,
            };

            return userDTO;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync(string searchOption)
        {
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


            return userList.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role = userRoles.FirstOrDefault(ur => ur.UserId == user.UserId)?.RoleName ?? string.Empty,
            });


        }

        public async Task <bool> EmailExistsAsync (string email)
        {
           return await _context.Users.AnyAsync(x=>x.Email.ToLower()  == email.ToLower());
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(x=>x.Username.ToLower() == username.ToLower());
        }

        public async Task<List<IdentityResult>> updateUser(int id, UpdateUserDTO user)
        {
           List<IdentityResult> errors = new List<IdentityResult>();

            var existingUser = await _context.Users.FindAsync(id);


            if (existingUser == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Benutzer nicht gefunden!" }));

                return errors;
            }

            if(!string.IsNullOrEmpty(user.Email) && await _context.Users.AnyAsync(u => u.Email == user.Email && u.UserId != id))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "E-Mail wird bereits verwendet!" }));
            }

            if (!string.IsNullOrEmpty(user.Username) && await _context.Users.AnyAsync(u => u.Username == user.Username && u.UserId != id))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Benutzername wird bereits verwendet." }));
            }

            var userRole = user.Role != null ? await _context.Roles.FirstOrDefaultAsync(r => r.Rolename.ToLower() == user.Role.ToLower()) : null;
            if (userRole == null && user.Role != null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Diese Rolle gibt es im System nicht." }));
            }

            if (errors.Count > 0)
            {
                return errors;
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
            existingUser!.Password = user.Password ?? existingUser.Password;


            

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            errors.Add(IdentityResult.Success);
            return errors;

        }

        public async Task<IdentityResult> DeleteUserAsync(int id)
        {
           var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Benutzer nicht gefunden!" });
            }

            user.Role = await _context.Roles.FirstOrDefaultAsync(x => x.Rolename.ToLower().Equals("inaktiv"));

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }
    }
}
