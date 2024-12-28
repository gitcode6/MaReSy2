using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SetDTO;
using MaReSy2_Api.Services.RentalService;
using MaReSy2_Api.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/rentals")]
    [ApiController]
    [Authorize]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IUserManagementService _userManagementService;
        private readonly MaReSyDbContext _maReSyDbContext;

        public RentalController(IRentalService rentalService, MaReSyDbContext maReSyDbContext, IUserManagementService userManagementService)
        {
            _rentalService = rentalService;
            _maReSyDbContext = maReSyDbContext;
            _userManagementService = userManagementService;
        }


        [HttpGet("")]
        public async Task<ActionResult<APIResponse<List<RentalDTO>>>> Get()
        {
            var result = await _rentalService.GetAllRentalsAsync();

            return helperMethod.ToActionResult(result, this);
        }

        [HttpGet("userrentals")]
        public async Task<ActionResult<APIResponse<List<RentalDTO>>>> GetUserRentals()
        {
            var result = await _rentalService.GetAllUserRentalsAsync(_userManagementService.getLoggedInUserId());

            return helperMethod.ToActionResult(result, this);
        }

        [HttpGet("{rentalId}")]
        public async Task<ActionResult<APIResponse<RentalDTO>>> GetSingleRental(int rentalId)
        {
            var rental = await _rentalService.GetRentalAsync(rentalId, _userManagementService.getLoggedInUserId());

            return helperMethod.ToActionResult(rental, this);
        }

        [HttpPost("")]
        public async Task<ActionResult<APIResponse<string>>> CreateRental(CreateRentalDTO rental)
        {
            //Validator.ValidateObject(rental, new ValidationContext(rental), validateAllProperties: true);

            //if (ModelState.IsValid)
            //{
            var result = await _rentalService.AddNewRentalAsync(rental, _userManagementService.getLoggedInUserId());

            return helperMethod.ToActionResultBasic(result, this);
            //}

        }


        [HttpDelete("{rentalId}/cancel")]
        public async Task<ActionResult<APIResponse<string>>> CancelRental(int rentalId)
        {


            var result = await _rentalService.userCancelRental(rentalId, _userManagementService.getLoggedInUserId());

            return helperMethod.ToActionResultBasic(result, this);
        }


        [HttpPut("")]
        public async Task<ActionResult<APIResponse<string>>> UpdateRental(ActionDTO actionDTO)
        {
            var result = await _rentalService.UpdateRental(_userManagementService.getLoggedInUserId(), actionDTO);
            return helperMethod.ToActionResultBasic(result, this);
        }

        


    }
}

