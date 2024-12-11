using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;

namespace MaReSy2_Api.Services
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalDTO>> GetAllRentalsAsync();
        Task<IEnumerable<RentalDTO>> GetAllUserRentalsAsync(int userId);
    }
}
