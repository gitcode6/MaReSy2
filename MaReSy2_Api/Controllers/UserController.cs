using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.UserDTO;
using MaReSy2_Api.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.ConstrainedExecution;

namespace MaReSy2_Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        public MaReSyDbContext context { get; }


        public UserController(MaReSyDbContext context, IUserManagementService userManagementService)
        {
            this.context = context;
            this._userManagementService = userManagementService;
        }



        [HttpGet("")]
        public async Task<ActionResult<APIResponse<IEnumerable<UserDTO>>>> getUsers(string searchOption = null)
        {
            var result = await _userManagementService.GetUsersAsync(searchOption);

            return helperMethod.ToActionResult(result, this);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<UserDTO?>>> getUser(int id)
        {
            var result = await _userManagementService.FindUserAsync(id);
            return helperMethod.ToActionResult(result, this);
        }

        [HttpPost("")]
        public async Task<ActionResult<APIResponse<string>>> createUser(CreateUserDTO user)
        {
            // Validator.ValidateObject(user, new ValidationContext(user), validateAllProperties: true);
            //if (ModelState.IsValid)
            {

                var result = await _userManagementService.AddUserAsync(user);

                return helperMethod.ToActionResultBasic(result, this);

                //}

            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<APIResponse<string>>> updateUser(int id, UpdateUserDTO user)
        {
            var result = await _userManagementService.updateUser(id, user);

            return helperMethod.ToActionResultBasic(result, this);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<APIResponse<string>>> deactivateUser(int id)
        {
            var result = await _userManagementService.DeleteUserAsync(id);

            return helperMethod.ToActionResultBasic(result, this);

        }

        [HttpPut("me/change-password")]
        public async Task<ActionResult<APIResponse<bool>>> ChangeUserPassword(PasswordChangeDTO passwordChange)
        {
            var result = await _userManagementService.ChangePasswordAsync(_userManagementService.getLoggedInUserId(), passwordChange.confirmPassword, passwordChange.newPassword);
            return helperMethod.ToActionResultBasic(result, this);
        }



    }
}
