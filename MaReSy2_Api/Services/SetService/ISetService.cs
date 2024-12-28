using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SetDTO;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public interface ISetService
    {
        Task<APIResponse<IEnumerable<SetDTO>>> GetSetsAsync();
        Task<APIResponse<SetDTO>> GetSetByIdAsync(int setId);
        Task<bool> SetExistsAsync(int setId);
        Task<APIResponse<string>> AddNewSetAsync(CreateSetDTO set);
        Task<APIResponse<string>> UpdateSetAsync(UpdateSetDTO set, int setId);
        Task<APIResponse<string>> deleteSetAsync(int setId);

    }
}
