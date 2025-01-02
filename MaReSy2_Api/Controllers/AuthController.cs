using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using System.Text;
using MaReSy2_Api.Models.DTO.Auth;
using Azure;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MaReSy2_Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        MaReSyDbContext _maReSyDbContext;
        readonly IConfiguration _configuration;


        public AuthController(MaReSyDbContext maReSyDbContext, IConfiguration configuration)
        {
            _maReSyDbContext = maReSyDbContext;
            _configuration = configuration;
        }


        [HttpPost("login")]
        public async Task<ActionResult<APIResponse<LoginSuccess>>> Login(LoginDTO login)
        {
            var result = new APIResponse<LoginSuccess>();

            var user = await _maReSyDbContext.Users
                .Include(user=> user.Role)
                .FirstOrDefaultAsync(user => user.Username.ToLower() == login.Username.ToLower());

            if (user == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "Username", Error = "Benutzer nicht gefunden" });

                result.StatusCode = 401;
                return helperMethod.ToActionResult(result, this);
            }


            if(user.Role.RoleId == 1002)
            {
                result.Errors.Add(new ErrorDetail { Field = "General", Error = $"Der User {user.Username} ist inaktiv und hat deswegen keinen Zugriff zum System." });
                result.StatusCode = 401;
                return helperMethod.ToActionResult(result, this);
            }

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                result.Errors.Add(new ErrorDetail { Field = "Password", Error = "Ungültiges Passwort" });

                result.StatusCode = 401;
                return helperMethod.ToActionResult(result, this);
            }

            var token = GenerateJwtToken(user);

            result.Data = new LoginSuccess
            {
                Token = token,
                User = new Models.DTO.UserDTO.UserDTO
                {
                    UserId = user.UserId,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname,
                    Email = user.Email,
                    Username = user.Username,
                    Role = user.Role.Rolename,
                }
            };
            result.StatusCode = 200;

            return helperMethod.ToActionResult(result, this);

        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

                new Claim(ClaimTypes.Name, user.Username),

                new Claim(ClaimTypes.Email, user.Email),

                new Claim(ClaimTypes.Role, user.Role.Rolename.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("password")]
        public ActionResult<String> Hash(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

    }
}
