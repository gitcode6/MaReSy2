using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SetDTO;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public interface ISetService
    {
        Task<IEnumerable<SetDTO>> GetSetsAsync();
        Task<SetDTO?> GetSetByIdAsync(int setId);
        Task<bool> SetExistsAsync(int setId);
        Task<List<IdentityResult>> AddNewSetAsync(CreateSetDTO set);

    }
}
