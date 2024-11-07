using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.UserDTO;
using MaReSy2_Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace MaReSy2_Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        public MaReSyDbContext context { get; }


        public UserController(MaReSyDbContext context, IUserManagementService userManagementService)
        {
            this.context = context;
            this._userManagementService = userManagementService;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<User>>> GetUsers()
        //{
        //    return Ok(await context.Users.Include(u=>u.Role).ToListAsync());
        //}

        [HttpGet("")]
        public async Task<ActionResult<List<UserDTO>>> getUsers(string searchOption=null)
        {
            var users = await _userManagementService.GetUsersAsync(searchOption);

            if(users == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(users);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> getUser(int id)
        {
            var user = await _userManagementService.FindUserAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost ("")]
        public async Task<IActionResult> createUser( CreateUserDTO user)
        {
             Validator.ValidateObject(user, new ValidationContext(user), validateAllProperties: true);
            if (ModelState.IsValid)
            {

                var result = await _userManagementService.AddUserAsync(user);

                if (result.Contains(IdentityResult.Success))
                {
                    return Ok(result);
                }

                else
                {
                    return BadRequest(result);
                }
            }

            return BadRequest();

        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> updatePassword(int id, string newPassword)
        {
            bool success = (bool)await _userManagementService.ChangePasswordAsync(id, newPassword);
            if(success)
            {
                return Ok(IdentityResult.Success);
            }
            else
            {
                return BadRequest(id);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> updateUser(int id, UserDTO? user)
        {
            var result = await _userManagementService.updateUser(id, user);

            if (result.Contains(IdentityResult.Success))
            {
                return Ok(result);
            }

            else
            {
                return BadRequest(result);
            }
        }
        


    }
}
