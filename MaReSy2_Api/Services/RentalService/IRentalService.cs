using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services.RentalService
{
    public interface IRentalService
    {
        Task<APIResponse<List<RentalDTO>>> GetAllRentalsAsync();
        Task<APIResponse<List<RentalDTO>>> GetAllUserRentalsAsync(int userId);
        Task<APIResponse<string>> AddNewRentalAsync(CreateRentalDTO rental, int userId);
        Task<APIResponse<string>> UpdateRental(int userId, ActionDTO rentalAction);
        Task<APIResponse<string>> userCancelRental(int rentalId, int userId);
        Task<APIResponse<RentalDTO>> GetRentalAsync(int rentalId, int userId);
    }
}
