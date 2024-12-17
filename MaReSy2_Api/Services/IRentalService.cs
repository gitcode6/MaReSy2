using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalDTO>> GetAllRentalsAsync();
        Task<IEnumerable<RentalDTO>> GetAllUserRentalsAsync(int userId);
        Task<List<IdentityResult>> AddNewRentalAsync(CreateRentalDTO rental);
        Task<IEnumerable<IdentityResult>> UpdateRental(ActionDTO rentalAction);
        Task<List<IdentityResult>> userCancelRental(int rentalId, int userId);
        Task<RentalDTO?> GetRentalAsync(int rentalId, int userId);
    }
}
